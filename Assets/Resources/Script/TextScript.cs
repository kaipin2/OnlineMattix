using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //TextMeshProを使用するのに必要
using Photon.Pun;
using Photon.Realtime;

public class TextScript : MonoBehaviourPunCallbacks
{
    public void ChangeText(string text)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RpcChangeText), RpcTarget.All, text);
        }
        else
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = text;
        }
    }

    public void ChengeText_Diff(string text)
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ChangeAlignment(string alignment)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RpcChangeAlignment), RpcTarget.All, alignment);
        }
        else
        {
            if (alignment == "Right")
            {
                this.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
            }
            else if(alignment == "Left")
            {
                this.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
            }
            else if(alignment == "Center")
            {
                this.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            }
        }
    }

    public void ChangeColor(Color color)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RpcChangeColor), RpcTarget.All, color);
        }
        else
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().color = color;
        }
    }

    [PunRPC]
    private void RpcChangeText(string text)
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = text;
    }

    [PunRPC]
    private void RpcChangeAlignment(string alignment)
    {
        if(alignment == "Right")
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        }
        else if (alignment == "Left")
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        }
        else if (alignment == "Center")
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        }
    }

    [PunRPC]
    private void RpcChangeColor(Color color)
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().color = color;
    }
}
