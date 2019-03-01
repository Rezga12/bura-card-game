using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class XController : NetworkBehaviour {

	public GameController table;

	public CardistController player1;
	public CardistController player2;

	public Material redMat;
	public Material greenMat;
	public Camera cam;
	// Use this for initialization

	string youText;
	string opponentText;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {



		if (table.GetComponent<Timer> ().remainingTime <= 0) {
			player1.forfeitRound ();
			player1.setWaitingForAnswer (false);
			disappear ();
			table.offeredMode = false;

			player1.removeKozir ();
			player1.GetComponent<TimerController> ().stopTimer ();
		}

		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit)){
			if (hit.transform.gameObject.name == "X_Cube") {
				changeColor (0,1,-10,0.02f,redMat);
				if(Input.GetKeyDown(KeyCode.Mouse0)){
					player1.forfeitRound ();
					player1.setWaitingForAnswer (false);
					disappear ();
					table.offeredMode = false;

					player1.removeKozir ();
					player1.GetComponent<TimerController> ().stopTimer ();
					player1.messanger.DisplayTurn (false);


					opponentText = player1.messanger.box.opponentOffers;
					player1.messanger.onTakingCards (true,opponentText,opponentText);
				}
			}else{
				changeColor (0,10,0,-0.02f,redMat);
			}
			if (hit.transform.gameObject.name == "Y_Cube") {
				changeColor (1,1,-10,0.02f,greenMat);
				if(Input.GetKeyDown(KeyCode.Mouse0)){
					player1.riseRoundValue ();
					table.canRaise = true;
					disappear ();
					player1.setWaitingForAnswer (false);
					table.offeredMode = false;

					player1.GetComponent<TimerController> ().startTimer ();
					player1.messanger.DisplayTurn (false);

					youText = player1.messanger.box.raiseAccept;
					player1.messanger.onTakingCards (true,youText,youText);
				}
			}else{
				changeColor (1,10,0,-0.02f,greenMat);
			}


		}




	}


	private void changeColor(int index,float maxColor, float minColor, float delta,Material mat){
		Color col =  mat.GetColor ("_EmissionColor");
		if (col[index] >= maxColor || col[index] <= minColor) {
			return;
		}
		col[index] = col[index] + delta;
		mat.SetColor ("_EmissionColor",col);

	}

	public void appear(){
		gameObject.SetActive (true);
	}

	public void disappear(){
		gameObject.SetActive (false);
	}

}
