using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	public AudioSource pickFromDeck;
	public AudioSource cardClearing;
	public AudioSource cardAttacking;
	public AudioSource cardAnswering;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void playPickFromDeck(){
		pickFromDeck.Play ();
	}

	public void playCardClearing(){
		cardClearing.Play ();
	}

	public void playCardAttacking(){
		cardAttacking.Play ();
	}

	public void playCardAnswering(){
		cardAnswering.Play ();
	}
}
