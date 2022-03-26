using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Const; //�萔���`���Ă���

public class ExplanationController : MonoBehaviour
{
    //canvas��Component
    private Canvas Maincanvas; //���C����ʂ�Canvas��Component
    private Canvas Audiocanvas; //���ʉ�ʂ�Canvas��Component
    //���ʂ�ݒ肷���ʂ�Object
    private GameObject audiocanvas;

    private ImageDisp ID_Script; //ImageDisp�̃X�N���v�g

    private static int PageMax = 6; //�����̃y�[�W��
    private int PageNumber = 0; //���݂̃y�[�W��
    private TextMeshProUGUI Text; //�����p��Text�ꏊ
    private string[] Sentence = new string[PageMax]; //������
    //private Sprite[] sprite = new Sprite[PageMax]; //�C���[�W�摜
    
    public GameObject canvas; //�Q�[���̃��C����ʂ�Object
    public GameObject ImageBorad; //�����p�̃Q�[�����
    public GameObject TextObject; //�����p��Text�ꏊ
    public GameObject PageFeedButton; //�y�[�W��i�߂�{�^��
    public GameObject PageBackButton; //�y�[�W�����ǂ��{�^��

    private GameObject OptionCanvasObject; //Option��ʂ�Canvas�̏ꏊ
    private GameObject AudioObject; //Audio�̃I�u�W�F�N�g
    private SoundPresenter SoundP; //SoundPresenter�̃X�N���v�g

    #region PublicAccessor��`
    //���C����ʂ�Accessor
    public Canvas MainCanvas
    {
        get { return Maincanvas; }
        set { Maincanvas = value; }
    }
    //���ʉ�ʂ�Accessor
    public Canvas AudioCanvas
    {
        get { return Audiocanvas; }
        set { Audiocanvas = value; }
    }
    
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //������
        Initialize();
        
        //BGM�n��
        SoundP.ChangeBGM(1);

    }
    //������
    private void Initialize()
    {
        //BGM
        OptionCanvasObject = GameObject.Find(Const.CO.AudioCanvasName).gameObject;
        AudioObject = OptionCanvasObject.transform.parent.gameObject;
        SoundP = AudioObject.GetComponent<SoundPresenter>();

        audiocanvas = GameObject.Find(Const.CO.AudioCanvasName).gameObject; //���ʉ�ʂ�Object���擾
        Audiocanvas = audiocanvas.GetComponent<Canvas>(); //���ʉ�ʂ�Canvas��Component���擾

        Maincanvas = canvas.GetComponent<Canvas>(); //���C����ʂ�Object���擾

        Text = TextObject.GetComponent<TextMeshProUGUI>();
        ID_Script = ImageBorad.GetComponent<ImageDisp>();
        string str = "";
        //sprite sp = null;
        for(int i = 0; i < PageMax;i++)
        {
            switch (i)
            {
                case 0:
                    str = "Mattix�Ƃ́A\n���ƌ��ɕ�����ċ����荇�����_�������Q�[���ł�";
                    break;
                case 1:
                    str = "�Տ�̐Ԃ���Q�l�̃v���C���[�̌��݈ʒu�ł��B";
                    break;
                case 2:
                    str = "���͌��݈ʒu�Ɠ�������̋��P��I�����A�擾�ł��܂��B\n�擾��͐Ԃ�������̏ꏊ�Ɉړ������܂��B";
                    break;
                case 3:
                    str = "���͌��݈ʒu�Ɠ����c��̋��P��I�����A���Ɠ����悤�Ɏ擾���ĐԂ�����ړ������܂��B";
                    break;
                case 4:
                    str = "��������݂ɌJ��Ԃ��čs���A�����Ȃ��Ȃ������A�Q�[�����I�����A���̎��̓��_�������v���C���[�̏����ƂȂ�܂�";
                    break;
                case 5:
                    str = "�������A�e�v���C���[�ɂ͐������Ԃ����݂��A���̎��Ԃ��Ȃ��Ȃ����炻�̃v���C���[�̕����ɂȂ��Ă��܂��܂��B";
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
