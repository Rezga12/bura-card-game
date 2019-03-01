using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour {

	//Card Deck
	public Card[] Cards;

	// Use this for initialization
	void Start () {
		aboveIndex = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private int aboveIndex;

	public void removeCard(){
		transform.GetChild (aboveIndex).gameObject.SetActive (false);
		aboveIndex++;
	}



	public Transform getAboveCard(){
		return transform.GetChild (aboveIndex);
	}

	public void revert(){
		Debug.Log ("yee");
		for(int i=1;i<transform.childCount;i++){
			transform.GetChild (i).gameObject.SetActive (true);
		}
		aboveIndex = 1;
	}
}
