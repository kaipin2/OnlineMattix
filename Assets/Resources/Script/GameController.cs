//ゲーム画面のメインスクリプト
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

using Const;  //定数を定義している

public class GameController : MonoBehaviourPunCallbacks //MonoBehaviour
{
    //盤面は右下を(0,0)とし、x座標(左右)は左を正,y座標(上下)は上を正とする

    #region Private変数定義
    /*PaintColor関数で色を塗るための条件分岐として
     * 盤面の色を塗るのなら「int BoardNumber」
     * 駒の外側の色を塗るのなら「int PointNumber」
     * 駒の内側の色を塗るのなら「int InsideNumber」
     * で表している
     * 
     * ゲーム番の大きさは
     * 縦の大きさを「int Ver」
     * 横の大きさを「int Si」
     * で表している
     * 
     * 駒の得点は
     * 上限「int Max」
     * 下限「int Min」
     * で決めており、その間の整数の得点しか登場しない
     * 
     * ゲームの手番を
     * 先手の手番なら「int SIDE」
     * 後手の手番なら「int VERTICAL」
     * を「currentPlayer」に入れる
     */

    //盤面の大きさ
    private int Ver;
    private int Si;

    //ゲームがOFFLINEかONLINEか判断(ture:ONLINE,false:OFFLINE)
    private bool ONLINE = false;
    //盤面に置いてある駒をint型2次元配列を定義(スタート駒:MAIN,数字の駒:その値,空白:EMPTY)
    private int[,] squares;
    //現在のプレイヤー(初期プレイヤーは横(SIDE))
    private int currentPlayer = Const.CO.SIDE;
    //自分の手番かどうか(ture:自分の手番,false:相手の手番)
    private bool TurnPlayer = false;
    //main石の場所(盤面の右下が(0,0)である)
    private int mainX = -1;
    private int mainY = -1;
    //盤上に置いてある駒をGameObject型２次元配列で定義
    private GameObject[,] Piece;
    //石の取った枚数(A:先手,B:後手)
    private int stoneA = 0;
    private int stoneB = 0;
    //取った石の合計点数(A:先手,B:後手)
    private int ScoreA = 0;
    private int ScoreB = 0;
    //取った石をGameObject型の配列で保存(GetPieceA:先手,GetPieceB:後手)
    private GameObject[] GetPieceA;
    private GameObject[] GetPieceB;
    //取った石の合計得点を記載するテキスト(A:先手,B:後手)
    private TextMeshProUGUI scoreA_text;
    private TextMeshProUGUI scoreB_text;
    //制限時間を表示するテキスト
    private TextMeshProUGUI timeA_text;
    private TextMeshProUGUI timeB_text;

    //カメラ情報
    private Camera camera_object;
    //クリックしたオブジェクトを保存
    private RaycastHit hit;
    //「AudioSource」のComponent
    private AudioSource audioSource;
    //Resources/Audio/SEの中で「MovingSound」が何番目に入っているか(0から数えて)
    private static int MovingSound = 1;
    //盤上に置いてある駒の点数表示をするObjectを配列で定義
    private GameObject[] Point;
    //盤上に置いてある駒の点数表示をするテキストを配列で定義
    private TextMeshProUGUI[] PointText;
    //盤面をGameobject型の２次元配列で定義
    private GameObject[] Board;
    //オブジェクト(ゲーム盤)の名前を分割する
    private static char[] separetechar1 = Const.CO.BoardName.ToCharArray();
    private static char[] separetechar2 = Const.CO.BoardName_Connect.ToCharArray();
    private char[] separate = { separetechar1[separetechar1.Length - 1] , separetechar2 [separetechar2.Length - 1] }; //Cube(数字)_(数字)の数字の部分を取り出す
    //音量を設定する画面のObject
    private GameObject audiocanvas;
    //音量を設定するObject
    private GameObject VolumeManager;
    //音量を設定するスクリプト
    private AudioManager VCscript;

    //盤面の大きさを指定しているObjectのスクリプト
    private OptionStatusController PW_Script;

    //canvasのComponent
    private Canvas Maincanvas; //メイン画面のCanvasのComponent
    private Canvas Subcanvas; //(ゲーム終了後の)追加画面のCanvasのComponent
    private Canvas Audiocanvas; //音量画面のCanvasのComponent
    //文字のColor
    private Color FirstColor = Color.red; //先手のイメージ色
    private Color SecondColor = Color.yellow; //後手のイメージ色
    private Color DefaltColor = Color.black; //文字のデフォル色
    private Color WINColor = Color.red; //勝利のテキストの色

    private float seconds = 0; //経過時間(秒)
    private float old_seconds = 0; //経過時間を秒単位で動かすための変数
    private static int time = 5 * 60;　//制限時間(s)
    private int time_A = time; //先手の残り時間
    private int time_B = time; //後手の残り時間
    private int TimeUp = 0;//０なら時間切れしていない、1なら先手,２なら後手の時間切れ

    private bool finish = false; //ゲームが終了しているか判定する変数

    //Audio(BGM)
    private SoundPresenter SoundP; //SoundPresenterのスクリプト
    private Canvas Audio; //AudioCanvas

    private Buttons btn; //ボタンのスクリプト

    //デバッグ用ボード配置
    //private int[,] Board_pre = new int[,] { { 3, -7, -3 }, { 2, -3, 7 }, { -4, 100, 1 } };
    #endregion

    #region PrivateAccessor定義
    private int TimeCount
    {
        get {
            if (currentPlayer == Const.CO.SIDE)
            {
                return time_A;
            }else if (currentPlayer == Const.CO.VERTICAL)
            {
                return time_B;
            }
            return -1;//エラー
        }
        set {
            if (currentPlayer == Const.CO.SIDE)
            {
                time_A = value;
            }
            else if (currentPlayer == Const.CO.VERTICAL)
            {
                time_B = value;
            }
            
        }
    }
    private TextMeshProUGUI TimeText
    {
        get
        {
            if (currentPlayer == Const.CO.SIDE)
            {
                return timeA_text;
            }
            else if (currentPlayer == Const.CO.VERTICAL)
            {
                return timeB_text;
            }
            return null;//エラー
        }
        set
        {
            if (currentPlayer == Const.CO.SIDE)
            {
                timeA_text = value;
            }
            else if (currentPlayer == Const.CO.VERTICAL)
            {
                timeB_text = value;
            }

        }
    }

    #endregion

    #region Public変数定義

    //prehabs
    public GameObject mainPiece; //スタート駒のObject
    public GameObject pointPiece; //ポイント駒のObject
    public GameObject scoreA_object;//プレイヤーAのスコア表示のObject
    public GameObject scoreB_object;//プレイヤーBのスコア表示のObject
    public GameObject timeA_object;//プレイヤーAのTime表示のObject
    public GameObject timeB_object;//プレイヤーBのTime表示のObject
    public GameObject Result_score;//結果表示のObject
    public GameObject WIN; //勝者を表すテキストのObject
    public GameObject Turn; //どちらの手番か示すテキストのObject
    public GameObject Turn_Status; //自分がどちらの手番か示すテキストのObject
    public GameObject button = null; //ゲームが終了した後表示する、ボタンのObject
    public GameObject title_button = null; //ゲームが終了した後表示する、タイトルに戻るボタンのObject
    public GameObject canvas; //ゲームのメイン画面のObject
    public GameObject canvasletter; //ゲーム終了後の(追加)画面のObject
    public GameObject Panel;
    public GameObject gameboard; //ゲームの盤面(１マス)のObject
    public GameObject board; //ゲームの盤面を表示する場所のObject
    public GameObject PlayerA; //先手プレイヤーの取った駒を置く場所のObject
    public GameObject PlayerB; //後手プレイヤーの取った駒を置く場所のObject

    //マテリアル
    public Material board1Color; //盤面の色(１色目)
    public Material board2Color; //盤面の色(２色目)
    public Material SelectBoardColor; //現在取れる場所を表示する色
    public Material pointColor; //プラスのポイント駒の外側の色
    public Material unpointColor; //マイナスのポイント駒の外側の色
    public Material PointInside; //ポイント駒の内側の色
    

    #endregion

    #region PublicAccessor定義
    //メイン画面のAccessor
    public Canvas MainCanvas
    {
        get { return Maincanvas; }
        set { Maincanvas = value; }
    }
    //サブ画面のAccessor
    public Canvas SubCanvas
    {
        get { return Subcanvas; }
        set { Subcanvas = value; }
    }
    //音量画面のAccessor
    public Canvas AudioCanvas
    {
        get { return Audiocanvas; }
        set { Audiocanvas = value; }
    }
    public bool ON_LINE
    {
        get { return ONLINE; }
        set { ONLINE = value; }
    }

    #endregion

    #region Private関数
    //最初に呼び出される関数
    private void Start()
    {
        //変数の初期化
        Initialize();

        #region ゲーム盤面生成
        //自分がこのゲームのHostなら
        if (PhotonNetwork.IsMasterClient)
        {
            //配列を初期化
            InitializeArray();
            //ゲーム盤を取得
            BoardGet();
        }
        //盤面を生成
        GenerateArray();
        //自分がこのゲームのHostなら
        if (PhotonNetwork.IsMasterClient)
        {
            //移動可能位置を可視化
            SelectableBoard();
        }
        #endregion

        //取った駒を置く位置を示すObject(A:先手,B:後手)
        PlayerA = GameObject.Find(Const.CO.FirstGetPositionName);
        PlayerB = GameObject.Find(Const.CO.SecondGetPositionName);

        //ターン表示
        Turn.GetComponent<TextScript>().ChangeText(Const.CO.FirstTrunText); //どちらの手番であるかテキスト表示(先手の手番)
        Turn.GetComponent<TextScript>().ChangeAlignment(Const.CO.Right); //文字を右寄せにする
        Turn.GetComponent<TextScript>().ChangeColor(FirstColor); //文字を先手のイメージ色にする

        //自分が先手か後手かを確認
        CheckTurnPlayer();
        if (ONLINE) 
        { 
        
            
            if (TurnPlayer)
            {
                Turn_Status.GetComponent<TextScript>().ChengeText_Diff(Const.CO.FirstPlayerText); //自分が先手番であると表示
            }
            else
            {
                Turn_Status.GetComponent<TextScript>().ChengeText_Diff(Const.CO.SecondPlayerText); //自分が後手番であると表示
            }
        }
        else
        {
            Turn_Status.GetComponent<TextScript>().ChangeText(Const.CO.OnePlayerText); //自分が後手番であると表示
        }


    }

    // Update is called once per frame
    private void Update()
    {

        if (!finish)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                TimeManager();
            }
           
            //ONLINEで参加人数が２人未満(途中で相手が落ちたとき)はタイトルに戻る
            if (ONLINE) //ONLINEの時
            {

                //参加人数が２人未満(途中で相手が落ちたとき)
                if (PhotonNetwork.PlayerList.Length < 2)
                {
                    //PhotonNetwork.Disconnect(); //NetWorkを切断
                    //PhotonNetwork.LoadLevel(Const.CO.LobbySceneName); //タイトルに戻る
                    SceneManager.LoadScene(Const.CO.LobbySceneName); //タイトルに戻る
                }
            }

            //マウスがクリックされたとき
            if (Input.GetMouseButtonDown(0) && Maincanvas.enabled == true)
            {
                //マウスのポジションを所得してRayに代入
                Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);

                //マウスのポジションからRayを投げて何かに当たったらhitに入れる
                if (Physics.Raycast(ray, out hit))
                {
                    //当たった盤面(１マス)の名前からx,y座標を取り出す
                    string[] str = hit.collider.gameObject.name.Split(separate);

                    //x,yの値を盤面の名前から取得(Cube(x座標)_(y座標))
                    int x = int.Parse(str[1]);
                    int y = int.Parse(str[2]);

                    //スコアの更新を行う
                    try
                    {
                        ScoreUP(x, y);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        //無効なクリック
                        //UnityEngine.Debug.Log("無効なクリックです");    
                    }
                }
            }
        }
    }

    //変数の初期化とObjectやComponentの取得など
    private void Initialize()
    {
        //Colorのシリアライズの登録しています
        //Color型をRPCの入力にするために「ColorSerializer」の関数を使う
        ColorSerializer.Register();

        finish = false;//ゲームが終了していない

        btn = this.gameObject.AddComponent<Buttons>();//ボタンのスクリプトを追加

        //ONLINEでゲームをしているなら
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            //UnityEngine.Debug.Log("ONLINE");
            ONLINE = true;
        }
        //OFFLINEでゲームをしているなら
        else
        {
            //UnityEngine.Debug.Log("OFFLINE");
            ONLINE = false;
        }
        
        //画面のObjectの取得
        audiocanvas = GameObject.Find(Const.CO.AudioCanvasName).gameObject; //音量画面のObjectを取得
        Maincanvas = canvas.GetComponent<Canvas>(); //メイン画面のObjectを取得
        Subcanvas = canvasletter.GetComponent<Canvas>(); //サブ画面のObjectを取得
        Audiocanvas = audiocanvas.GetComponent<Canvas>(); //音量画面のCanvasのComponentを取得

        //メイン画面(0)を表示
        CanvasDisplay(0);

        //BGM
        VolumeManager = GameObject.Find(Const.CO.AudioObjectName); //音量操作のObjectを取得
        VCscript = VolumeManager.GetComponent<AudioManager>(); //音量操作のスクリプトを取得
        VCscript.PlayBGM(0);



        //スコア情報を表示するテキスト(A:先手,B:後手)
        scoreA_text = scoreA_object.GetComponent<TextMeshProUGUI>();
        scoreB_text = scoreB_object.GetComponent<TextMeshProUGUI>();
        //制限時間を表示するテキスト
        timeA_text = timeA_object.GetComponent<TextMeshProUGUI>();
        timeB_text = timeB_object.GetComponent<TextMeshProUGUI>();
        timeA_text.text = "TIME:"+(time_A / 60).ToString("00") + ":"+(time_A % 60).ToString("00");
        timeB_text.text = "TIME:"+(time_B / 60).ToString("00") + ":"+(time_B % 60).ToString("00");
        //ONLINEでゲームをしているなら
        if (ONLINE)
        {
            //先手の名前と点数(０点)を表示
            scoreA_object.GetComponent<TextScript>().ChangeText(PhotonNetwork.PlayerList[0].NickName + ":0");
            //後手の名前と点数(０点)を表示
            scoreB_object.GetComponent<TextScript>().ChangeText(PhotonNetwork.PlayerList[1].NickName + ":0");

        }

        //カメラのComponent(Camera)を取得
        camera_object = GameObject.Find(Const.CO.MainCamera).GetComponent<Camera>();

        //効果音を操作するスクリプトを取得
        audioSource = GetComponent<AudioSource>();

        //盤面の大きさを決定する
        PW_Script = GameObject.Find(Const.CO.AudioCanvasName).GetComponent<OptionStatusController>();
        SetBoardSize(PW_Script.Size, PW_Script.Size);

        //盤面の大きさを使用した変数を定義
        squares = new int[Si, Ver];
        Piece = new GameObject[Si, Ver];
        GetPieceA = new GameObject[Si * Ver / 2];
        GetPieceB = new GameObject[Si * Ver / 2];
        Point = new GameObject[Si * Ver];
        Board = new GameObject[Si * Ver];
        //駒のポイントを表示するテキスト
        PointText = new TextMeshProUGUI[Si * Ver];

        //自分の手番かどうかを示す変数をfalseで初期化
        TurnPlayer = false;
        //RandomTurn(); //どちらが先手か決める関数
    }

    //盤面の大きさをセットする関数
    private void SetBoardSize(int ver,int si)
    {
        Ver = ver;
        Si = si;
    }

    //時間を管理する
    private void TimeManager()
    {
        seconds += Time.deltaTime;
        if ((int)seconds != (int)old_seconds)
        {
            int time = TimeCount - 1;
            TextMeshProUGUI time_text = TimeText;
            int judge = currentPlayer == Const.CO.SIDE ? 1:2; //時間切れになる可能性があるのはどちらか(1:先手,2:後手)

            int sec = (int)time % 60;
            int min = (int)time / 60;
            string str = "TIME:" + min.ToString("00") + ":" + ((int)sec).ToString("00");
            if (ONLINE)
            {
                photonView.RPC(nameof(RpcTimeUpdate), RpcTarget.All, currentPlayer, str, time);
            }
            else
            {
                time_text.text = str;
            }

            if (time <= 0f)
            {
                TimeUp = judge;
                Result();
            }

            TimeCount = time;
            TimeText = time_text;
        }
        old_seconds = seconds;
    }
    //表示する画面を変更する
    private void CanvasDisplay(int i)
    {
        //表示する画面を選択
        switch (i)
        {
            case 0: //MainCanvasを表示
                Maincanvas.enabled = true;
                Subcanvas.enabled = false;
                Audiocanvas.enabled = false;
                break;
            case 1:　//MainCanvasとSubCanvasを表示
                Maincanvas.enabled = true;
                Subcanvas.enabled = true;
                Audiocanvas.enabled = false;
                break;
            case 2:　//AudioCanvasを表示
                Maincanvas.enabled = false;
                Subcanvas.enabled = false;
                Audiocanvas.enabled = true;
                break;
            default:
                break;
        }
    }

    private void RandomTurn()
    {
        if (ONLINE)
        {
            //自分がこのゲームのHostなら自分が先手か後手かを設定
            if (PhotonNetwork.IsMasterClient)
            {
                int value = UnityEngine.Random.Range(1, 2 + 1);
                Board[0].GetComponent<BoardScript>().ChangeOwnership(value);
            }
        }
    }
    //ゲーム盤面に関わる配列と変数を初期化する
    private void InitializeArray()
    {
        //配列の初期化&盤上の状況を初期化
        for (int y = 0; y < Ver; y++)
        {
            for (int x = 0; x < Si; x++)
            {
                //配列を空(値は0)にする
                squares[x, y] = Const.CO.EMPTY;
                //駒のObjectがあるなら消去
                Destroy(Piece[x, y]);
            }
        }

        //手番の初期化(先手(横移動)が先)
        currentPlayer = Const.CO.SIDE;

        //main石の場所の初期化
        mainX = -1; //x座標
        mainY = -1; //y座標

        //石の取った枚数の初期化(A:先手,B:後手)
        stoneA = 0;
        stoneB = 0;

        //取った石の状況の初期化(A;先手,B:後手)
        for (int i = 0; i < (Si * Ver / 2); i++)
        {
            Destroy(GetPieceA[i]); //先手が取った駒があるなら消去
            Destroy(GetPieceB[i]); //後手が取った駒があるなら消去
        }

        //取った石の合計点数の初期化(A;先手,B:後手)
        ScoreA = 0;
        ScoreB = 0;

    }

    //駒を配置を決定する配列を生成
    private void GenerateArray()
    {
        //mainの駒の配置をランダム配置
        int MainPieceX = 2;//(int)UnityEngine.Random.Range(0.0f, (float)Si); //x座標
        int MainPieceY = 1;//(int)UnityEngine.Random.Range(0.0f, (float)Ver); //y座標

        //スタート駒の位置を設定(int値)
        squares[MainPieceX, MainPieceY] = Const.CO.MAIN;

        //for文を利用して配列アクセス(盤面の右下から順番になんの駒を置くか設定)
        for (int y = 0; y < Ver; y++)
        {
            for (int x = 0; x < Si; x++)
            {
                //何点の駒を置くか処理(0はmain駒)
                int point = 0;

                //スタート駒以外の場所なら
                if (squares[x, y] == Const.CO.EMPTY)
                {
                    do
                    {
                        
                        //Min～Max内の整数値のポイント駒を設定(int値)
                        point = (int)UnityEngine.Random.Range((float)Const.CO.Min, (float)Const.CO.Max);

                        //デバッグ用ボード配置(上の行をコメントアウト)
                        //point = Board_pre[x, y];
                        
                        squares[x, y] = point;
                        
                    } while (point == 0);
                }

                //ゲームのHostなら、決定した駒を置く
                if (PhotonNetwork.IsMasterClient)
                {
                    PieceGet(x, y, point);
                }
            }
        }
    }

    //駒を生成した配列にしたがって置く
    private void PieceGet(int x, int y, int point)
    {
        #region 変数定義
        //指定された盤面の位置
        Vector3 tmps = Board[y * Si + x].transform.localPosition;
        //画面のz座標の位置
        float pre_z = -canvas.transform.position.z;
        //指定された盤面の位置(z座標を変更)
        Vector3 vec = new Vector3(tmps.x, tmps.y, pre_z);
        //画面に表示する駒(1つ)のPrehabがあるURLの保存場所
        string Pre_Piece;
        //駒の回転方向(初期化)
        Quaternion Pre_rotation = new Quaternion(0, 0, 0, 0);
        //駒の回転方向(足し合わせ)
        Pre_rotation = Quaternion.AngleAxis(90, Vector3.right);
        //駒の大きさ
        float piecescale = (float)(Const.CO.MaxScale / Si);
        //駒の厚さ
        float piecethickness = 0.01f;
        #endregion

        //スタート駒(point=0)を置くとき
        if (point == 0)
        {
            //スタート駒のPrehabが保存されているURL
            Pre_Piece = Const.CO.GameBoardURL + mainPiece.name;
            //ONLINEなら
            if (ONLINE)
            {
                //駒のインスタンス生成
                Piece[x, y] = PhotonNetwork.Instantiate(Pre_Piece, vec, Pre_rotation, 0);
                //駒のObjectの親を設定
                Piece[x, y].GetComponent<PointPieceScript>().SetParent();
                //駒の大きさ変更
                Piece[x, y].GetComponent<PointPieceScript>().ChangeScale(new Vector3(piecescale, piecethickness, piecescale));
            }
            //OFFLINEなら
            else
            {
                //駒のインスタンス生成
                Piece[x, y] = Instantiate(mainPiece);
                //駒の配置を設定
                Piece[x, y].transform.localPosition = vec;
                //駒のObjectの親を設定
                Piece[x, y].transform.SetParent(canvas.transform, false);
                //駒の回転向きを設定
                Piece[x, y].transform.rotation = Pre_rotation;
                //駒の大きさを設定
                Piece[x, y].transform.localScale = new Vector3(piecescale, piecethickness, piecescale);
            }

            //スタート駒の位置を保存
            mainX = x; //x座標
            mainY = y; //y座標

        }
        //ポイント駒(point!=0)を置くとき
        else
        {
            #region　駒の色(外側)の設定
            Material matrial = null; //マテリアル(色)を初期化

            //マイナスのポイント駒の時
            if (point < 0)
            {
                //マイナスポイント駒用の色を代入
                matrial = unpointColor;
            }
            //プラスのポイント駒の時
            else if (point > 0)
            {
                //プラスポイント駒用の色を代入
                matrial = pointColor;
            }
            #endregion

            //ポイント駒のPrehabが保存されているURL
            Pre_Piece = Const.CO.GameBoardURL + pointPiece.name;
            //ONLINEの時
            if (ONLINE)
            {
                //駒のインスタンス生成
                Piece[x, y] = PhotonNetwork.Instantiate(Pre_Piece, vec, Pre_rotation, 0);
                //駒のObjectの親を設定
                Piece[x, y].GetComponent<PointPieceScript>().SetParent();
                //駒の大きさ変更
                Piece[x, y].GetComponent<PointPieceScript>().ChangeScale(new Vector3(piecescale, piecethickness, piecescale));
                //駒の外側の色を塗る
                PaintColor(Piece[x, y], matrial, Const.CO.PointNumber);
                //駒の内側の色を塗る
                PaintColor(Piece[x, y], PointInside, Const.CO.InsideNumber);
                //Pointの数字を表示するテキストのObjectを取得
                Point[y * Si + x] = Piece[x, y].transform.Find(Const.CO.PointNumberURL).gameObject;
                //駒にPointを示す数字を表示
                Piece[x, y].GetComponent<PointPieceScript>().ChangePoint(point);
            }
            else
            {
                //駒のインスタンス生成
                Piece[x, y] = Instantiate(pointPiece);
                //駒の配置を設定
                Piece[x, y].transform.localPosition = vec;
                //駒のObjectの親を設定
                Piece[x, y].transform.SetParent(canvas.transform, false);
                //駒の回転向きを設定
                Piece[x, y].transform.rotation = Pre_rotation;
                //駒の大きさを設定
                Piece[x, y].transform.localScale = new Vector3(piecescale, piecethickness, piecescale);
                //駒の(外側の)色を変更
                Piece[x, y].GetComponent<Renderer>().material = matrial;
                //駒の内側の色を変更
                Piece[x, y].transform.Find(Const.CO.PointInsideURL).gameObject.GetComponent<Renderer>().material = PointInside;
                //Pointの数字を表示するテキストのObjectを取得
                Point[y * Si + x] = Piece[x, y].transform.Find(Const.CO.PointNumberURL).gameObject;
                //Pointの数字を表示するテキストのObjectの「TextMeshProUGUI」Component取得
                PointText[y * Si + x] = Point[y * Si + x].GetComponent<TextMeshProUGUI>();
                //Pointの数字を表示
                PointText[y * Si + x].text = point.ToString();
            }

        }
    }

    //ゲーム盤を取得
    private void BoardGet()
    {
        #region 変数定義

        //ゲーム盤のPrehabの保存場所のURL
        string Pre_Board = Const.CO.GameBoardURL + gameboard.name;
        //ゲーム番の回転
        Quaternion rotation = new Quaternion(0, 0, 0, 0);
        //ゲーム番の大きさ
        float scale = (float)(Const.CO.MaxScale / Si);
        //ゲーム盤面の厚さ
        float boardthickness = 0.01f;
        //ゲーム番の位置
        Vector3 vec;
        #endregion

        //ゲーム盤を１枚づつ生成
        for (int y = 0; y < Ver; y++)
        {
            for (int x = 0; x < Si; x++)
            {
                #region 盤面の生成位置の誤差を調整
                //盤面の位置の調整用の数値(盤面が偶数なら0.5,奇数なら0)
                float Gosa_Si = 0; //横の誤差
                float Gosa_Ver = 0; //縦の誤差
                //盤面の横の大きさが偶数の時
                if (Si % 2 == 0)
                {
                    Gosa_Si = (float)((Si / 2) - 0.5);
                }
                //盤面の横の大きさが奇数の時
                else
                {
                    Gosa_Si = (float)((Si -1) / 2);
                }
                //盤面の縦の大きさが偶数の時
                if (Ver % 2 == 0)
                {
                    Gosa_Ver = (float)((Ver / 2) - 0.5);
                }
                //盤面の縦の大きさが奇数の時
                else
                {
                    Gosa_Ver = (float)((Ver - 1) / 2);
                }
                #endregion

                //ゲーム盤の位置を設定
                vec = new Vector3((float)((-x + (Gosa_Si)) * scale), (float)((-Gosa_Ver + y) * scale), 0);

                #region　盤面の色の設定
                //ゲーム盤の色を初期化
                Material matrial = null;
                //盤面を市松模様にする
                if ((y + x) % 2 == 0)
                {
                    matrial = board2Color;
                }
                else if ((y + x) % 2 == 1)
                {
                    matrial = board1Color;
                }
                #endregion

                //ONLINEの時
                if (ONLINE)
                {
                    //ゲーム盤のインスタンスを生成
                    Board[y * Si + x] = PhotonNetwork.Instantiate(Pre_Board, vec, rotation, 0);
                    //ゲーム盤のObjectの親を設定
                    Board[y * Si + x].GetComponent<BoardScript>().SetParent();
                    //ゲーム盤のObjectの名前を設定
                    Board[y * Si + x].GetComponent<BoardScript>().ChangeName(Const.CO.BoardName + x + Const.CO.BoardName_Connect + y);
                    //ゲーム盤の大きさを設定
                    Board[y * Si + x].GetComponent<BoardScript>().ChangeScale(new Vector3(scale, scale, boardthickness));
                    //ゲーム盤に色を塗る
                    PaintColor(Board[y * Si + x], matrial, Const.CO.BoardNumber);
                }
                //OFFLINEの時
                else
                {
                    //ゲーム盤のインスタンスを生成
                    Board[y * Si + x] = Instantiate(gameboard);
                    //ゲーム盤の位置を設定
                    Board[y * Si + x].transform.localPosition = vec;
                    //ゲーム盤のObjectの親を設定
                    Board[y * Si + x].transform.SetParent(canvas.transform, false);
                    //ゲーム盤の回転位置を設定
                    Board[y * Si + x].transform.rotation = rotation;
                    //ゲーム盤の大きさを設定
                    Board[y * Si + x].transform.localScale = new Vector3(scale, scale, 0.01f);
                    //ゲーム盤の名前を設定
                    Board[y * Si + x].name = Const.CO.BoardName + x + Const.CO.BoardName_Connect + y;
                    //ゲーム盤の色を塗る
                    Board[y * Si + x].GetComponent<Renderer>().material = matrial;
                }

            }
        }
    }

    //色を塗る関数
    private void PaintColor(GameObject Board, Material boardColor, int type)
    {
        //盤面の色塗りの場合
        if (type == Const.CO.BoardNumber)
        {
            //「BoardScript」のComponentを取得
            BoardScript B_component = Board.GetComponent<BoardScript>();
            //盤面の色を指定された色に変更
            B_component.ChangeColor(boardColor.color);
        }
        //駒(内側と外側)の色塗りの場合
        else if (type == Const.CO.PointNumber || type == Const.CO.InsideNumber)
        {
            //「PointPieceScript」のComponentを取得
            PointPieceScript P_component = Board.GetComponent<PointPieceScript>();
            //駒を指定した色に変更
            P_component.ChangeColor(boardColor.color, type - 1);
        }
    }

    //現在、盤面の中で取れる場所を黄色(色は設定で変更可)に表示
    private void SelectableBoard()
    {
        #region 変数の定義
        int main = 0; //main石の場所
        int order = 0; //移動可能な盤面のマスが配列に何個先に入っているか
        int n = 0; //まとめるのに必要な変数
        int number = 0; //選択可能な場所を見つけた個数
        int Max_number = Si; //選択可能な場所の上限

        //現在の手番が先手(横)の手番の時
        if (currentPlayer == Const.CO.SIDE)
        {
            Max_number = Si; //選択可能な場所の上限
            main = mainY; //スタート駒のy座標を登録
            order = 1; //移動可能な盤面のマスが配列に何個先に入っているか
            n = Si; //main * nするときに、必要(計算に必要な変数)
        }
        //現在の手番が後手(縦)の手番の時
        else if (currentPlayer == Const.CO.VERTICAL)
        {
            Max_number = Ver; //選択可能な場所の上限
            main = mainX; //スタート駒のx座標を登録
            order = Si; //移動可能な盤面のマスが配列に何個先に入っているか
             n = 1; //main * nするときに、必要(計算に必要な変数)
        }
        #endregion

        //盤面を右下から見ていく
        for (int y = 0; y < Ver; y++)
        {
            for (int x = 0; x < Si; x++)
            {
                //選択している盤面のマテリアルを初期化
                Material matrial = null;

                //見ている個所が次に移動できる盤面の時かつ移動できる盤面の個数が上限を越えていない時
                if (y * Si + x == (main * n) + (order * number) && number < Max_number)
                {
                    //色を登録
                    matrial = SelectBoardColor;
                    //移動できる盤面を見つけた数を更新
                    number++;
                }
                //見ている個所が次に移動できる盤面でない時
                else
                {
                    # region 元の色(市松模様)を登録する
                    if ((y + x) % 2 == 0)
                    {
                        matrial = board2Color;
                    }
                    else if ((y + x) % 2 == 1)
                    {
                        matrial = board1Color;
                    }
                    #endregion
                }

                #region 盤面のマスに色を塗る
                //ONLINEの時
                if (ONLINE)
                {
                    //盤面のマスに指定した色を塗る
                    PaintColor(Board[y * Si + x], matrial, Const.CO.BoardNumber);
                }
                //OFFLINEの時
                else
                {
                    //盤面のマスに指定した色を塗る
                    Board[y * Si + x].GetComponent<Renderer>().material = matrial;
                }
                #endregion

            }
        }
    }

    //スコアを表示
    private void ScoreUpdate(int x, int y)
    {
        #region 変数定義
        string Player = ""; //プレイヤー
        int position = 0; //クリックした場所
        int main = 0; //main石の場所
        GameObject[] GetPiece = null; //取った石
        int stoneX = 0; // 取った石の枚数
        int Score = 0; //スコア
        GameObject score_object = null; //取った石の合計得点を記載するテキストを持つObject
        TextMeshProUGUI score_text = null; //取った石の合計得点を記載するテキスト
        string name = "Score"; //得点の前につけるテキスト
        Vector3 vec = new Vector3(530f, 214.5f, -80.0299f); //取った駒を配置する位置
        Quaternion rotation = new Quaternion(0, 0, 0, 0);
        //Vector3 scale = new Vector3(0.04452032f, 100f, 0.01300387f); //取った駒を配置する大きさ
        int row = 3; //この数字ずつ並べる

        //先手の手番の時
        if (currentPlayer == Const.CO.SIDE)
        {
            Player = "A"; //先手(A)の手番
            position = y; //クリックしたy座標
            main = mainY; //スタート駒のy座標
            GetPiece = GetPieceA; //先手が取った駒(の集合)
            stoneX = stoneA; //先手が取った駒の枚数
            Score = ScoreA; //先手の得点
            score_object = scoreA_object; //先手の得点を記載するテキストを持つObject
            score_text = scoreA_text; //先手の得点を記載するテキスト
            //ONLINEの時、名前を取得
            if (ONLINE) name = PhotonNetwork.PlayerList[0].NickName;
        }
        //後手の手番の時
        else if (currentPlayer == Const.CO.VERTICAL)
        {
            Player = "B"; //後手(B)の手番
            position = x; //クリックしたx座標
            main = mainX; //スタート駒のx座標
            GetPiece = GetPieceB; //後手が取った駒(の集合)
            stoneX = stoneB; //後手が取った駒の枚数
            Score = ScoreB; //後手の得点
            score_object = scoreB_object; //後手の得点を記載するテキストを持つObject
            score_text = scoreB_text; //後手の得点を記載するテキスト
            vec = new Vector3(-770f, 214.5f, -80.0299f); //取った駒を配置する位置
            //ONLINEの時、名前を取得
            if (ONLINE) name = PhotonNetwork.PlayerList[1].NickName;
        }

        vec = vec + new Vector3((float)((stoneX % row) * 100), (float)((stoneX / row) * -80), 0);
        #endregion

        #region デバッグ用
        //UnityEngine.Debug.Log("squares["+x+","+y+"]:"+ squares[x, y]);
        //UnityEngine.Debug.Log("Score:" + Score);
        //UnityEngine.Debug.Log("ScoreA:" + ScoreA);
        //UnityEngine.Debug.Log("ScoreB:" + ScoreB);
        //UnityEngine.Debug.Log("main:"+ main+"\nposition:" + position);
        #endregion

        //クリックした(xかy)座標とスタート駒の(xかy)座標が同じ時（取れる駒だった時）
        if (main == position)
        {
            //取った駒を登録
            GetPiece[stoneX] = Piece[x, y];

            #region 変数の更新
            //得点の更新
            Score += squares[x, y];
            //盤面の更新
            squares[x, y] = Const.CO.EMPTY;

            //取った駒の位置の更新
            Piece[mainX, mainY].GetComponent<PointPieceScript>().ChangePosition(GetPiece[stoneX].transform.localPosition, true);
            //MainStoneの位置更新
            Piece[x, y] = Piece[mainX, mainY];

            //Squaresの値を更新(スタート駒の場所の変更と元のスタート駒の位置を空白にする)
            squares[mainX, mainY] = Const.CO.EMPTY; //元のスタート駒の位置を空白にする
            squares[x, y] = Const.CO.MAIN; //スタート駒の位置を更新
            mainX = x; //スタート駒のx座標を更新
            mainY = y; //スタート駒のy座標を更新
            //取った駒の位置を更新
            GetPiece[stoneX].GetComponent<PointPieceScript>().ChangeGetPosition(Player, stoneX,vec);
            //GetPiece[stoneX].transform.rotation = rotation;
            //GetPiece[stoneX].transform.localScale = GetPiece[stoneX].transform.Scale;
            stoneX++; //取った駒の枚数の更新
            //得点のテキストの更新
            score_object.GetComponent<TextScript>().ChangeText(name + ":" + Score);
            #endregion

            #region 効果音を出力
            //ONLINEの時
            if (ONLINE)
            {
                //駒の移動音を流す
                Piece[x, y].GetComponent<PointPieceScript>().SE(MovingSound);
            }
            //OFFLINEの時
            else
            {
                //駒の移動音を流す
                VCscript.PlaySound(MovingSound);
            }
            #endregion

            #region 変更した値を元の場所(変数)に代入
            //先手の手番の時
            if (currentPlayer == Const.CO.SIDE)
            {
                GetPieceA = GetPiece; //更新している、先手の取った駒(の集合)を代入
                stoneA = stoneX; //更新している、先手が取った駒の数を代入
                ScoreA = Score; //更新している、先手の得点を代入
                scoreA_text = score_text; //更新している、先手の得点を表示するテキストを代入
                //手番を表示するテキストを更新
                Turn.GetComponent<TextScript>().ChangeText(Const.CO.SecondTrunText);
                //テキストを右詰めで表示
                Turn.GetComponent<TextScript>().ChangeAlignment(Const.CO.Right);
                //テキストを後手のイメージ色で表示
                Turn.GetComponent<TextScript>().ChangeColor(SecondColor);
                //手番を後手(2)に渡す(Board[0]の「BoardScript」ComponentのOwnershipで判断)
                Board[0].GetComponent<BoardScript>().ChangeOwnership(2);

            }
            //後手の手番の時
            else if (currentPlayer == Const.CO.VERTICAL)
            {
                GetPieceB = GetPiece; //更新している、後手の取った駒(の集合)を代入
                stoneB = stoneX; //更新している、後手が取った駒の数を代入
                ScoreB = Score; //更新している、後手の得点を代入
                scoreB_text = score_text; //更新している、後手の得点を表示するテキストを代入
                //手番を表示するテキストを更新
                Turn.GetComponent<TextScript>().ChangeText(Const.CO.FirstTrunText);
                //テキストを右詰めで表示
                Turn.GetComponent<TextScript>().ChangeAlignment(Const.CO.Right);
                //テキストを先手のイメージ色で表示
                Turn.GetComponent<TextScript>().ChangeColor(FirstColor);
                //手番を先手(1)に渡す(Board[0]の「BoardScript」ComponentのOwnershipで判断)
                Board[0].GetComponent<BoardScript>().ChangeOwnership(1);

            }
            #endregion

            #region ゲームが終わっているか確認
            //ゲームが終わっている時
            if (CheckFinished())
            {
                //UnityEngine.Debug.Log("FINISH");
                Result();　//終了処理
            }
            //ゲームが終わっていない時
            else
            {
                //UnityEngine.Debug.Log("CONTINUE");
            }
            #endregion

            //Playerを交代
            currentPlayer = -currentPlayer;
            if(ONLINE)  photonView.RPC(nameof(RpcCurrentPlayer), RpcTarget.All, currentPlayer);
            //移動可能なマスを表示
            SelectableBoard();
        }


    }

    //ゲームが終了しているか判定する
    private bool CheckFinished()
    {
        //先手の手番(が終わった)時
        if (currentPlayer == Const.CO.SIDE)
        {
            //縦を確認
            for (int i = 0; i < Ver; i++)
            {
                //スタート駒と同じ縦位置にポイント駒がある時
                if (squares[mainX, i] != Const.CO.EMPTY && squares[mainX, i] != Const.CO.MAIN)
                {
                    //ゲームは終了しない
                    return false;
                }
            }
        }
        //後手の手番(が終わった)時
        else if (currentPlayer == Const.CO.VERTICAL)
        {
            //横を確認
            for (int i = 0; i < Si; i++)
            {
                //スタート駒と同じ横位置にポイント駒がある時
                if (squares[i, mainY] != Const.CO.EMPTY && squares[i, mainY] != Const.CO.MAIN)
                {
                    //ゲームは終了しない
                    return false;
                }
            }
        }
        //ゲーム終了
        return true;
    }

    //結果発表
    private void Result()
    {
        finish = true;//ゲーム終了
        string text = "";
        //ゲーム終了したときにでる画面の表示
        FinishCanvas();

        #region 手番表示テキストの変更
        //終了のテキスト
        Turn.GetComponent<TextScript>().ChangeText(Const.CO.Finish);
        //テキストを右詰めにする
        Turn.GetComponent<TextScript>().ChangeAlignment(Const.CO.Right);
        //テキストをデフォルト色にする
        Turn.GetComponent<TextScript>().ChangeColor(DefaltColor);
        #endregion

        #region ボタンの変数
        //タイトルに戻るボタンの変数
        //ボタンの位置
        Vector3 position = new Vector3(-380, -300, -canvas.transform.position.z - 1);
        //ボタンの大きさ
        Vector3 scale = new Vector3(10f, 10f, 10f);
        //ボタンの回転位置
        Quaternion rotation = new Quaternion(0, 0, 0, 0);
        //ボタン内部のテキスト
        string text2 = "Title";
        //ボタンのサイズ
        float Width2 = 50.95551f;
        float Height2 = 17.46552f;

        //リトライボタンの変数
        //ボタンの位置
        Vector3 position3 = new Vector3(380, -300, -canvas.transform.position.z - 1);
        //ボタンの大きさ
        Vector3 scale3 = new Vector3(10f, 10f, 10f);
        //ボタンの回転位置
        Quaternion rotation3 = new Quaternion(0, 0, 0, 0);
        //ボタン内部のテキスト
        string text3 = "Retry";
        //ボタンのサイズ
        float Width3 = 50.95551f;
        float Height3 = 17.46552f;
        #endregion

        #region ボタンの生成
        //ONLINeの時
        if (ONLINE)
        {
            
            GameObject button2 = PhotonNetwork.Instantiate(Const.CO.GameBoardURL + title_button.name,position,rotation,0);//タイトルに戻るボタンのインスタンス生成
            button2.GetComponent<ButtonScript>().SetParent(Panel.name);//ボタンObjectの親を設定
            button2.GetComponent<ButtonScript>().ChangeScale(scale);//ボタンの大きさを変更
            //button2.GetComponent<ButtonScript>().ChangeText(Const.CO.NomalButtonURL + button.name + "/Text(TMP)", text2);
            //button2.GetComponent<ButtonScript>().ChangeSize(Width2, Height2);
            //button2.GetComponent<ButtonScript>().SetAction(btn,"LobbyButton");


            //リトライボタンのインスタンス生成
            //GameObject button3 = PhotonNetwork.Instantiate(GameBoardURL + Retry_button.name, position3, rotation3, 0);
            //ボタンObjectの親を設定
            //button3.GetComponent<ButtonScript>().SetParent(Panel.name);
            //ボタンの大きさを変更
            //button3.GetComponent<ButtonScript>().ChangeScale(scale3);
        }
        //OFFLINEの時
        else
        {

            GameObject button2 = Instantiate(button);//タイトルに戻るボタンのインスタンス生成
            button2.transform.SetParent(Panel.transform, false);//ボタンObjectの親を設定
            button2.transform.localPosition = position;//ボタンの位置を変更
            button2.transform.localScale = scale;//ボタンの大きさを変更
            TextMeshProUGUI button2text = button2.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();//ボタン内部のテキスト
            button2text.text = text2;//ボタン内部のテキストを変更
            var rtf2 = button2.GetComponent<RectTransform>();//ボタンのサイズ
            rtf2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width2);// 横方向のサイズ
            rtf2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height2);// 縦方向のサイズ
            button2.GetComponent<Button>().onClick.AddListener(btn.TitleButton);//ボタンのアクションを設定



            GameObject button3 = Instantiate(button);//リトライボタンのインスタンス生成
            button3.transform.SetParent(Panel.transform, false);//ボタンObjectの親を設定
            button3.transform.localPosition = position3;//ボタンの位置を変更
            button3.transform.localScale = scale3;//ボタンの大きさを変更
            TextMeshProUGUI button3text = button3.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();//ボタン内部のテキスト
            button3text.text = text3;//ボタン内部のテキストを変更
            var rtf3 = button3.GetComponent<RectTransform>();//ボタンのサイズ
            rtf3.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width3);// 横方向のサイズ
            rtf3.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height3);// 縦方向のサイズ
            button3.GetComponent<Button>().onClick.AddListener(btn.GameStartButton);//ボタンのアクションを設定

        }
        #endregion

        #region ゲーム結果の表示
        //ゲーム結果を表示するテキスト
        if (TimeUp == 0)
        {
            text = "  " + ScoreB + "  -  " + ScoreA;

        }else if(TimeUp == 1)
        {
            text = "先手：TIME UP";
        }else if (TimeUp == 2)
        {
            text = "後手：TIME UP";
        }

        Result_score.GetComponent<TextScript>().ChangeText(text);
        //後手の得点が先手の得点より大きいとき
        if(TimeUp == 1)
        {
            //勝利テキストを表示
            WIN.GetComponent<TextScript>().ChangeText("後手"+Const.CO.WINText);
            //テキストを中詰めで表示
            WIN.GetComponent<TextScript>().ChangeAlignment(Const.CO.Center);
        }
        else if (TimeUp == 2)
        {
            //勝利テキストを表示
            WIN.GetComponent<TextScript>().ChangeText("先手" + Const.CO.WINText);
            //テキストを中詰めで表示
            WIN.GetComponent<TextScript>().ChangeAlignment(Const.CO.Center);
        }
        else if (ScoreB > ScoreA)
        {
            //勝利テキストを表示
            WIN.GetComponent<TextScript>().ChangeText(Const.CO.WINText);
            //テキストを左詰めで表示
            WIN.GetComponent<TextScript>().ChangeAlignment(Const.CO.Left);
        }
        //後手の得点が先手の得点より少ないとき
        else if (TimeUp == 0 && ScoreB < ScoreA)
        {
            //勝利テキストを表示
            WIN.GetComponent<TextScript>().ChangeText(Const.CO.WINText);
            //テキストを右詰めで表示
            WIN.GetComponent<TextScript>().ChangeAlignment(Const.CO.Right);
        }
        //同点の時
        else
        {
            //引き分けテキストを表示
            WIN.GetComponent<TextScript>().ChangeText(Const.CO.DRAW);
            //テキストを中央詰めで表示
            WIN.GetComponent<TextScript>().ChangeAlignment(Const.CO.Center);
        }

        //勝利のテキストの色に変更
        WIN.GetComponent<TextScript>().ChangeColor(WINColor);
        #endregion

    }

    //スコアが変更（上がった）時に呼び出す関数
    private void ScoreUP(int x, int y)
    {
        /*
        if(currentPlayer == SIDE)
        {
            time_A = time_A - seconds;
            timeA_text.text = "TIME:"+(time_A / 60f )+":"+(time_A % 60f);
            seconds = 0;
        }else if(currentPlayer == VERTICAL)
        {
            time_B = time_B - seconds;
            timeB_text.text = "TIME:" + (time_B / 60f) + ":" + (time_B % 60f);
            seconds = 0;
        }
        */

        //自分が手番かどうか確認
        CheckTurnPlayer();

        #region スコア更新
        //ONLINEの時
        if (ONLINE)
        {
            //自分がHostかつ手番の場合
            if (PhotonNetwork.IsMasterClient && TurnPlayer)
            {
                //マスにポイント駒がある時
                if (squares[x, y] != Const.CO.EMPTY && squares[x, y] != Const.CO.MAIN)
                {
                    //スコア更新
                    ScoreUpdate(x, y);
                }
            }
            //自分がHost出ない時
            else if (!PhotonNetwork.IsMasterClient)
            {
                //Hostにデータを送信し、Host側でスコア更新処理
                photonView.RPC(nameof(RpcScoreUP), RpcTarget.MasterClient, x, y);
            }
        }
        //OFFLINEの時
        else
        {
            //マスにポイント駒がある時
            if (squares[x, y] != Const.CO.EMPTY && squares[x, y] != Const.CO.MAIN)
            {
                //スコア更新
                ScoreUpdate(x, y);
            }
        }
        #endregion

    }

    //自分の手番か確認
    private void CheckTurnPlayer()
    {
        //ONLINEの時
        if (ONLINE)
        {
            //UnityEngine.Debug.Log("Check");
            photonView.RPC(nameof(RpcCheckTurnPlayer), RpcTarget.MasterClient);
        }
    }

    //ゲームが終了した時の画面を表示
    private void FinishCanvas()
    {
        //ONLINEの時
        if (ONLINE)
        {
            photonView.RPC(nameof(RpcFinishCanvas), RpcTarget.All);
        }
        else
        {
            //サブ画面を表示
            CanvasDisplay(1);
        }

    }

    #endregion

    #region Public関数
    // Photonサーバ切断処理
    public void DisConnectPhoton()
    {
        PhotonNetwork.Disconnect();
    }
    #endregion

    #region PunRPC Private 関数
    [PunRPC] //ScoreUPから呼び出される関数
    private void RpcScoreUP(int x, int y)
    {
        //マスにポイント駒がある時
        if (squares[x, y] != Const.CO.EMPTY && squares[x, y] != Const.CO.MAIN)
        {
            //自分の手番でない時
            if (!TurnPlayer) ScoreUpdate(x, y);
        }
    }

    [PunRPC] //CheckTurnPlayerから呼び出される関数
    private void RpcCheckTurnPlayer()
    {
        //自分の手番の時
        if (Board[0].GetComponent<PhotonView>().IsMine)
        {
            TurnPlayer = true;
        }
        //自分の手番でない時
        else
        {
            TurnPlayer = false;
        }
    }

    [PunRPC] //FinishCanvasから呼び出される関数
    private void RpcFinishCanvas()
    {
        //サブ画面を表示
        CanvasDisplay(1);
    }
    [PunRPC] //FinishCanvasから呼び出される関数
    private void RpcCurrentPlayer(int turn)
    {
        currentPlayer = turn;
    }
    [PunRPC] //残り時間を同期させるための関数
    private void RpcTimeUpdate(int player, string time,int timecount)
    {

        if (player == Const.CO.SIDE)
        {
            time_A = timecount;
            timeA_text.text = time;
        }else if(player == Const.CO.VERTICAL)
        {
            time_B = timecount;
            timeB_text.text = time;
        }
    }

    #endregion
}
