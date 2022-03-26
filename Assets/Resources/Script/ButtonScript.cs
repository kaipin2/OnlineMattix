using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI; //UIを使用するのに必要

public class ButtonScript : MonoBehaviourPunCallbacks
{

    private GameObject board;

    void Start()
    {

    }
    private void Update()
    {
    }

    public void ChangeColor(Color color)
    {
        photonView.RPC(nameof(RpcChangeColor), RpcTarget.All, color);
    }

    public void ChangeName(string name)
    {
        photonView.RPC(nameof(RpcChangeName), RpcTarget.All, name);
    }

    public void ChangeScale(Vector3 scale)
    {
        photonView.RPC(nameof(RpcChangeScale), RpcTarget.All, scale);
    }
    
    public void ChangeSize(float w, float h)
    {
        photonView.RPC(nameof(RpcChangeSize), RpcTarget.All, w, h);
    }

    public void SetParent(string str)
    {
        photonView.RPC(nameof(RpcSetParent), RpcTarget.All,str);
    }

    public void ChangeText(string URL, string text)
    {
        photonView.RPC(nameof(RpcChangeText), RpcTarget.All, URL,text);
    }

    public void SetAction(Buttons btn ,string script)
    {
        photonView.RPC(nameof(RpcSetAction), RpcTarget.All,btn,script);
    }

    [PunRPC]
    private void RpcChangeColor(Color color)
    {
        this.gameObject.GetComponent<Renderer>().material.color = color;
    }

    [PunRPC]
    private void RpcChangeName(string name)
    {
        this.gameObject.name = name;
    }

    [PunRPC]
    private void RpcChangeScale(Vector3 scale)
    {
        this.gameObject.transform.localScale = scale;
    }

    [PunRPC]
    private void RpcChangeSize(float w, float h)
    {
        var rtf = this.gameObject.GetComponent<RectTransform>();//ボタンのサイズ
        rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);// 横方向のサイズ
        rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);// 縦方向のサイズ
    }

    [PunRPC]
    private void RpcSetParent(string str)
    {
        if (board == null) board = GameObject.Find(str).gameObject;
        this.gameObject.transform.SetParent(board.transform, false);
    }

    [PunRPC]
    private void RpcChangeText(string URL ,string text)
    {
        this.gameObject.transform.Find(URL).gameObject.GetComponent<TextMeshProUGUI>().text = text;
    }

    [PunRPC]
    private void RpcSetAction(Buttons btn, string script)
    {
        switch (script)
        {
            case "LobbyButton":
                this.gameObject.GetComponent<Button>().onClick.AddListener(btn.LobbyButton);//ボタンのアクションを設定
                break;
            case "GameStartButton":
                this.gameObject.GetComponent<Button>().onClick.AddListener(btn.GameStartButton);//ボタンのアクションを設定
                break;
        }
        
    }
}
