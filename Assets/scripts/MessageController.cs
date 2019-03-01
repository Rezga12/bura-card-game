using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MessageController : NetworkBehaviour {

	public MessageBox box;
	GameController table;
	// Use this for initialization
	void Start () {
		box = GameObject.Find ("Table").GetComponent<MessageBox> ();
		table = box.GetComponent<GameController> ();
	}

	public void DisplayTurn(bool my){
		box.Turn (my);
		if(isServer){
			RpcDisplayTurn (!my);
		}else{
			CmdDisplayTurn (!my);
		}
	}

	[Command]
	public void CmdDisplayTurn(bool my){
		if (!box) {
			box = GameObject.Find ("Table").GetComponent<MessageBox> ();
		}
		box.Turn (my);
	}

	[ClientRpc]
	public void RpcDisplayTurn(bool my){
		if(!isServer){
			if (!box) {
				box = GameObject.Find ("Table").GetComponent<MessageBox> ();
			}
			box.Turn (my);
		}
	}



	public void onTakingCards(bool win,string playerPhrase, string opponentPhrase){

		if(isServer){
			if (win) {
				box.addInfo (playerPhrase);
				RpcOnTakingCards (opponentPhrase);
			} else {
				box.addInfo (opponentPhrase);
				RpcOnTakingCards (playerPhrase);
			}
		}else{
			if (win) {
				box.addInfo (playerPhrase);
				CmdOnTakingCards (opponentPhrase);
			} else {
				box.addInfo (opponentPhrase);
				CmdOnTakingCards (playerPhrase);
			}
		}
	}

	[Command]
	private void CmdOnTakingCards(string text){
		box.addInfo (text);

	}

	[ClientRpc]
	private void RpcOnTakingCards(string text){
		if(!isServer)
			box.addInfo (text);

	}

	public void displayWinInfo(bool win){

		box.addWinInfo (win);
		RpcDisplayWinInfo (!win);
	}

	[ClientRpc]
	public void RpcDisplayWinInfo(bool win){
		if(!isServer)
			box.addWinInfo (win);
	}

	public void updateScoreTable(){
		box.yourScore.text = "" + table.playerOneFinalScore;
		box.opponentsScore.text = "" + table.playerTwoFinalScore;
		RpcUpdateScoreTable (table.playerTwoFinalScore,table.playerOneFinalScore);
	}

	[ClientRpc]
	private void RpcUpdateScoreTable(int a, int b){
		if (!isServer) {
			box.yourScore.text = "" +  a;
			box.opponentsScore.text = "" + b;
		}
	}

	public void setWinningValue(int value){
		
		RpcSetWinningValue (value);
	}
		

	[ClientRpc]
	void RpcSetWinningValue(int value){
		box.setWinningValue (value);
	}

}
