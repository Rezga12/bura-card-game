using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour {

	private Text matchName;
	private Text numberOfPlayers;
	public Button joinButton;
	private Text maluteType;
	void Start () {
		
	}

	void OnEnable(){
		matchName = transform.Find ("Room Name").gameObject.GetComponent<Text>();
		numberOfPlayers = transform.Find ("Number Of Players").gameObject.GetComponent<Text> ();
		joinButton = transform.Find ("Join").gameObject.GetComponent<Button> ();
		maluteType = transform.Find ("Maliutka").gameObject.GetComponent<Text> ();
	}
	
	public void Destroy(){
		Destroy (gameObject);
	}


	public void setName(string name){
		
		matchName.text = name;
	}

	public void setNumPlayers(int num){
		
		numberOfPlayers.text = "Players: " + num;
	}

	public void setNumMaluteType(int num){
		if (num == 0) {
			maluteType.text = "Malute without turn";
		} else {
			maluteType.text = "Malute with turn";
		}
	}

}
