//Lobby画面のメインスクリプト
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Const; //定数を定義している

public class LobbyController : MonoBehaviour
{
    private GameObject PhotonCanvasObject; //Photon画面のCanvasの場所
    private GameObject OptionCanvasObject; //Option画面のCanvasの場所
    private PhotonManager PM_Script = null; //「PhotonManager」のComponentを格納する
    private SoundPresenter SoundP; //SoundPresenterのスクリプト
    private GameObject AudioObject; //Audioのオブジェクト
    
    private Canvas P_Canvas; //PhotonCanvas
    private Canvas Audio; //AudioCanvas

    public GameObject PhotonManagerObject;//PhotonManagerのオブジェクト
    public GameObject OnlineButtonObject;

    public Canvas PhotonCanvas
    {
        get { return P_Canvas; }
        set { P_Canvas = value; }
    }
    public Canvas AudioCanvas
    {
        get { return Audio; }
        set { Audio = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //PhotonManagerのスクリプト
        PM_Script = PhotonManagerObject.GetComponent<PhotonManager>();
        //OnlineButtonのスクリプト
        //CanvasのComponentがあるObjectの取得
        PhotonCanvasObject = GameObject.Find(Const.CO.PhotonCanvasName).gameObject;
        OptionCanvasObject = GameObject.Find(Const.CO.AudioCanvasName).gameObject;

        //Audioのあるオブジェクトを取得
        AudioObject = OptionCanvasObject.transform.parent.gameObject;
        //AudioObjectのcomponentを取得
        SoundP = AudioObject.GetComponent<SoundPresenter>();
        //Canvasのcomponentを取得
        P_Canvas = PhotonCanvasObject.GetComponent<Canvas>();
        AudioCanvas = OptionCanvasObject.GetComponent<Canvas>();
        P_Canvas.enabled = true;
        //BGM始動
        SoundP.ChangeBGM(2);
  
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
