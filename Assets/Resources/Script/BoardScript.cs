using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class BoardScript : MonoBehaviourPunCallbacks
{
    private GameObject board = null;

    public void ChangeColor(Color color)
    {
        photonView.RPC(nameof(RpcChangeColor), RpcTarget.All,color);
    }

    public void ChangeName(string name)
    {
        photonView.RPC(nameof(RpcChangeName), RpcTarget.All, name);
    }

    public void ChangeScale(Vector3 scale)
    {
        photonView.RPC(nameof(RpcChangeScale), RpcTarget.All, scale);
    }

    public void SetParent()
    {
        photonView.RPC(nameof(RpcSetParent), RpcTarget.All);
    }

    public void ChangeOwnership(int id)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            this.gameObject.GetComponent<PhotonView>().TransferOwnership(id);
        }
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
    private void RpcSetParent()
    {
        if (board == null)
        {
            board = GameObject.Find("Canvas").gameObject;
        }
        this.gameObject.transform.SetParent(board.transform, false);
    }

}
