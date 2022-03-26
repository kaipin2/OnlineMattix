using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Const; //’è”‚ğ’è‹`‚µ‚Ä‚¢‚é

public class OptionManager : MonoBehaviour
{
    private bool flag = false;
    private GameObject VolumeCanvasObject;
    private Canvas VolumeCanvas;
    public GameObject FinishButton;

    // Start is called before the first frame update
    void Start()
    {
        VolumeCanvasObject = transform.parent.gameObject.transform.parent.gameObject;
        VolumeCanvas = VolumeCanvasObject.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!flag && VolumeCanvas.enabled == true)
        {
            flag = true;
            if (SceneManager.GetActiveScene().name == CO.MattixSceneName)
            {
                FinishButton.SetActive(true);
            }
            else
            {
                FinishButton.SetActive(false);
            }
        }
        else if (flag && VolumeCanvas.enabled == false)
        {
            flag = false;
        }

    }
}
