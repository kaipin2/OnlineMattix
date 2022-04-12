using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UIを使用するのに必要
using TMPro; //TextMeshProを使用するのに必要
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
namespace Const
{
    public static class CO
    {
        //主にボタンで使用する変数
        public static string AudioObjectName = "Audio"; //AudioManagerがComponentされているGameobjectの名称
        public static string MainObjectName = "TitleController"; //Title画面のTitleContoroller.csが格納されているObjectの名称
        public static string LobbyObjectName = "LobbyController"; //Lobby画面のLobbyContoroller.csが格納されているObjectの名称
        public static string PhotonManagerObjectName = "PhotonManager"; //Photon画面のPhotonManager.csが格納されているObjectの名称
        public static string GameControllerObjectName = "GameController"; //マティックス画面のGameController.csが格納されているObjectの名称
        public static string ExplanationControllerObjectName = "ExplanationController"; //説明画面のExplanationControllerController.csが格納されているObjectの名称

        public static string DisconnectName = "Disconnect"; //切断ボタンの名前
        public static string GameStartName = "GameStart"; //ゲーム開始ボタン
        public static string OFFLINEButtonName = "OFFLINEButton"; //OFFLINEButtonの名前
        public static string ONLINEButtonName = "ONLINEButton"; //ONLINEButtonの名前
        public static string PlayerNameFieldName = "PlayerNameField"; //PlayerNameの名前
        public static string RoomNameFieldName = "RoomNameField"; //RoomNameの名前
        public static string EntertheRoomName = "EntertheRoom"; //EntertheRoomの名前
        public static string UpdateButtonName = "UpdateButton"; //UPdateButtonの名前
        public static string RoomNameName = "RoomName"; //制作されている部屋への入室ボタンのobject名
        public static string TitleSceneName = "Title"; //TitleSceneの名前
        public static string MattixSceneName = "マティックス"; //MainSceneの名前
        public static string LobbySceneName = "Lobby"; //LobbySceneの名前
        public static string ExplanationSceneName = "Explanation"; //ExplanationSceneの名前
        public static string ConfirmationWindowsName = "ConfirmationWindows"; //ConfirmationWindowsの名前

        //主にTitleControllerで使用する変数
        public static string CanvasName = "Canvas"; //Title画面のCanvasの名称
        public static string AudioCanvasName = "VolumeCanvas"; //OptionのCanvasの名称
        public static string VersionTextName = "Version";
        public static string TitleName = "OptionTitle"; //TitleのTextがあるTextMeshProObjectの名称

        //主にAudioManagerで使用する変数
        public static string BGMFolder = "Audio/BGM"; //BGMが入っているフォルダ
        public static string SEFolder = "Audio/SE"; //SEが入っているフォルダ

        //主にSoundPresenterで使用する変数
        public static string bgmNamePrefKey = "BGMVolume";
        public static string seNamePrefKey = "SEVolume";
        public static string Option = "VolumeCanvas/Option";
        public static string BGM = "/BGM";
        public static string SE = "/SE";
        public static string Volume = "/Volume";

        //主にLobbyControllerで使用する変数
        public static string PhotonCanvasName = "PhotonCanvas"; //Photon画面のCanvasの名称

        //主にGameControllerで使用する変数
        //gameBoardで使うPrefabが格納されているURL
        public static string GameBoardURL = "Prehabs/GameBoard/";
        //NomalButtonのPrefabが格納されているURL
        public static string NomalButtonURL = "Prehabs/";
        public static string NomalButtonURL2 = "Prehabs/NomalButton";
        //カメラの名称
        public static string MainCamera = "Main Camera";
        //取った駒を置く位置を示すObjectの名称(A:先手,B:後手)
        public static string FirstGetPositionName = "CubeA";
        public static string SecondGetPositionName = "CubeB";
        //先手が手番の時に示すテキスト表示
        public static string FirstTrunText = "先攻(赤)の番です";
        public static string FirstPlayerText = "あなたは先手です";
        //後手が手番の時に示すテキスト表示
        public static string SecondTrunText = "後攻(黄)の番です";
        public static string SecondPlayerText = "あなたは後手です";
        //練習モードの時のテキスト
        public static string OnePlayerText = "一人用モード";
        //ゲームが終了したときの(手番を表示する)テキスト
        public static string Finish = "ゲーム終了";
        //勝利したときのテキスト
        public static string WINText = "WIN";
        //引き分けの時のテキスト
        public static string DRAW = "DRAW";
        //テキストを右寄せするとき(ChangeAlignment()の入力)
        public static string Right = "Right";
        //テキストを左寄せするとき(ChangeAlignment()の入力)
        public static string Left = "Left";
        //テキストを中央寄せするとき(ChangeAlignment()の入力)
        public static string Center = "Center";
        //駒の内側のObjectのURL
        public static string PointInsideURL = "inside";
        //ポイント駒のポイント数を表示するテキストのObjectのURL
        public static string PointNumberURL = "inside/Point";
        //色を塗る場所(0:盤面の色塗り,1:駒の外側の色塗り,2:駒の内側の色塗り)
        public static int BoardNumber = 0;
        public static int PointNumber = 1;
        public static int InsideNumber = 2;
        //ゲーム番の大きさ
        public static int Ver = 6; //縦
        public static int Si = 6; //横
        //駒の点数の上限と下限
        public static int Max = 10; //上限
        public static int Min = -10; //下限
        //EMPTY=0,MAIN=30で定義
        public static int EMPTY = 0;
        public static int MAIN = Max * 3;
        //手番のプレイヤーを示す(SIDE:先手,VERTICAL:後手)
        public static int VERTICAL = 2;
        public static int SIDE = -2;
        //ゲーム番の名前( (BoardName) + (x座標) + (BoardName_Connect) + (y座標) となる)
        public static string BoardName = "Cube";
        public static string BoardName_Connect = "_";

        //主にPhotonManagerで使用する変数
        public static int button_Ver = 3; //ボタンの並び(縦の個数)
        public static int button_Side = 3; //ボタンの並び(横の個数)
        public static int Number = button_Ver * button_Side; //ルームの最大制作可能個数
        public static string RoomButtonParent = "PhotonCanvas/Room";//ルームボタンの位置のURL
        public static string StatusTextName = "Title";
        public static string MessageTextName = "Message";
        public static string LobbyTitle = "ロビー";
        public static string GameVersion = "Ver 1.1.0"; //ゲームバージョン指定（設定しないと警告が出る）

    }
}
