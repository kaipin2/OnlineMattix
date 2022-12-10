using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; //TextMeshPro���g�p����̂ɕK�v

using Const;  //�萔���`���Ă���

public class OptionStatusController : MonoBehaviour
{
    private GameObject PopupWindowObject; //PopupWindow���i�[����Object
    private GameObject DropdownObject;
    private Buttons btn; //�{�^���̃X�N���v�g

    private Canvas VolumeCanvas;
    private TMP_Dropdown ddtmp = null;
    private int board_size;

    public int Size
    {
        get { return board_size; }
        set { board_size = value; }
    }

    public GameObject Popup
    {
        get { return PopupWindowObject; }
        set { PopupWindowObject = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //PopupWindow��Object�̎擾
        Popup = GameObject.Find(Const.CO.PopUpWindowName).gameObject;
        Popup.SetActive(false);

        VolumeCanvas = GameObject.Find(Const.CO.AudioCanvasName).GetComponent<Canvas>();
        DropdownObject = GameObject.Find(Const.CO.DropdownName);
        ddtmp = DropdownObject.GetComponent<TMP_Dropdown>();

        btn = this.gameObject.AddComponent<Buttons>();
        btn.PM_ScriptCreate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Size != int.Parse(ddtmp.options[ddtmp.value].text))
        {
            Size = int.Parse(ddtmp.options[ddtmp.value].text);
        }
        if (this.GetComponent<Canvas>().worldCamera == null)
        {
            this.GetComponent<Canvas>().worldCamera = GameObject.Find(Const.CO.MainCamera).GetComponent<Camera>();
        }
        if (ContainsScene(Const.CO.MattixSceneName)) {
            DropdownObject.SetActive(false);
        }else
        {
            DropdownObject.SetActive(true);
        }
    }

    //�V�[�������݂��邩�m���߂�
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
}
