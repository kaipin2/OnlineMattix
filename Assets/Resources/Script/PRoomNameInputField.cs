using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PRoomNameInputField : MonoBehaviourPunCallbacks
{
	#region Private変数定義
	private static string roomNamePrefKey = "RoomName";
	 TMP_InputField _inputField;
	#endregion
	#region MonoBehaviourコールバック
	void Start()
	{


		string defaultName = "";
		_inputField = this.GetComponent<TMP_InputField>();
		//UnityEngine.Debug.Log("_inputField.text:" + _inputField);
		//前回プレイ開始時に入力した名前をロードして表示
		if (_inputField != null)
		{
			if (PlayerPrefs.HasKey(roomNamePrefKey))
			{
				defaultName = PlayerPrefs.GetString(roomNamePrefKey);
				_inputField.text = defaultName;
			}
		}
	}
	#endregion
	#region Public Method
	public void SetPlayerName()
	{
		//PhotonNetwork.room.name = _inputField.text + " ";     //今回ゲームで利用するプレイヤーの名前を設定
		PlayerPrefs.SetString(roomNamePrefKey, _inputField.text);    //今回の名前をセーブ
		//Debug.Log(PhotonNetwork.NickName);   //playerの名前の確認。（動作が確認できればこの行は消してもいい）
	}
	#endregion
}
