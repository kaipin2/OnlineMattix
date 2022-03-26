//表示させるかさせないかを判断し、表示や非表示を切り替えるスクリプト
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UIを使用するのに必要
using TMPro; //TextMeshProを使用するのに必要

public class Enabled : MonoBehaviour
{
    private GameObject parentGameObject; //このスクリプトをComponentしているObjectの親Objectの親Objectの……
    private bool Display = false;
    private static int Parent = 2; //何個上の親まで検索するか
    public bool Start_UP = true;

    // Start is called before the first frame update
    void Start()
    {
        parentGameObject = transform.parent.gameObject; //CanvasがComponentされているObject
    }

    // Update is called once per frame
    void Update()
    {
        if (Start_UP)
        {
            for (int i = 0; i < Parent; i++)
            {
                //一つ上の親のComponentにCanvasが存在するなら
                if (parentGameObject.GetComponent<Canvas>() != null)
                {
                    //親のCanvasが非表示状態なら
                    if (!parentGameObject.GetComponent<Canvas>().enabled)
                    {
                        Display = false;
                    }
                    else if (parentGameObject.GetComponent<Canvas>().enabled)
                    {
                        Display = true;
                    }

                    if (this.gameObject.GetComponent<Slider>() != null) //Sliderに対して
                    {
                        this.gameObject.GetComponent<Slider>().enabled = Display;
                    }
                    else if (this.gameObject.GetComponent<Button>() != null) //Buttonに対して
                    {
                        this.gameObject.GetComponent<Button>().enabled = Display;
                    }else if(this.gameObject.GetComponent<TextMeshProUGUI>() != null) //TextMeshProに対して
                    {
                        this.gameObject.GetComponent<TextMeshProUGUI>().enabled = Display;
                    }
                    else if (this.gameObject.GetComponent<TMP_InputField>() != null) //TextMeshProの入力欄に対して
                    {
                        this.gameObject.GetComponent<TMP_InputField>().enabled = Display;
                    }
                    break;
                }
                parentGameObject = parentGameObject.transform.parent.gameObject;
            }
        }
    
    }
}
