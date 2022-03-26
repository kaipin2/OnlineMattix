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

    private Buttons btn; //ボタンのスクリプト
    private bool Updata = true;
    private bool RoomCreate = false; //自身がRoomを制作しているかどうか
    private int participants = 0; //現在同じ部屋に入っているプレイヤーの人数

    private GameObject[] RoomButton = new GameObject[Const.CO.Number];
     
    private string mode;                 // モード(ONLINE, OFFLINE)
    private string dispMessage = "";          // 画面項目：メッセージ
    private string dispRoomName;         // 画面項目：ルーム名
    private List<RoomInfo> roomDispList; // 画面項目：ルーム一覧
    private List<RoomInfo> roomButtonList; // 画面項目：ルーム一覧(Button表示するかどうか)

    private GameObject StatusTextObject;//Status状態を示すテキストが存在するObject
    private TextMeshProUGUI StatusText;//Status状態を示すテキスト

    private GameObject MessageTextObject;//Mesagge状態を示すテキストが存在するObject
    private TextMeshProUGUI MessageText;//Mesagge状態を示すテキスト

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

    //SecneをまたいでもObjectが破壊されないようにする
    static PhotonManager Instance = null;

    public static PhotonManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<PhotonManager>();
        }
        return Instance;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == Const.CO.LobbySceneName)
        {
            initParam();
        }

    }
    /*
    private void Awake()
    {
        if (this != GetInstance())
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    */
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
            if (PhotonNetwork.IsConnected)
            {
                RoomButtonCreate();//現在作られている部屋に入室するボタンを配置
            }

            //デバック用(「P」を押すと、現在の状況を表示)
            if (Input.GetKeyDown(KeyCode.P))
            {
                foreach (RoomInfo room in roomDispList) //Roomを1つずつ検索
                {
                    UnityEngine.Debug.Log("roomDispList:" + room);
                }
                foreach (RoomInfo room in roomButtonList) //Roomを1つずつ検索
                {
                    UnityEngine.Debug.Log("roomButtonList:" + room);
                }
                UnityEngine.Debug.Log("ONLINE:" + ONLINE);
                UnityEngine.Debug.Log("RoomCreate:" + RoomCreate);
                UnityEngine.Debug.Log("dispStatus:" + dispStatus);
                UnityEngine.Debug.Log("dispMessage:" + dispMessage);
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
            MessageText.text = "";
        }
        else
        {
            MessageText.text = "";
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            //UnityEngine.Debug.Log("in");
        }
        
        Updata = true;
        RoomCreate = false;
        ONLINE = false;
        dispMessage = "";

        //UnityEngine.Debug.Log("in");
        dispStatus = "OFFLINE";
        //dispStatus = Status.OFFLINE.ToString();
        roomDispList = new List<RoomInfo>();
        roomButtonList = new List<RoomInfo>();
        PhotonNetwork.GameVersion = Const.CO.GameVersion;
        for(int i = 0;i < Const.CO.Number ; i++)
        {
            RoomButton[i] = null;
        }

        btn = this.gameObject.AddComponent<Buttons>();//ボタンのスクリプトを追加

    }

    //現在作られている部屋に入室するボタンを配置
    public void RoomButtonCreate()
    {
        int RoomSave = -1; //どこにButtonのObjectを配置するか
        RoomInfo DeleteRoom_button = null; //削除する部屋(のボタン)
        int textName = 0; //テキストの場所(子供の第何要素か)

        //Roomが作られていて、自身がRoomを作っていない時
        if (roomDispList != null && roomDispList.Count > 0 && !RoomCreate && ONLINE)
        {
            foreach (RoomInfo room in roomDispList) //Roomを1つずつ検索
            {
                for (int i = 0; i < Const.CO.Number; i++)
                {
                    if (RoomButton[i] == null && RoomSave < 0) //空いているところに検索したRoomへ入るButtonを制作
                    {
                        RoomSave = i;
                        break;
                    }
                }

                if (roomButtonList.Contains(room)) //すでにButoonが作られているなら作らなくてよい
                {
                    RoomSave = -1;
                }

                if (RoomSave != -1 && RoomButton[RoomSave] == null) //Buttonの制作
                {
                    roomButtonList.Add(room); //作られたButtonにつながっている部屋を保存

                    Vector3 vec = new Vector3(-198 + (207 * (RoomSave / Const.CO.button_Ver)), 45 - (75 * (RoomSave % Const.CO.button_Ver)), 0);
                    Quaternion rotation = new Quaternion(0, 0, 0, 0);
                    Vector3 scale = new Vector3(2, 2, 2); //ボタンの大きさ(何倍か)
                    //ボタンのサイズ
                    int Width = 100;
                    int Height = 20;

                    RoomButton[RoomSave] = Instantiate(Roombutton); //インスタンスを生成
                    RoomButton[RoomSave].transform.position = vec; //ボタンの位置
                    RoomButton[RoomSave].transform.SetParent(GameObject.Find(Const.CO.RoomButtonParent).gameObject.transform, false);//ボタンの親を設定
                    RoomButton[RoomSave].transform.GetChild(textName).gameObject.GetComponent<TextMeshProUGUI>().text = room.Name; //ボタンのテキストを設定
                    RoomButton[RoomSave].name = Const.CO.RoomNameName; //ボタンオブジェクトの名前を設定
                    //ボタンのサイズ
                    var rtf = RoomButton[RoomSave].GetComponent<RectTransform>();
                    rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);// 横方向のサイズ
                    rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);// 縦方向のサイズ
                    //ボタンの大きさ
                    RoomButton[RoomSave].transform.localScale = scale;
                    //ボタンのアクションの設定
                    RoomButton[RoomSave].GetComponent<Button>().onClick.AddListener(btn.RoomEnterButton);
                    
                }

            }
        }

        if (roomDispList.Count != roomButtonList.Count) //作ったボタンと部屋の数が合わない時(作ったボタンの方が多いとき)
        {
            foreach (RoomInfo room in roomButtonList)
            {
                if (!roomDispList.Contains(room)) //ボタンを押したときに入る部屋が存在しない時
                {
                    for(int i = 0;i < Const.CO.Number ; i++)
                    {
                        if(RoomButton[i] != null)
                        {
                            //押したときに入る部屋が存在しないボタンを探す
                            if (RoomButton[i].transform.GetChild(textName).gameObject.GetComponent<TextMeshProUGUI>().text == room.Name)
                            {
                                Destroy(RoomButton[i]); //ボタンを削除
                                RoomButton[i] = null;
                                break;
                            }
                        }
                    }

                    DeleteRoom_button = room; //Butotnが削除されたことをメモ
                    break;
                }
            }
        }

        if (RoomCreate || !ONLINE) //自身がRoomを制作しているとき
        { //制作したButtonをすべて削除
            roomButtonList.Clear();

            for(int i = 0;i < Const.CO.Number; i++)
            {
                if(RoomButton[i] != null)
                {
                    Destroy(RoomButton[i]);
                    RoomButton[i] = null;
                }
            }
            System.Array.Clear(RoomButton, 0, RoomButton.Length);

        }
        roomButtonList.Remove(DeleteRoom_button);
    }

    //ロビーに入りなおす
    public void UpdataButtonOnClick()
    {
        Updata = false;
        // ロビーに入り直す
        roomDispList = new List<RoomInfo>();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
    }

    // Photonサーバ接続処理
    public void ConnectPhoton(bool boolOffline)
    {
        if (boolOffline)
        {
            // オフラインモードを設定
            mode = Status.OFFLINE.ToString();
            ONLINE = false;
            PhotonNetwork.OfflineMode = true; // OnConnectedToMaster()が呼ばれる
            dispMessage = "OFFLINEモードで起動しました。";
            SceneManager.LoadScene(Const.CO.MattixSceneName);
            //PhotonNetwork.LoadLevel(GameScene);
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
        Updata = true;
        ONLINE = false;
        RoomCreate = false;
        roomDispList.Remove(PhotonNetwork.CurrentRoom);
        PhotonNetwork.Disconnect();
    }

    // コールバック：Photonサーバ接続完了
    public override void OnConnectedToMaster()
    {

        base.OnConnectedToMaster();
        if (Status.ONLINE.ToString().Equals(mode))
        {
            dispStatus = Status.ONLINE.ToString();
            dispMessage = "サーバに接続しました。";
            ONLINE = true;
            // ロビーに接続
            PhotonNetwork.JoinLobby();
        }
    }

    // コールバック：Photonサーバ接続失敗
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        dispMessage = "サーバから切断しました。";
        dispStatus = Status.OFFLINE.ToString();
    }

    // コールバック：ロビー入室完了
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        if (Updata)
        {
            UpdataButtonOnClick();
        }
    }

    // ルーム一覧更新処理 (ロビーに入室した時、他のプレイヤーが更新した時のみ)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        // ルーム一覧更新
        foreach (var info in roomList)
        {
            if (!info.RemovedFromList && !roomDispList.Contains(info))
            {
                // 更新データが削除でない場合
                roomDispList.Add(info);
            }
            else if(info.RemovedFromList)
            {
                // 更新データが削除の場合
                roomDispList.Remove(info);
            }
        }
    }

    // ルーム作成処理
    public void CreateRoom(string roomName)
    {
        RoomCreate = true;
        dispRoomName = roomName;
        PhotonNetwork.JoinOrCreateRoom(roomName, RoomOPS, TypedLobby.Default);
    }

    // ルーム入室処理
    public void ConnectToRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);

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
        // 表示ルームリストに追加する
        roomDispList.Add(PhotonNetwork.CurrentRoom);
        DisplayJoinedRoom();
    }

    //入った部屋と入っているプレイヤーを表示
    public void DisplayJoinedRoom()
    {
        participants = PhotonNetwork.PlayerList.Length; //参加人数

        dispMessage = "【" + PhotonNetwork.CurrentRoom.Name + "】" + "に入室しました。\n";
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
            photonView.RPC(nameof(RpcGameStart), RpcTarget.All);
        }
        //部屋に0人入っている時（自分も入っていない）
        else if(PhotonNetwork.PlayerList.Length == 0)
        {
            dispMessage = "部屋に入室してください";
        }
        //部屋に１人か３人以上いる時(設定で２人までしか部屋に入れない)
        else
        {
            dispMessage = "人数が足りません\n" + dispMessage;
            
        }
    }

    [PunRPC] //ゲームが開始される時に呼び出す関数
    private void RpcGameStart()
    {
        SceneManager.LoadScene(Const.CO.MattixSceneName);
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
                NetworkSettingWindow, "ロビー");



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
                    CreateRoom(dispRoomName);
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
                    {
                        //UnityEngine.Debug.Log("roomInfo:"+ roomInfo);
                        if (GUILayout.Button(roomInfo.Name, GUI.skin.box, GUILayout.Width(360)))
                            ConnectToRoom(roomInfo.Name);
                    }

                }
                GUILayout.EndScrollView();
            }
        }
    }
    */
}