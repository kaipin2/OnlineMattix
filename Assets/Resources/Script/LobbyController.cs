//Lobby��ʂ̃��C���X�N���v�g
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Const; //�萔���`���Ă���

public class LobbyController : MonoBehaviour
{
    private GameObject PhotonCanvasObject; //Photon��ʂ�Canvas�̏ꏊ
    private GameObject OptionCanvasObject; //Option��ʂ�Canvas�̏ꏊ
    private PhotonManager PM_Script = null; //�uPhotonManager�v��Component���i�[����
    private SoundPresenter SoundP; //SoundPresenter�̃X�N���v�g
    private GameObject AudioObject; //Audio�̃I�u�W�F�N�g
    
    private Canvas P_Canvas; //PhotonCanvas
    private Canvas Audio; //AudioCanvas

    public GameObject PhotonManagerObject;//PhotonManager�̃I�u�W�F�N�g
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
        //PhotonManager�̃X�N���v�g
        PM_Script = PhotonManagerObject.GetComponent<PhotonManager>();
        //OnlineButton�̃X�N���v�g
        //Canvas��Component������Object�̎擾
        PhotonCanvasObject = GameObject.Find(Const.CO.PhotonCanvasName).gameObject;
        OptionCanvasObject = GameObject.Find(Const.CO.AudioCanvasName).gameObject;

        //Audio�̂���I�u�W�F�N�g���擾
        AudioObject = OptionCanvasObject.transform.parent.gameObject;
        //AudioObject��component���擾
        SoundP = AudioObject.GetComponent<SoundPresenter>();
        //Canvas��component���擾
        P_Canvas = PhotonCanvasObject.GetComponent<Canvas>();
        AudioCanvas = OptionCanvasObject.GetComponent<Canvas>();
        P_Canvas.enabled = true;
        //BGM�n��
        SoundP.ChangeBGM(2);
  
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
