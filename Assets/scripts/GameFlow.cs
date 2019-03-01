using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.SceneManagement;


public class GameFlow : NetworkBehaviour {

	public int starter;
	public GameOverManager gameOver;
	private MyNetworkManager manager;

	public GameController table;
	public List<Card> shuffledDeck;


	public CardistController cardist1;
	public CardistController cardist2;

	public PlayerHandController Player1;
	public PlayerHandController Player2;

	private StateController state;

	public CardPlacements placements;
	public int winningValue;

	public int gameGoal;
	public List<Card> dummyShuffle;

	public float delayInCardDealing = 0.5f;

	private int deckCount;

	void Awake(){
		//.OrderBy (x => Random.Range (0, 1000))
		shuffledDeck = table.deck.Cards.ToList();//.GetRange(0,12);//dummyShuffle.ToList ();//table.Deck.OrderBy (x => Random.Range (0, 1000)).ToList ();
		deckCount = shuffledDeck.Count;
		manager = GameObject.Find ("Network Manager").GetComponent<MyNetworkManager>();
		starter = 1;
	}

	// Use this for initialization
	void Start () {
		
	}


	void pickFromDeck(PlayerHandController player){
		if(shuffledDeck.Count == 1){
			player.GivePlayerCard (-1);
		}else{
			player.GivePlayerCard (shuffledDeck[0].GetComponent<Card>().indexInDeck);
		}

		shuffledDeck.RemoveAt (0);
	}


	public bool round;
	// Update is called once per frame
	void Update () {
		
		if(!state && Player1){
			state = Player1.gameObject.GetComponent<StateController> ();
		}

		if(Player1 && Player2 && !round){
			Invoke ("startRound",3);
			round = true;
			table.malute = manager.malute;

		}
	}



	public void DealHand(int winner){

		if(Player1.cardCountInHand == 0 && Player2.cardCountInHand == 0 && shuffledDeck.Count == 0){

			cardist1.revertDeck ();

			shuffledDeck = table.deck.Cards.OrderBy (x => Random.Range (0, 1000)).ToList ();
			Player1.ClearTakenCards ();
			deckCount = shuffledDeck.Count;
			round = false;


			if (Player2.forfeit || Player1.bura || table.playerOneScore > table.playerTwoScore && !(Player2.bura || Player1.forfeit)) {
				Player2.forfeit = false;
				Player1.bura = false;
				table.playerOneFinalScore += winningValue;
				cardist1.messanger.updateScoreTable ();
				table.playerOneScore = 0;
				table.playerTwoScore = 0;
				state.startNewRound (true);
				starter = 1;
				cardist1.refreshRaisingStates ();
				cardist1.messanger.displayWinInfo (true);
				winningValue = 1;
				cardist1.messanger.setWinningValue (winningValue);

			} else if (Player1.forfeit || Player2.bura || table.playerOneScore < table.playerTwoScore && !(Player1.bura || Player2.forfeit)) {
				Player2.bura = false;
				Player1.forfeit = false;
				table.playerTwoFinalScore += winningValue;
				cardist1.messanger.updateScoreTable ();
				table.playerTwoScore = 0;
				table.playerOneScore = 0;
				state.startNewRound (false);
				starter = 2;
				cardist1.refreshRaisingStates ();
				cardist1.messanger.displayWinInfo (false);
				winningValue = 1;
				cardist1.messanger.setWinningValue (winningValue);
			} else {
				//draw
				table.playerTwoScore = 0;
				table.playerOneScore = 0;
				state.startNewRound (false);
			}

			if (table.playerTwoFinalScore >= gameGoal || table.playerOneFinalScore >= gameGoal) {
				gameOver.GameOver (starter == 1);
				round = true;
			}
			return;
		}else if(shuffledDeck.Count == 0){
			return;
		}

		var p1 = Player1;
		var p2 = Player2;
		if (winner == 2) {
			p1 = Player2;
			p2 = Player1;
			cardist1.messanger.DisplayTurn (false);
		} else {
			cardist1.messanger.DisplayTurn (true);
		}



		for(int i=Player1.cardCountInHand;i<5;i++){
			if (deckCount == 0) {
				return;
			}
			var coroutineOne = dealCard(p1,2 * (i - Player1.cardCountInHand) * delayInCardDealing);
			var coroutineTwo = dealCard (p2,(2 * (i - Player1.cardCountInHand) + 1) * delayInCardDealing);
			StartCoroutine(coroutineOne);
			StartCoroutine (coroutineTwo);

			deckCount-=2;

		}
	}

	private IEnumerator dealCard(PlayerHandController p, float delay){
		yield return new WaitForSeconds (delay);
		pickFromDeck (p);
		if(p.cardCountInHand == 5 || shuffledDeck.Count == 0){
			Player1.GetComponent<TimerController> ().startTimer ();
		}
	}

	private void startRound(){
		var card = shuffledDeck [shuffledDeck.Count - 1];
		placements.placeKozir (card.indexInDeck);
		placements.RpcPlaceKozir (card.indexInDeck);
		DealHand (starter);


	}

	private void updateScoreTable(){

	}





}
