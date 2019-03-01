using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;


public class GameController : NetworkBehaviour {

	public bool paused;

	public AudioManager audioManager;

	public float timeForTurn;

	public bool waitingForAnswer;

	public bool canRaise;

	public GameObject pauseMenu;
	public Button surrButton;

	public XController xControll;

	public Transform attackPosition;
	public Transform answerPosition;
	public Transform handPosition;

	public Material shiningCardMaterial;
	public Material standartCardMaterial;


	public DeckController deck;

	//Card Positions In Hand
	public Transform[] cardPositions;
	private int nextHandIndex;

	[SyncVar(hook="onMaluteChange")]
	public bool malute;

	public bool attackMode;
	public bool defenseMode;
	public bool offeredMode;

	[SyncVar(hook = "OnplayerOne")]
	public int playerOneScore;
	[SyncVar(hook = "OnplayerTwo")]
	public int playerTwoScore;

	[SyncVar(hook = "OnFinalOne")]
	public int playerOneFinalScore;
	[SyncVar(hook = "OnFinalTwo")]
	public int playerTwoFinalScore;

	public Transform opponentCardPlace;

	private void OnplayerOne(int score){
		playerOneScore = score;
	}
	private void OnplayerTwo(int score){
		playerTwoScore = score;
	}

	private void OnFinalOne(int score){
		playerOneFinalScore = score;
	}

	private void OnFinalTwo(int score){
		playerTwoFinalScore = score;
	}

	private void onMaluteChange(bool mal){
		malute = mal;
	}



	void Start(){
		canRaise = true;

	}



}
