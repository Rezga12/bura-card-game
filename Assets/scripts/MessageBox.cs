using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {

	public string loseMessage;
	public string winMessage;

	public string yourTurnText;
	public string opponentsTurnText;

	public Color yourTurnColor; 
	public Color opponentsTurnCOlor;

	public Text turnText;

	public GameObject infoBox;
	public GameObject textPrefab;

	public string youTakeText;
	public string opponentTakeText;

	public string playerFinishTurn;
	public string opponentFinishTurn;

	public string timesUpForYou;
	public string timesUpForOpponent;

	public string opponentOffers;
	public string youOffer;

	public string raiseAccept;
	public string raiseDenied;

	public string youLose;
	public string youWin;

	public Text yourScore;
	public Text opponentsScore;

	public Text WinningValue;

	// Use this for initialization
	void Start () {
		
	}
	
	public void Turn(bool your){

		if(your){
			turnText.text = yourTurnText;
			turnText.color = yourTurnColor;
		}else{
			turnText.text = opponentsTurnText;
			turnText.color = opponentsTurnCOlor;
		}
	}

	public void addInfo(string info){
		Text infoText = Instantiate (textPrefab,infoBox.transform).GetComponent<Text>();
		infoText.text = info;
		infoText.transform.SetSiblingIndex (0);
	}

	public void addWinInfo(bool won){
		Text infoText = Instantiate (textPrefab,infoBox.transform).GetComponent<Text>();
		if(won){
			infoText.text = youWin;
			infoText.transform.SetSiblingIndex (0);
			infoText.color = Color.green;
		}else{
			infoText.text = youLose;
			infoText.transform.SetSiblingIndex (0);
			infoText.color = Color.red;
		}

	}


	public void setWinningValue(int value){
		if(value == 1){
			WinningValue.text = "Nothing is said";
		}else if (value == 2) {
			WinningValue.text = "DAVI is said";
		} else if (value == 3) {
			WinningValue.text = "CE is said";
		} else if (value == 4) {
			WinningValue.text = "CHARI is said";
		} else if (value == 5) {
			WinningValue.text = "FANJI is said";
		} else if (value == 6) {
			WinningValue.text = "SHASHI is said";
		} else {
			WinningValue.text = value + " is said";
		}


	}



}
