using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class LobbyController : MonoBehaviour {

	public Button startMatchButton;
	private MyNetworkManager manager;
	public InputField matchNameField;

	public GameObject listItemPrefab;

	private List<ListItem> listItems;

	public GameObject content;
	public Toggle malute;

	// Use this for initialization
	void Start () {
		manager = GameObject.Find ("Network Manager").GetComponent<MyNetworkManager>();
		manager.StartMatchMaker();


		startMatchButton.onClick.AddListener (StartOnlineMatch);
		listItems = new List<ListItem> ();

		//manager.matchMaker.ListMatches(0,20, "", manager.OnMatchList);
		manager.matchMaker.ListMatches(0,20,"",false,0,0,manager.OnMatchList);
		RefreshLobby ();
		//InvokeRepeating ("RefreshLobby", 5, 5);


	}

	public void RefreshLobby(){
		
		//manager.matchMaker.ListMatches(0,20, "", manager.OnMatchList);
		manager.matchMaker.ListMatches(0,20,"",false,0,0,onMatchList);

	}



	void StartOnlineMatch(){
		manager.malute = malute.isOn;
		int eloScore = malute.isOn ? 0 : 1;
		manager.matchMaker.CreateMatch (matchNameField.text, manager.matchSize, true, "", "", "", eloScore, 0, manager.OnMatchCreate);
		//manager.matchMaker.c


	}


	//TODO: there is some bug here 
	void onMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> resmatch){

		manager.OnMatchList (success,extendedInfo,resmatch);

		foreach(var item in listItems){
			item.Destroy ();

		}
		listItems.Clear ();

		foreach(var match in manager.matches){
			var listItem = Instantiate (listItemPrefab, content.transform).GetComponent<ListItem>();
			listItem.setName (match.name);
			listItem.setNumPlayers (match.currentSize);
			listItem.setNumMaluteType (match.averageEloScore);
			listItem.joinButton.onClick.AddListener (()=>{
				manager.matchMaker.JoinMatch(match.networkId,"","","",0,0,manager.OnMatchJoined);
			});
			listItems.Add (listItem);
		}


	}


	void Update(){

	}
}
