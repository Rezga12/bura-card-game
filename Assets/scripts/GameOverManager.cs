using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameOverManager : NetworkBehaviour {

	public MyNetworkManager manager;
	public Button LoseButton;
	public Button WinButton;

	public GameObject LoseScreen;
	public GameObject WinScreen;

	// Use this for initialization
	void Start () {
		manager = GameObject.Find ("Network Manager").GetComponent<MyNetworkManager>();
		WinButton.onClick.AddListener (disconnect);
		LoseButton.onClick.AddListener (disconnect);
	}
	
	public void GameOver(bool win){
		WinScreen.SetActive (win);
		LoseScreen.SetActive (!win);
		RpcGameOver (!win);
		Debug.Log (win);
	}

	[ClientRpc]
	private void RpcGameOver(bool win){
		if (isServer) {
			return;
		}
		Debug.Log ("commandActivated");
		WinScreen.SetActive (win);
		LoseScreen.SetActive (!win);
	}

	private void disconnect(){
		Debug.Log ("disconnect");
		manager.StopHost ();
	}


}
