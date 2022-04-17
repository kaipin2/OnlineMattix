//Buttonのクラス
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UIを使用するのに必要
using TMPro; //TextMeshProを使用するのに必要
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
//using UnityEditor;

using Const; //定数を定義している

public class Buttons : MonoBehaviour
{
    #region Private変数定義
    /*
     * 表示する画面を選択する変数
     * Enabled()関数で使用
     */
    private static int OptionNumber = 0;
    /*
     * OFFLINEで対戦するかどうか選択する変数
     */
    private static bool OFFLINE = false;
    #endregion

    #region Public変数定義
    public AudioManager VCscript = null; //「AudioManager」のComponentを格納する
    public TitleController TC_Script = null; //「TitleContoroller」のComponentを格納する
    public LobbyController LC_Script = null; //「LobbyContoroller」のComponentを格納する
    public PhotonManager PM_Script = null; //「PhotonManager」のComponentを格納する
    public GameController GC_Script = null; //「GameController」のComponentを格納する
    public ExplanationController EC_Script = null; //「ExplanationController」のComponentを格納する
    public OptionStatusController PW_Script = null; //「PopupWindow」のComponentを格納する

    public GameObject MainObject = null; //「TitleContoroller」を格納するObject
    public GameObject LobbyObject = null; //「LobbyContoroller」を格納するObject
    public GameObject PhotonManagerObject = null; //「PhotonManager」を格納するObject
    public GameObject GameControllerObject = null; //「GameController」を格納するObject
    public GameObject ExplanationControllerObject = null; //「ExplanationController」を格納するObject
    public GameObject ConfirmationWindowsObject = null; //「ConfirmationWindows」を格納するObject
    public GameObject OptionStatusControllerObject = null; //「OptionStatusController」を格納するObject

    public List<GameObject> ButtonSetObject = new List<GameObject>();//ButtonのComponentが格納されているObjectを格納する集合
    public List<Button> ButtonSet = new List<Button>();//ButtonのComponent集合

    
    #endregion

    // はじめに呼び出される
    public virtual void Start()
    {


    }

    //ボタンをクリックしたときに呼び出される(規定値)
    public virtual void OnClick()
    {
        //「AudioManager」のComponentが作られていないなら制作
        if (VCscript == null)
        {
            VCscript = GameObject.Find(Const.CO.AudioObjectName).GetComponent<AudioManager>();
        }
        //ボタンのクリック音(規定値)
        VCscript.PlaySound(0);
    }

    //ボタンをクリックしたときに呼び出される(音変化)
    protected void OnClick(int ButtonSound)
    {
        //「AudioManager」のComponentが作られていないなら制作
        if (VCscript == null)
        {
            VCscript = GameObject.Find(Const.CO.AudioObjectName).GetComponent<AudioManager>();
        }
        //ボタンのクリック音
        VCscript.PlaySound(ButtonSound);
    }

    #region 他のObjectについているScriptを参照できるようにする関数
    //TC_Scriptを生成
    public void TC_ScriptCreate()
    {
        if(MainObject == null)
        {
            MainObject = GameObject.Find(Const.CO.MainObjectName).gameObject;
        }
        if(TC_Script == null)
        {
            TC_Script = MainObject.GetComponent<TitleController>();
        }

    }

    //LC_Scriptを生成
    public void LC_ScriptCreate()
    {
        if (LobbyObject == null)
        {
            LobbyObject = GameObject.Find(Const.CO.LobbyObjectName).gameObject;
        }
        if (LC_Script == null)
        {
            LC_Script = LobbyObject.GetComponent<LobbyController>();
        }

    }

    //PM_Scriptを生成
    public void PM_ScriptCreate()
    {
        if (GameObject.Find(Const.CO.PhotonManagerObjectName) != null)
        {
            if (PhotonManagerObject == null)
            {
                PhotonManagerObject = GameObject.Find(Const.CO.PhotonManagerObjectName).gameObject;
            }
        }
        else
        {
            return;
        }
        if (PM_Script == null)
        {
            PM_Script = PhotonManagerObject.GetComponent<PhotonManager>();
        }
    }

    //GC_Scriptを作成
    public void GC_ScriptCreate()
    {
        if (GameControllerObject == null)
        {
            if(GameObject.Find(Const.CO.GameControllerObjectName) != null)
            {
                GameControllerObject = GameObject.Find(Const.CO.GameControllerObjectName).gameObject;
            }
            else
            {
                return;
            }
        }
        if (GC_Script == null)
        {
            GC_Script = GameControllerObject.GetComponent<GameController>();
        }
    }

    //ExplanationContorollerのComponentを制作
    public void EC_ScriptCreate()
    {
        if (ExplanationControllerObject == null)
        {
            if (GameObject.Find(Const.CO.ExplanationControllerObjectName) != null)
            {
                ExplanationControllerObject = GameObject.Find(Const.CO.ExplanationControllerObjectName).gameObject;
            }
            else
            {
                return;
            }
        }
        if (EC_Script == null)
        {
            EC_Script = ExplanationControllerObject.GetComponent<ExplanationController>();
        }
    }

    //PopupWindowのComponentを作成
    public void PW_ScriptCreate()
    {
        if (OptionStatusControllerObject == null)
        {
            if (GameObject.Find(Const.CO.AudioCanvasName) != null)
            {
                OptionStatusControllerObject = GameObject.Find(Const.CO.AudioCanvasName).gameObject;
            }
            else
            {
                return;
            }
        }
        if (PW_Script == null)
        {
            PW_Script = OptionStatusControllerObject.GetComponent<OptionStatusController>();
        }
    }
    #endregion

    #region ボタンが押されたときの関数(protected関数)
    public void BackButton()
    {
        OptionNumber = 1;
        if (ContainsScene(Const.CO.TitleSceneName))
        {
            TC_ScriptCreate();//TitleContorollerのスクリプト制作
        }
        else if (ContainsScene(Const.CO.LobbySceneName))
        {
            PM_ScriptCreate();//PhotonManagerのComponentを制作
        }
        else if (ContainsScene(Const.CO.MattixSceneName))
        {
            GC_ScriptCreate();//GameControllerのComponentを制作
        }


        OnClick(); //ボタンを押したときの音を鳴らす
        Enabled(OptionNumber);
    }
    public void RoomEnterButton(string text)
    {
        

        PM_ScriptCreate();//PhotonManagerのComponentを制作
        OnClick(); //ボタンを押したときの音を鳴らす

        PM_Script.ConnectToRoom(text);
    }
    public void OptionButton()
    {
        OptionNumber = 0;
        OnClick(); //ボタンを押したときの音を鳴らす
        Enabled(OptionNumber);
    }
    public void OnLineButton()
    {
        OnClick();
        OFFLINE = false; //ONLINEでゲームを開始
        string[] str = { Const.CO.ONLINEButtonName };
        ButtonEnabled(str); //ボタンを非表示にする
        ButtonActive(Const.CO.DisconnectName); //切断ボタンを表示
        ButtonActive(Const.CO.GameStartName); //ゲーム開始ボタンを表示
        ButtonActive(Const.CO.PlayerNameFieldName); //PlayerNameInFieldを表示
        ButtonActive(Const.CO.RoomNameFieldName); //RoomNameInFieldを表示
        ButtonActive(Const.CO.EntertheRoomName); //入室ボタンを表示
        ButtonActive(Const.CO.UpdateButtonName); //Updateボタンを表示
        PhotonConent(OFFLINE);
    }
    public void GameStartButton()
    {
        OnClick(); //クリック音
        if (SceneManager.GetActiveScene().name == Const.CO.LobbySceneName)
        {
            PM_ScriptCreate();//PM_ScriptCreateのComponentを制作
            PM_Script.GameStart(); //ゲーム画面に移動する
        }
        else if (SceneManager.GetActiveScene().name == Const.CO.MattixSceneName)
        {
            SceneManager.LoadScene(Const.CO.MattixSceneName);
        }
    }
    public void HowToPlayButton()
    {
        OnClick(); //ボタンを押したときの音を鳴らす
        SceneManager.LoadScene(Const.CO.ExplanationSceneName);
    }
    public void LobbyButton()
    {
        OnClick(); //ボタンを押したときの音を鳴らす
        SceneManager.LoadScene(Const.CO.LobbySceneName);
    }
    public void OffLineBuuton()
    {
        OFFLINE = true;//OFFLINEでゲーム行う
        if (SceneManager.GetActiveScene().name == Const.CO.LobbySceneName)
        {
            string[] str = { Const.CO.ONLINEButtonName, Const.CO.OFFLINEButtonName };
            OnClick();//ボタンを押したときの音を鳴らす
            ButtonEnabled(str); //ボタンを非表示にする
            ButtonActive(Const.CO.DisconnectName); //切断ボタンを表示
        }
        PhotonConent(OFFLINE);
    }
    public void GameFinishButton()
    {
        OptionNumber = 1;
        if (ContainsScene(Const.CO.MattixSceneName))
        {
            OnClick();//ボタンを押したときの音を鳴らす
            GC_ScriptCreate();
            if (!GC_Script.ON_LINE)
            {
                Enabled(OptionNumber);
                SceneManager.LoadScene(Const.CO.TitleSceneName);
            }
            else
            {
                Enabled(OptionNumber);
                SceneManager.LoadScene(Const.CO.LobbySceneName);
            }
        }
    }
    public void TitleButton()
    {
        PM_ScriptCreate();//PM_ScriptCreateのComponentを制作
        OnClick(); //ボタンを押したときの音を鳴らす

        //ネットに繋がっていたら切断する
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            DisConnectPhoton();
        }
        else
        {
            SceneManager.LoadScene(Const.CO.TitleSceneName);
        }
        
    }
    public void DisconnectButton()
    {
        OnClick();
        PM_ScriptCreate();//PhotonManagerのComponentを制作

        if (PM_Script.dispStatus == "ONLINE")
        {
            PM_Script.Disconnect();
        }

    }
    public void ExitButton()
    {
        OnClick();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                UnityEngine.Application.Quit();
        #endif
    }
    public void PageBackButton()
    {
        OnClick(); //ボタンを押したときの音を鳴らす

        EC_ScriptCreate();//EC_ScriptCreateのComponentを制作
        EC_Script.PageBack();
    }
    public void RoomCreateButton()
    {
        GameObject RoomTextObject;
        TMP_InputField RoomNameText;
        OnClick(); //ボタンを押したときの音を鳴らす
        PM_ScriptCreate();//PhotonManagerのComponentを制作
        if (PhotonNetwork.IsConnected)
        {
            RoomTextObject = GameObject.Find(Const.CO.RoomNameFieldName).gameObject;
            RoomNameText = RoomTextObject.GetComponent<TMP_InputField>();
            if (RoomNameText.text == "")
            {
                PM_Script.SetMessage("ルーム名を入力してください");
            }
            else if(PM_Script.Room == false)
            {
                PM_Script.RoomCreate(RoomNameText.text);
            }
        }
        else
        {
            PM_Script.SetMessage("ONLINE状態でないため、ルームを制作することができません");
        }
    }
    public void PageFeedButton()
    {
        OnClick(); //ボタンを押したときの音を鳴らす
        EC_ScriptCreate();//EC_ScriptCreateのComponentを制作
        EC_Script.PageNest();
    }
    public void UpdataButton()
    {
        OnClick(); //ボタンを押したときの音を鳴らす
        PM_ScriptCreate();//PhotonManagerのComponentを制作
        PM_Script.UpdataButtonOnClick();
    }
    public void RetireButton()
    {
        OnClick(); //ボタンを押したときの音を鳴らす
        OptionNumber = 1;
        Enabled(OptionNumber);
        GC_ScriptCreate();
        GC_Script.DisConnectPhoton();
    }
    #endregion
    //画面を表示、非表示を変更する(Title ⇔ Option ⇔ Photon)
    public void Enabled(int Change)
    {
        Canvas Pre_AudioCanvas = null;
        Canvas Pre_TitleCanvas = null;
        Canvas Pre_PhotonCanvas = null;

        if (ContainsScene(Const.CO.TitleSceneName))
        {
            TC_ScriptCreate();//TitleContorollerのComponentを制作
            Pre_AudioCanvas = TC_Script.AudioCanvas;
            Pre_TitleCanvas = TC_Script.TitleCanvas;
            
        }
        else if (ContainsScene(Const.CO.LobbySceneName))
        {
            LC_ScriptCreate();//LobbyContorollerのComponentを制作
            Pre_AudioCanvas = LC_Script.AudioCanvas;
            Pre_TitleCanvas = LC_Script.PhotonCanvas;
        }
        else if (ContainsScene(Const.CO.MattixSceneName))
        {
            GC_ScriptCreate();//GameContorollerのComponentを制作
            Pre_AudioCanvas = GC_Script.AudioCanvas;
            Pre_TitleCanvas = GC_Script.MainCanvas;
            Pre_PhotonCanvas = GC_Script.SubCanvas;
        }
        else if (ContainsScene(Const.CO.ExplanationSceneName))
        {
            EC_ScriptCreate();//ExplanationContorollerのComponentを制作
            Pre_AudioCanvas = EC_Script.AudioCanvas;
            Pre_TitleCanvas = EC_Script.MainCanvas;
        }

        //TextMeshProUGUI TitleText = TC_Script.TitleTextMeshPro;
        //TextMeshProUGUI OptionText = TC_Script.OptionTextMeshPro;
        switch (Change)
        {

            case 0://Optionを開く
                Pre_AudioCanvas.enabled = true;//Optionのキャンバスの表示を表示にする
                Pre_TitleCanvas.enabled = false; //Mainのキャンバスの表示を非表示にする
                if (ContainsScene(Const.CO.MattixSceneName))
                {
                    Pre_PhotonCanvas.enabled = false; //Photonのキャンバスの表示を非表示にする
                }
                break;
            case 1://Title or Game　or Lobbyを開く
                Pre_AudioCanvas.enabled = false;//Optionのキャンバスの表示を非表示にする
                Pre_TitleCanvas.enabled = true; //Mainのキャンバスの表示を表示にする
                if (ContainsScene(Const.CO.MattixSceneName))
                {
                    Pre_PhotonCanvas.enabled = false; //Photonのキャンバスの表示を非表示にする
                }
                break;
            case 2://Photon or Sub(ゲーム結果)を開く
                Pre_AudioCanvas.enabled = false;//Optionのキャンバスの表示を非表示にする
                Pre_TitleCanvas.enabled = false; //Mainのキャンバスの表示を非表示にする
                if (ContainsScene(Const.CO.MattixSceneName))
                {
                    Pre_PhotonCanvas.enabled = true; //Photonのキャンバスの表示を表示にする
                }
                break;
            default:
                break;
        }
       
    }

    // Buttonが存在するか調べる
    public int ButtonSearch(string str)
    {

        GameObject obj = GameObject.Find(str).gameObject;
        if(obj != null && ButtonSetObject.IndexOf(obj) == -1)
        {
            ButtonSetObject.Add(obj);
            ButtonSet.Add(obj.GetComponent<Button>());
            return ButtonSetObject.IndexOf(obj);
        }
        return ButtonSetObject.IndexOf(obj);
    }

    //Buttonの非表示を行う
    public void ButtonEnabled(string[] str)
    {
        string[] ButtonName = str;
        foreach (string s in ButtonName)
        {
            int number = ButtonSearch(s);
            if (number != -1)
            {
                ButtonSetObject[number].SetActive(false);
            }
        }
    }

    //ButtonをActive(表示)にする
    public void ButtonActive(string str)
    {
        int limit = 2; //どこまで親obujectをさかのぼるか

        GameObject Gob = this.gameObject.transform.parent.gameObject;
        for (int i = 0;i < limit;i++)
        {
            //CanvasのComponentをもったObjectを探す
            if (Gob.GetComponent<Canvas>() != null)
            {
                //表示するボタンはCanvasの子供である必要がある(子供の子供は不可)
                Gob = Gob.transform.Find(str).gameObject;
                break;
            }
            else
            {
                Gob = Gob.gameObject.transform.parent.gameObject;
            }
        }
        Gob.SetActive(true);
        ButtonSearch(Gob.name);
    }

    //OFFLINEかONLINEでNetWorkに接続
    public void PhotonConent(bool OFFLINE)
    {
        if (OFFLINE)
        {
            TC_ScriptCreate();
            TC_Script.ConnectPhoton(OFFLINE);
        }
        else
        {
            PM_ScriptCreate();
            PM_Script.ConnectPhoton(OFFLINE);
        }
    }

    //NetWork切断
    public void DisConnectPhoton()
    {
        if (SceneManager.GetActiveScene().name == Const.CO.LobbySceneName)
        {
            PM_ScriptCreate();
            PM_Script.SetMessage("");
            SceneManager.LoadScene(Const.CO.TitleSceneName);
        }
        else if (SceneManager.GetActiveScene().name == Const.CO.MattixSceneName)
        {
            PW_ScriptCreate();
            PW_Script.Popup.SetActive(true);
        }
    }

    //シーンが存在するか確かめる
    public bool ContainsScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    //オブジェクトが存在するか調べる
    public bool ContainObject(string ObjectName)
    {
        bool flag = GameObject.Find(ObjectName);
        return flag;
    }

    //オブジェクト生成
    public void CreateObject(GameObject Object,string ObjectName)
    {
        GameObject go = Instantiate(Object);
        go.name = ObjectName;
    }
}
