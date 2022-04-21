using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

using Const; //定数を定義している

public class PointPieceScript : MonoBehaviourPunCallbacks 
{

    private GameObject board;
    private GameObject child; //子供のGameObject
    private GameObject grandchild; //孫のGameObject
    private TextMeshProUGUI PointText;
    private GameObject VolumeManager;
    private AudioManager VCscript;

    public void ChangePosition(Vector3 vec ,bool local)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RpcChangePosition), RpcTarget.All, vec,local);
        }
        else
        {
            if (local)
            {
                this.gameObject.transform.localPosition = vec;
            }
            else
            {
                this.gameObject.transform.position = vec;
            }

        }
    }

    public void ChangeGetPosition(string Player,int stoneX,Vector3 vec)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            photonView.RPC(nameof(RpcChangeGetPosition), RpcTarget.All, Player, stoneX);
        }
        else
        {
            GameObject canvas = this.gameObject.transform.parent.gameObject;
            
            //this.gameObject.transform.SetParent(canvas.transform.Find("Player" + Player + "/Cube" + Player).gameObject.transform, false);
            this.gameObject.transform.localPosition = vec;
            
        }

    }

    public void ChangeColor(Color color, int type)
    {
        photonView.RPC(nameof(RpcChangeColor), RpcTarget.All, color,type);
    }

    public void ChangePoint(int point)
    {
        photonView.RPC(nameof(RpcChangePoint), RpcTarget.All, point);
    }

    public void ChangeScale(Vector3 scale)
    {
        photonView.RPC(nameof(RpcChangeScale), RpcTarget.All, scale);
    }

    public void SE(int SE)
    {
        photonView.RPC(nameof(RpcSE), RpcTarget.All, SE);
    }

    public void SetParent()
    {
        photonView.RPC(nameof(RpcSetParent), RpcTarget.All);
    }

    [PunRPC]
    private void RpcChangePosition(Vector3 position, bool local)
    {
        if (local)
        {
            this.gameObject.transform.localPosition = position;
        }
        else
        {
            this.gameObject.transform.position = position;
        }

    }

    [PunRPC]
    private void RpcChangeGetPosition(string Player, int stoneX)
    {
        GameObject canvas = this.gameObject.transform.parent.gameObject;
        this.gameObject.transform.position = canvas.transform.Find("Player" + Player + "/Cube" + Player + "/Plane" + stoneX).gameObject.transform.position;

    }

    [PunRPC]
    private void RpcChangeColor(Color color, int type)
    {
        child = this.gameObject;

        for (int i = 0; i < type; i++)
        {
            //UnityEngine.Debug.Log("child:" + child);
            child = child.transform.GetChild(0).gameObject;
        }
        child.GetComponent<Renderer>().material.color = color;
    }

    [PunRPC]
    private void RpcChangePoint(int point)
    {
        child = this.gameObject;
        
        for (int i = 0;i < 3 ;i++)
        {
            PointText = child.GetComponent<TextMeshProUGUI>();
            if (PointText == null)
            {
                child = child.transform.GetChild(0).gameObject;
            }
            else
            {
                break;
            }
        }
        PointText.text = point.ToString();
    }

    [PunRPC]
    private void RpcChangeScale(Vector3 scale)
    {
        this.gameObject.transform.localScale = scale;
    }

    [PunRPC]
    private void RpcSE(int SE)
    {
        if (VolumeManager == null) VolumeManager = GameObject.Find(Const.CO.AudioObjectName);
        if (VCscript == null) VCscript = VolumeManager.GetComponent<AudioManager>();
        VCscript.PlaySound(SE);
    }

    [PunRPC]
    private void RpcSetParent()
    {
        if (board == null) board = GameObject.Find("Canvas").gameObject;
        this.gameObject.transform.SetParent(board.transform, false);
    }
}
