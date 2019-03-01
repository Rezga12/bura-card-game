using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public float remainingTime;
	public float timeForTurn;

	public Text timerLabel;
	public Color startingColor;

	// Use this for initialization
	void Start () {
		



	}
	
	// Update is called once per frame
	void Update () {
		if(started){
			remainingTime = remainingTime - Time.deltaTime;
			remainingTime = remainingTime <= 0 ? 0 : remainingTime;
			//Debug.Log ("lel");
			if (timerLabel.text != "" + (int)remainingTime) {
				timerLabel.color = timerLabel.color + new Color (0.6f,-0.01f,0,0)/remainingTime;
				timerLabel.text = "" + (int)remainingTime;
			}
				
		}
	}

	private bool started;

	public void startTimer(){
		Debug.Log ("hey");
		timerLabel.color = startingColor;
		remainingTime = timeForTurn;
		started = true;


	}

	public void stopTimer(){
		started = false;
		remainingTime = timeForTurn;
		timerLabel.text = "" + (int)remainingTime;
		timerLabel.color = startingColor;
	}


}
