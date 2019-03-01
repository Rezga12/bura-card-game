using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoTextBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (startDissapearing());
	}
	

	void Update () {
		if(start){
			GetComponent<Text> ().color -= new Color (0,0,0,0.005f);
			if(GetComponent<Text> ().color.a <= 0){
				Destroy (gameObject);
			}
		}
	}
	bool start;
	IEnumerator startDissapearing(){
		yield return new WaitForSeconds (3f);
		start = true;


	}
}
