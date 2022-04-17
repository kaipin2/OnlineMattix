//ネット接続と対戦相手とのマッチングのスクリプト
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro; //TextMeshProを使用するのに必要
using UnityEngine.SceneManagement;
using UnityEngine.UI; //UIを使用するのに必要

using Const;  //定数を定義している

public class PhotonManager : MonoBehaviourPunCallbacks
{
    #region Private変数定義

    private int BoardSize = 0;

    private Buttons btn; //ボタンのスクリプト
    private bool ReEntry = false;
    private bool RoomCre = false; //自身がRoomを制作しているかどうか
    private bool RoomIn = false; //自身がRoomに入っているかどうか
    private int participants = 0; //現在同じ部屋に入っているプレイヤーの人数

    //private GameObject[] RoomButton = new GameObject[Const.CO.Number];

    private string mode;                 // モード(ONLINE, OFFLINE)
    private string dispMessage = "";          // 画面項目：メッセージ
    private string dispRoomName;         // 画面項目：ルーム名
    private List<RoomInfo> roomDispList; // 画面項目：ルーム一覧
    private List<GameObject> roomButtonList; // 画面項目：ルーム一覧(Button表示するかどうか)

    private GameObject StatusTextObject;//Status状態を示すテキストが存在するObject
    private TextMeshProUGUI StatusText;//Status状態を示すテキスト

    private GameObject MessageTextObject;//Mesagge状態を示すテキストが存在するObject
    private TextMeshProUGUI MessageText;//Mesagge状態を示すテキスト

    private GameObject OptionStatusControllerObject;
    private OptionStatusController OSC_Script;

    private bool ready = false; //準備ができているかどうかの関数
    private string OpponentName = ""; //対戦相手の名前
    #endregion

    #region Public変数定義

    public bool ONLINE = false; //自身がONLINEかどうか
    public GameObject Roombutton; //制作されている部屋に入るボタン
    public string dispStatus;           // 画面項目：状態
    #endregion

    #region Photonの設定変数定義
    // 状態
    public enum Status
    {
        ONLINE,   // オンライン
        OFFLINE,  // オフライン
    };
    public static string GameVersion = "Ver 1.1.2"; //ゲームバージョン指定（設定しないと警告が出る）
    //ルームオプションのプロパティー
    static RoomOptions RoomOPS = new RoomOptions()
    {
        MaxPlayers = 2, //0だと人数制限なし
        IsOpen = true, //部屋に参加できるか
        IsVisible = true, //この部屋がロビーにリストされるか
    };
    #endregion

    //ルームに入っているか判定するプロパティ
    public bool Room{
        set { RoomIn = value; }
        get { return RoomIn; }
    }


    private void Start()
    {
        if (SceneManager.GetActiveScene().name == Const.CO.LobbySceneName)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.IsMessageQueueRunning = true;
            initParam();
            initMessage();
            initPhoton();
        }

    }

    public void SetMessage(string m)
    {
        dispMessage = m;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == Const.CO.LobbySceneName)
        {
            StatusText.text = Const.CO.LobbyTitle + " <color=red>" + dispStatus + "</color>";
            MessageText.text = dispMessage;
            //ONLINEの時
            if (PhotonNetwork.CurrentRoom != null && ONLINE)
            {
                if (PhotonNetwork.PlayerList.Length != participants)//参加人数に変動があった場合
                {
                    
                    DisplayJoinedRoom(); // 表示を更新
                }

            }
            if (ready && PhotonNetwork.PlayerList.Length < RoomOPS.MaxPlayers)
            {
                dispMessage = "相手がいなくなりました\n" + dispMessage;
                ready = false;
            }

            //デバック用(「P」を押すと、現在の状況を表示)
            if (Input.GetKeyDown(KeyCode.P))
            {
                foreach (RoomInfo room in roomDispList) //Roomを1つずつ検索
                {
                    UnityEngine.Debug.Log("roomDispList:" + room);
                }
                foreach (GameObject room in roomButtonList) //Roomを1つずつ検索
                {
                    UnityEngine.Debug.Log("roomButtonList:" + room);
                }
                UnityEngine.Debug.Log("ONLINE:" + ONLINE);
                UnityEngine.Debug.Log("RoomCreate:" + RoomCre);
                UnityEngine.Debug.Log("dispStatus:" + dispStatus);
                UnityEngine.Debug.Log("dispMessage:" + dispMessage);
                UnityEngine.Debug.Log("RoomOPS.IsVisible:" + RoomOPS.IsVisible);
            }
        }
    }

    // 変数初期化処理
    private void initParam()
    {
        if (StatusTextObject == null)
        {
            StatusTextObject = GameObject.Find(Const.CO.StatusTextName).gameObject;
        }
        if (StatusText == null)
        {
            StatusText = StatusTextObject.GetComponent<TextMeshProUGUI>();
        }
        if (MessageTextObject == null)
        {
            MessageTextObject = GameObject.Find(Const.CO.MessageTextName).gameObject;
        }
        if (MessageText == null)
        {
            MessageText = MessageTextObject.GetComponent<TextMeshProUGUI>();
        }
        if(OptionStatusControllerObject == null)
        {
            OptionStatusControllerObject = GameObject.Find(Const.CO.AudioCanvasName).gameObject;
        }
        OSC_Script = OptionStatusControllerObject.GetComponent<OptionStatusController>();

        MessageText.text = "";

        ready = false;
        ReEntry = false;
        RoomCre = false;
        Room = false;
        ONLINE = true;

        dispStatus = "OFFLINE";
        //dispStatus = Status.OFFLINE.ToString();
        roomDispList = new List<RoomInfo>();
        roomButtonList = new List<GameObject>();
        PhotonNetwork.GameVersion = Const.CO.GameVersion;
        btn = this.gameObject.AddComponent<Buttons>();//ボタンのスクリプトを追加

        


    }

    //ネットへの接続状態の初期化(ONLINEで初期化)
    private void initPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        ConnectPhoton(ONLINE); //オンライン
    }

    //テキストを初期化
    private void initMessage()
    {
        dispMessage = "";
    }

    //現在作られている部屋に入室するボタンを配置
    public void RoomButtonCreate(RoomInfo r)
    {
        GameObject roomButton = null;

        string[] split = r.Name.Split(' '); //split[0]:Name , split[1]:盤面の大きさ（縦）,split[2]:盤面の大きさ（横） 
        GameObject obj = GameObject.Find(split[0]);

        //すでに存在していたのなら情報の更新
        if (obj)
        {
            roomButton = obj;
            RoomInfoUpdate(obj, r,split);
        }
        //新しく作られたルームならばボタンの作成
        else
        {
            roomButton = (GameObject)Instantiate(Roombutton);
            roomButton.transform.SetParent(GameObject.Find(Const.CO.RoomButtonParent).gameObject.transform, false);

            //ボタンのアクションの設定
            roomButton.GetComponent<Button>().onClick.AddListener(() => btn.RoomEnterButton(r.Name));
            RoomInfoUpdate(roomButton, r,split);
            //生成したボタンの名前を作成するルームの名前にする

            roomButton.name = split[0];
            roomButton.GetComponentsInChildren<TextMeshProUGUI>()[1].text = split[1] + " * " + split[2];

            roomButtonList.Add(roomButton);
        }

        RoomButtonPosition(roomButton);  //ルームボタンの配置の更新
        
    }

    //ルームボタンの削除
    public void RoomButtonDelete(RoomInfo r)
    {
        string[] split = r.Name.Split(' '); //split[0]:Name , split[1]:盤面の大きさ（縦）,split[2]:盤面の大きさ（横）
        GameObject obj = GameObject.Find(split[0]);
        //ボタンが存在すれば削除
        if (obj)
        {
            GameObject.Destroy(obj);
            roomButtonList.Remove(obj);
        }
        foreach(GameObject button_obj in roomButtonList){
            RoomButtonPosition(button_obj);
        }
    }

    //ルームボタンのInfoの更新
    public void RoomInfoUpdate(GameObject button, RoomInfo info,string[] split)
    {
        foreach (TextMeshProUGUI t in button.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (t.name == "RoomName")
            {
                t.text = split[0];
            }
            else if (t.name == "MaxPlayerCount")
            {
                t.text = info.MaxPlayers.ToString();
            }
            else if (t.name == "RoomInPlayerCount")
            {
                t.text = info.PlayerCount.ToString();
            }else if (t.name == "BoardSizeText")
            {

            }
        }
    }

    //ルームボタンの配置の更新
    public void RoomButtonPosition(GameObject roomButton)
    {
        Vector3 vec = Vector3.zero;

        for (int i = 0; i < roomButtonList.Count; i++)
        {
            bool hit = false;

            vec = new Vector3(-235 + (447 * (i % Const.CO.button_Side)), 66 - (110 * (i / Const.CO.button_Side)), 0);
            for(int j = 0; j < roomButtonList.Count; j++)
            {
                if (vec == roomButtonList[j].transform.localPosition)
                {
                    hit = true;
                    break;
                }
            }
            if (!hit) break;
        }
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            Vector3 scale = new Vector3(1, 1, 1); //ボタンの大きさ(何倍か)
                                                  //ボタンのサイズ
            int Width = 426;
            int Height = 90;

            roomButton.transform.localPosition = vec; //ボタンの位置
            //ボタンの大きさ
            roomButton.transform.localScale = scale;
            //ボタンのサイズ
            var rtf = roomButton.GetComponent<RectTransform>();
            rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);// 横方向のサイズ
            rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);// 縦方向のサイズ
        
    }

    //ロビーに入りなおす
    public void UpdataButtonOnClick()
    {
        // ロビーに入り直す
        roomDispList = new List<RoomInfo>();
        roomButtonList = new List<GameObject>();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
    }

    // Photonサーバ接続処理
    public void ConnectPhoton(bool boolOnline)
    {
        if (!boolOnline)
        {
            // オフラインモードを設定
            mode = Status.OFFLINE.ToString();
            ONLINE = false;
            PhotonNetwork.OfflineMode = true; // OnConnectedToMaster()が呼ばれる
            dispMessage = "OFFLINEモードで起動しました。";
            SceneManager.LoadScene(Const.CO.MattixSceneName);
            return;
        }
        // Photonサーバに接続する
        mode = Status.ONLINE.ToString();
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Photonサーバ切断処理
    public void DisConnectPhoton()
    {
        Room = false;
        ONLINE = false;
        RoomCre = false;
        roomDispList.Clear();
        PhotonNetwork.Disconnect();
    }

    // コールバック：Photonサーバ接続完了
    public override void OnConnectedToMaster()
    {
        ready = false; //対戦準備ができていない
        base.OnConnectedToMaster();
        if (Status.ONLINE.ToString().Equals(mode))
        {
            //initParam();
            dispStatus = Status.ONLINE.ToString();
            Room = false;
            if (dispMessage == "") dispMessage = "サーバに接続しました。";
            else if(dispMessage != "ルームに入室していないのでルームからの退出はできません") dispMessage = "ルームから退出しました";
            ONLINE = true;
            // ロビーに接続
            PhotonNetwork.JoinLobby();
        }
    }

    // コールバック：Photonサーバ接続失敗
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Room = false;
        dispMessage = "サーバから切断しました。";
        dispStatus = Status.OFFLINE.ToString();
    }

    // コールバック：ロビー入室完了
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        if (ReEntry)
        {
            ReEntry = false;
            ConnectToRoom(dispRoomName);
        }
    }

    // ルーム一覧更新処理 (ロビーに入室した時、他のプレイヤーが更新した時のみ)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        roomDispList = roomList;
        int number = 0;

        foreach (RoomInfo r in roomList)
        {
            //プレイヤーが存在しているルーム
            if (r.PlayerCount > 0)
            {
                RoomButtonCreate(r);
            }
            else
            {
                RoomButtonDelete(r);
            }
            number++;
        }
    }

    // ルーム作成処理
    public void RoomCreate(string roomName)
    {
        if (Room == false)
        {
            BoardSize = OSC_Script.Size;
            string name = roomName + " " + BoardSize + " " + BoardSize;
            RoomCre = true;
            Room = true;
            dispRoomName = name;
            PhotonNetwork.JoinOrCreateRoom(name, RoomOPS, TypedLobby.Default);
        }
    }

    // ルーム入室処理
    public void ConnectToRoom(string roomName)
    {
        dispRoomName = roomName;
        if (Room)
        {
            PhotonNetwork.LeaveRoom();
            Room = false;
            ReEntry = true;
            
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    //ルーム退出処理
    public void Disconnect()
    {
        if (Room)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            dispMessage = "ルームに入室していないのでルームからの退出はできません";
        }
    }
    // コールバック：ルーム作成完了
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        dispMessage = "ルームを作成しました。";
    }

    // コールバック：ルーム作成失敗
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        dispMessage = "ルーム作成に失敗しました。";
    }

    // コールバック：ルームに入室した時
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Room = true;
        // 表示ルームリストに追加する
        roomDispList.Add(PhotonNetwork.CurrentRoom);
        DisplayJoinedRoom();
    }
    // コールバック：ルーム入室に失敗した時
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        dispMessage = "ルーム入室に失敗しました:"+message;
    }
    //入った部屋と入っているプレイヤーを表示
    public void DisplayJoinedRoom()
    {
        participants = PhotonNetwork.PlayerList.Length; //参加人数
        string[] split = PhotonNetwork.CurrentRoom.Name.Split(' ');

        dispMessage = "【" + split[0] +"   "+split[1]+"×"+ split[2] + "】" + "に入室しました\n";
        foreach (var p in PhotonNetwork.PlayerList)
        {
            dispMessage += p.NickName + "\n";
        }
        dispMessage += "参加人数" + participants + "人";
        
    }

    //GameStart処理
    public void GameStart()
    {
        //部屋に2人入っている時
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            dispMessage = "対戦相手の行動を待っています";
            foreach (var p in PhotonNetwork.PlayerListOthers)
            {
                OpponentName = p.NickName;
                dispMessage += "\n"+OpponentName + ":準備中";
            }
            ready = true;
            PhotonNetwork.IsMessageQueueRunning = false;
            photonView.RPC(nameof(RpcReadyGame), RpcTarget.Others);
            PhotonNetwork.IsMessageQueueRunning = true;
        }
        //部屋に0人入っている時（自分も入っていない）
        else if(PhotonNetwork.PlayerList.Length == 0)
        {
            dispMessage = "部屋に入室してください";
        }
        //部屋に１人か３人以上いる時(設定で２人までしか部屋に入れない)
        else
        {
            if(dispMessage.Split('\n')[0] != "人数が足りません")
            {
                dispMessage = "人数が足りません\n" + dispMessage;
            }
            
        }
    }

    [PunRPC] //ゲームが開始される時に呼び出す関数
    private void RpcReadyGame()
    {
        if (ready)
        {
            photonView.RPC(nameof(RpcGameStart), RpcTarget.All);
        }
    }

    [PunRPC] //ゲームが開始される時に呼び出す関数
    private void RpcGameStart()
    {
        PhotonNetwork.LoadLevel(Const.CO.MattixSceneName);

    }
   


    
    /*
    // ---------- 設定GUI ----------
    void OnGUI()
    {
        float scale = Screen.height / 480.0f;
        GUI.matrix = Matrix4x4.TRS(new Vector3(
            Screen.width * 0.5f, Screen.height * 0.5f, 0),
            Quaternion.identity,
            new Vector3(scale, scale, 1.0f));

        GUI.Window(0, new Rect(-200, -200, 400, 400),
            NetworkSettingWindow, "Photon接続テスト");
    }

    Vector2 scrollPosition;
    void NetworkSettingWindow(int windowID)
    {
        // ステータス, メッセージの表示
        GUILayout.BeginHorizontal();
        GUILayout.Label("状態: " + dispStatus, GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        if (Status.ONLINE.ToString().Equals(dispStatus))
        {
            // サーバ接続時のみ表示
            if (GUILayout.Button("切断"))
                DisConnectPhoton();
        }
        GUILayout.EndHorizontal();
        GUILayout.Label(dispMessage);
        GUILayout.Space(20);

        if (!Status.ONLINE.ToString().Equals(dispStatus))
        {
            // --- 初期表示時、OFFLINEモードのみ表示
            // マスターサーバに接続する
            GUILayout.Label("【モード選択】");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ONLINE Mode"))
                ConnectPhoton(false);
            if (GUILayout.Button("OFFLINE Mode"))
                ConnectPhoton(true);
            GUILayout.EndHorizontal();
        }
        else if (Status.ONLINE.ToString().Equals(dispStatus))
        {
            // --- ONLINEモードのみ表示
            if (!(PhotonNetwork.CurrentRoom != null))
            {
                // ルーム作成
                GUILayout.Label("【ルーム作成】");
                GUILayout.BeginHorizontal();
                GUILayout.Label("　ルーム名: ");
                dispRoomName = GUILayout.TextField(dispRoomName, GUILayout.Width(150));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                // 作成ボタン
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("作成 & 入室"))
                {
                    RoomCreate(dispRoomName);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(20);

                // ルーム一覧
                GUILayout.Label("【ルーム一覧 (クリックで入室)】");
                // 一覧表示
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(380), GUILayout.Height(100));
                if (roomDispList != null && roomDispList.Count > 0)
                {
                    // 更新ボタン
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("更新"))
                    {
                        // ロビーに入り直す
                        roomDispList = new List<RoomInfo>();
                        PhotonNetwork.LeaveLobby();
                        PhotonNetwork.JoinLobby();
                    }
                    // ルーム一覧
                    GUILayout.EndHorizontal();
                    foreach (RoomInfo roomInfo in roomDispList)
                        if (GUILayout.Button(roomInfo.Name, GUI.skin.box, GUILayout.Width(360)))
                            ConnectToRoom(roomInfo.Name);
                }
                GUILayout.EndScrollView();
            }
        }
    }
    */
}
