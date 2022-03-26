using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Const; //定数を定義している

public class ExplanationController : MonoBehaviour
{
    //canvasのComponent
    private Canvas Maincanvas; //メイン画面のCanvasのComponent
    private Canvas Audiocanvas; //音量画面のCanvasのComponent
    //音量を設定する画面のObject
    private GameObject audiocanvas;

    private ImageDisp ID_Script; //ImageDispのスクリプト

    private static int PageMax = 6; //説明のページ数
    private int PageNumber = 0; //現在のページ数
    private TextMeshProUGUI Text; //説明用のText場所
    private string[] Sentence = new string[PageMax]; //説明文
    //private Sprite[] sprite = new Sprite[PageMax]; //イメージ画像
    
    public GameObject canvas; //ゲームのメイン画面のObject
    public GameObject ImageBorad; //説明用のゲーム画面
    public GameObject TextObject; //説明用のText場所
    public GameObject PageFeedButton; //ページを進めるボタン
    public GameObject PageBackButton; //ページをもどすボタン

    private GameObject OptionCanvasObject; //Option画面のCanvasの場所
    private GameObject AudioObject; //Audioのオブジェクト
    private SoundPresenter SoundP; //SoundPresenterのスクリプト

    #region PublicAccessor定義
    //メイン画面のAccessor
    public Canvas MainCanvas
    {
        get { return Maincanvas; }
        set { Maincanvas = value; }
    }
    //音量画面のAccessor
    public Canvas AudioCanvas
    {
        get { return Audiocanvas; }
        set { Audiocanvas = value; }
    }
    
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //初期化
        Initialize();
        
        //BGM始動
        SoundP.ChangeBGM(1);

    }
    //初期化
    private void Initialize()
    {
        //BGM
        OptionCanvasObject = GameObject.Find(Const.CO.AudioCanvasName).gameObject;
        AudioObject = OptionCanvasObject.transform.parent.gameObject;
        SoundP = AudioObject.GetComponent<SoundPresenter>();

        audiocanvas = GameObject.Find(Const.CO.AudioCanvasName).gameObject; //音量画面のObjectを取得
        Audiocanvas = audiocanvas.GetComponent<Canvas>(); //音量画面のCanvasのComponentを取得

        Maincanvas = canvas.GetComponent<Canvas>(); //メイン画面のObjectを取得

        Text = TextObject.GetComponent<TextMeshProUGUI>();
        ID_Script = ImageBorad.GetComponent<ImageDisp>();
        string str = "";
        //sprite sp = null;
        for(int i = 0; i < PageMax;i++)
        {
            switch (i)
            {
                case 0:
                    str = "Mattixとは、\n先手と後手に分かれて駒を取り合い得点を競うゲームです";
                    break;
                case 1:
                    str = "盤上の赤い駒が２人のプレイヤーの現在位置です。";
                    break;
                case 2:
                    str = "先手は現在位置と同じ横列の駒から１つを選択し、取得できます。\n取得後は赤い駒をその場所に移動させます。";
                    break;
                case 3:
                    str = "後手は現在位置と同じ縦列の駒から１つを選択し、先手と同じように取得して赤い駒を移動させます。";
                    break;
                case 4:
                    str = "これを交互に繰り返して行い、取れる駒がなくなった時、ゲームが終了し、その時の得点が多いプレイヤーの勝利となります";
                    break;
                case 5:
                    str = "ただし、各プレイヤーには制限時間が存在し、その時間がなくなったらそのプレイヤーの負けになってしまいます。";
                    break;

            }
            Sentence[i] = str;
            //image.sprite = sprite;

        }

        PageBackButton.SetActive(false);
        PageFeedButton.SetActive(true);
        PageNumber = 0;
        ID_Script.ChengeImage(PageNumber);
        Text.text = Sentence[PageNumber];
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void PageNest()
    {
        ID_Script.ChengeImage(PageNumber + 1);
        Text.text = Sentence[++PageNumber];
        if (!PageBackButton.activeSelf)
        {
            PageBackButton.SetActive(true);
        }
        if(PageNumber == PageMax-1)
        {
            PageFeedButton.SetActive(false);
        }
    }
    public void PageBack()
    {
        ID_Script.ChengeImage(PageNumber - 1);
        Text.text = Sentence[--PageNumber];
        if (!PageFeedButton.activeSelf)
        {
            PageFeedButton.SetActive(true);
        }
        if (PageNumber == 0)
        {
            PageBackButton.SetActive(false);
        }
    }
}
