using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyNetworkManager : NetworkManager {

	public bool malute;

	// Use this for initialization
	void Start () {
		

	}



	
	// Update is called once per frame
	void Update () {
		
	}



	public override void OnClientSceneChanged(NetworkConnection conn)
	{
		// always become ready.
		if(!ClientScene.ready)
			ClientScene.Ready(conn);

		if (!autoCreatePlayer)
		{
			return;
		}

		bool addPlayer = (ClientScene.localPlayers.Count == 0);
		bool foundPlayer = false;
		foreach (var playerController in ClientScene.localPlayers)
		{
			if (playerController.gameObject != null)
			{
				foundPlayer = true;
				break;
			}
		}
		if (!foundPlayer)
		{
			// there are players, but their game objects have all been deleted
			addPlayer = true;
		}
		if (addPlayer)
		{
			ClientScene.AddPlayer(0);
		}
	}

}
