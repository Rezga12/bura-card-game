using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PauseMenuController : NetworkBehaviour {

	private GameController table;
	private Button surrButton;

	// Use this for initialization
	void Start () {
		
		table = GameObject.Find ("Table").GetComponent<GameController>();
		//surrButton = table.pauseMenu.transform.GetChild (0);
		table.surrButton.onClick.AddListener(() => {surrender();});

	}
	
	// Update is called once per frame
	void Update () {

		if(!isLocalPlayer){
			return;
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			table.paused = !table.paused;
			table.pauseMenu.gameObject.SetActive (table.paused);
		}
	}

	void surrender(){
		table.pauseMenu.gameObject.SetActive (false);
		if (isServer) {
			
			table.GetComponent<GameOverManager> ().GameOver (false);
		} else {
			CmdSurrender ();
		}
	}

	[Command]
	void CmdSurrender(){
		
		table.GetComponent<GameOverManager> ().GameOver (true);
	}
}
