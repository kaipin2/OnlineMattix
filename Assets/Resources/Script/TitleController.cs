//Title画面のメインスクリプト
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //TextMeshProを使用するのに必要
using UnityEngine.UI; //UIを使用するのに必要

public class TitleController : MonoBehaviour
{
    private Buttons btn; //ボタンのスクリプト
    private GameObject VersionObject;//ゲームのバージョンを示すテキストが存在するObject
    private TextMeshProUGUI VersionText;//ゲームのバージョンを示すテキスト

    private GameObject TitleCanvasObject; //Title画面のCanvasの場所
    private GameObject OptionCanvasObject; //Option画面のCanvasの場所

    private GameObject AudioObject; //Audioのオブジェクト
    private SoundPresenter SoundP; //SoundPresenterのスクリプト
    private Canvas Audio; //AudioCanvas
    private Canvas Canvas; //MainCanvas

    private TextMeshProUGUI TitleText; //TitleのText
    private TextMeshProUGUI OptionText; //OptionのText

    private static int ButtonNumber = 4; // Title画面にあるボタンの数
    public GameObject Button; //Title画面にあるボタン(Unity内で設定)

    

    public Canvas AudioCanvas
    {
        get { return Audio; }
        set { Audio = value; }
    }

    public Canvas TitleCanvas
    {
        get { return Canvas; }
        set { Canvas = value; }
    }

    public TextMeshProUGUI TitleTextMeshPro
    {
        get { return TitleText; }
        set { TitleText = value; }
    }
    public TextMeshProUGUI OptionTextMeshPro
    {
        get { return OptionText; }
        set { OptionText = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        btn = this.gameObject.AddComponent<Buttons>();
        if (VersionObject == null)
        {
            VersionObject = GameObject.Find(Const.CO.VersionTextName).gameObject;
        }
        if (VersionText == null)
        {
            VersionText = VersionObject.GetComponent<TextMeshProUGUI>();
        }
        VersionText.text = Const.CO.GameVersion;

        //CanvasのComponentがあるObjectの取得
        TitleCanvasObject = GameObject.Find(Const.CO.CanvasName).gameObject;
        OptionCanvasObject = GameObject.Find(Const.CO.AudioCanvasName).gameObject;

        //Audioのあるオブジェクトを取得
        AudioObject = OptionCanvasObject.transform.parent.gameObject;

        //Canvasのcomponentを取得
        TitleCanvas = TitleCanvasObject.GetComponent<Canvas>();
        AudioCanvas = OptionCanvasObject.GetComponent<Canvas>();

        //AudioObjectのcomponentを取得
        SoundP = AudioObject.GetComponent<SoundPresenter>();

        //BGM始動
        SoundP.ChangeBGM(2);

        //Title内のTextのComponentを取得
        TitleText = GameObject.Find(Const.CO.TitleName).GetComponent<TextMeshProUGUI>();
        OptionText = GameObject.Find(Const.CO.AudioCanvasName + "/" + Const.CO.TitleName).GetComponent<TextMeshProUGUI>();

        SetButton(); //Title画面のButtonを配置

        InitializeCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeCanvas()
    {
        TitleCanvas.enabled = true;
        AudioCanvas.enabled = false;
        //P_Canvas.enabled = false;
    }

    void SetButton()
    {
        Vector3 ButtonPosition = new Vector3(-242, 162, 0); //Buttonの位置
        Vector3 ButtonScale = new Vector3(3, 3, 3); //Buttonの大きさ
        Vector3 ButtonInterval = new Vector3(0, -135,0); //Button間の間隔
        int Width = 160;
        int Height = 30;

        for(int button = 0 ; button < ButtonNumber ; button++)
        {
            GameObject Gob = Instantiate(Button);//Object生成
            TextMeshProUGUI buttontext = Gob.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();//ボタン内部のテキスト
            //ボタン内部のテキストの詳細(ButtonNumber分必要)
            string[] text = { "OnlineGame","OfflineGame","How To Play","Exit"};
            buttontext.text = text[button]; //テキストを配置
            Gob.name = text[button] + "Button";//オブジェクトの名前を変更
            Gob.transform.SetParent(TitleCanvasObject.transform, false);//親を設定
            
            //ボタンの位置と間隔
            Gob.transform.localPosition = ButtonPosition;
            ButtonPosition += ButtonInterval;
            
            Gob.transform.localScale = ButtonScale;//ボタンの大きさ(何倍か)

            
            //ボタンのサイズ
            var rtf = Gob.GetComponent<RectTransform>();
            // 横方向のサイズ
            rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);
            // 縦方向のサイズ
            rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);

            //ボタンのアクションの設定
            switch (button)
            {
                case 0:
                    Gob.GetComponent<Button>().onClick.AddListener(btn.LobbyButton);
                    break;
                case 1:
                    Gob.GetComponent<Button>().onClick.AddListener(btn.OffLineBuuton);
                    break;
                case 2:
                    Gob.GetComponent<Button>().onClick.AddListener(btn.HowToPlayButton);
                    break;
                case 3:
                    Gob.GetComponent<Button>().onClick.AddListener(btn.ExitButton);
                    break;
            }
        }
        /*
        Button SetButton = Button[0].GetComponent<Button>();
        UnityEngine.Debug.Log("SetButton.name:" + SetButton.name);
        //ボタンが選択された状態になる
        SetButton.Select();
        */
    }
}
