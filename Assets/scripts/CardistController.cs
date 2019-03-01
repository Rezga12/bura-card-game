using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class CardistController : NetworkBehaviour {

	private PlayerHandController player;

	private GameController table;
	private CardPlacements placements;


	private GameFlow flow;
	private StateController state;

	public MessageController messanger;

	// Use this for initialization
	void Start () {

		player = GetComponent<PlayerHandController> ();
		table = GameObject.Find ("Table").GetComponent<GameController> ();
		messanger = GetComponent<MessageController> ();
		placements = table.GetComponent<CardPlacements> ();

		cam = GameObject.Find ("Camera").GetComponent<Camera> ();
		state = gameObject.GetComponent<StateController> ();
		if (isServer) {
			
			flow = GameObject.Find ("Game Flow Watcher").GetComponent<GameFlow>();
			if (isLocalPlayer) {
				flow.Player1 = gameObject.GetComponent<PlayerHandController> ();
				flow.cardist1 = this;
			} else {
				flow.Player2 = gameObject.GetComponent<PlayerHandController> ();
				flow.cardist2 = this;
			}
		} 
		if (isLocalPlayer) {
			table.xControll.player1 = this;
		} else {
			table.xControll.player2 = this;
		}


	}



	private Camera cam;

	private int selectedIndex;
	private bool selected;
	public int maximumCardsInHand = 5;

	void Update () {

		if(!isLocalPlayer || table.paused){
			return;
		}



		
		bool cardsFinishedAnimation =  cardsFinishedAnimating();
		if(!cardsFinishedAnimation){
			if (player.selected) {
				player.toggleSelected ();
			}
		}

		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			player.highlightNext ();
		}

		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			player.highlightPrevious ();
		}

		if(cardsFinishedAnimation && Input.GetKeyDown(KeyCode.Space)){
			
			player.toggleSelected ();
		}

		if(Input.GetKeyDown(KeyCode.RightArrow)){
			player.switchWithRight ();
		}

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			player.switchWithLeft ();
		}

		if(table.attackMode && !table.waitingForAnswer){

			if (table.GetComponent<Timer> ().remainingTime <= 0) {
				if (placements.numAttackCards == 0) {
					forfeitRound ();
					removeKozir ();
					messanger.onTakingCards (true,messanger.box.timesUpForYou,messanger.box.timesUpForOpponent);
				} else {
					state.AttackComplete ();
					if(player.selected)
						player.toggleSelected ();
					GetComponent<TimerController> ().startTimer();
					messanger.DisplayTurn (false);
					messanger.onTakingCards (true,messanger.box.timesUpForYou,messanger.box.timesUpForOpponent);
				}
			}

			if (placements.numAttackCards == 0 && Input.GetKeyDown (KeyCode.F) && table.canRaise) {
				offerRaise ();
				table.canRaise = false;
				table.waitingForAnswer = true;
				GetComponent<TimerController> ().startTimer ();
				messanger.DisplayTurn (false);
				messanger.onTakingCards (true,messanger.box.youOffer,messanger.box.opponentOffers);
			}

			if(table.malute && player.malute > 0 && Input.GetKeyDown(KeyCode.D)){
				bool bura = player.malute == 2;
				attackWithAllCards ();
				if (bura) {
					finishRound ();
					player.wonWithBura ();
					GetComponent<TimerController> ().stopTimer ();
				} else {
					state.AttackComplete ();
					GetComponent<TimerController> ().startTimer ();
				}
			}

			if(player.selected && player.cardCountInHand != 0){
				if(Input.GetKeyDown(KeyCode.Mouse0)){
					
					if(placements.validPlacement(player.selectedCardKind())){
						Card card = player.popCard ();
						attackWithCard (card);
					}

				}
			}
		}

		if(table.attackMode && Input.GetKeyDown(KeyCode.Return)){
			state.AttackComplete ();
			if(player.selected)
				player.toggleSelected ();
			GetComponent<TimerController> ().startTimer();
			messanger.DisplayTurn (false);
			messanger.onTakingCards (true,messanger.box.playerFinishTurn,messanger.box.opponentFinishTurn);
		}



		if(table.defenseMode && !table.waitingForAnswer){

			if (table.GetComponent<Timer> ().remainingTime <= 0) {
				forfeitRound ();
				removeKozir ();
				messanger.onTakingCards (true,messanger.box.timesUpForYou,messanger.box.timesUpForOpponent);
			}

			if (placements.numAnswerCards == 0 && Input.GetKeyDown (KeyCode.F) && table.canRaise) {
				offerRaise ();
				table.canRaise = false;
				table.waitingForAnswer = true;
				GetComponent<TimerController> ().startTimer ();
				messanger.DisplayTurn (false);
				messanger.onTakingCards (true,messanger.box.youOffer,messanger.box.opponentOffers);
			}

			bool bura = player.malute == 2;
			if(table.malute && player.malute > 0 && placements.numAttackCards < 5 && Input.GetKeyDown(KeyCode.D)){
				table.defenseMode = false;
				if (isServer) {
					returnAttackedCards ();
				} else {
					CmdReturnAttackedCards ();
				}
				placements.ClearTableWeak ();
				attackWithAllCards ();

				if (bura) {
					finishRound ();
					player.wonWithBura ();
					GetComponent<TimerController> ().stopTimer ();
				} else {
					state.AttackComplete ();
					GetComponent<TimerController> ().startTimer ();
					messanger.onTakingCards (true,messanger.box.playerFinishTurn,messanger.box.opponentFinishTurn);
				}
			}


			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			Position pos;
			if (Physics.Raycast (ray, out hit) && (pos = hit.transform.gameObject.GetComponent<Position> ()) && !placements.cardsInAnswer[pos.position]) {
				if(selCard)
					selCard.TableUnselect ();
				
				selCard = placements.cardsInAttack [pos.position];

				if (selCard) {
					selCard.TableSelect ();
					if(player.selected && Input.GetKeyDown(KeyCode.Mouse0)){
						Card card = player.popCard ();
						AnswerWithCard(card,pos.position);
						if(placements.numAnswerCards == placements.numAttackCards){
							StartCoroutine (finishDefence());
							GetComponent<TimerController> ().stopTimer ();

						}

					}
				}
					
			} else {
				if (selCard)
					selCard.TableUnselect ();
			}
		}
	}

	private IEnumerator finishDefence(){

		yield return new WaitForSeconds (5);

		bool defSucc = placements.defenseSuccessful ();
		messanger.DisplayTurn (defSucc);
		messanger.onTakingCards (defSucc,messanger.box.youTakeText,messanger.box.opponentTakeText);
		int score = placements.getScore ();
		int win = handleScores (defSucc,score);
		//ClearTable ();
		TakeCards(defSucc ? 1:2);
		DealHand (win);
		state.DefenceComplete (defSucc);
		player.toggleSelected ();


		clearOpponentDeck ();
		if(isServer){
			RpcClearOpponent ();
		}else{
			CmdClearOpponent ();

		}


		//GetComponent<TimerController> ().startTimer ();

	}

	private void clearOpponentDeck(){
		var opponentHand = table.opponentCardPlace.transform.parent;
		for(int i=1;i<opponentHand.childCount;i++){
			Destroy(opponentHand.GetChild(i).gameObject);
		}


	}

	[Command]
	private void CmdClearOpponent(){
		clearOpponentDeck ();

	}

	[ClientRpc]
	private void RpcClearOpponent(){
		if(!isServer){
			clearOpponentDeck ();

		}
	}

	private Card selCard;

	private void offerRaise(){
		state.setOpponentOffered (true);
		if(isServer){
			RpcDaviMenuAppear ();
		}else{
			CmdDaviMenuAppear ();
		}
	}

	[Command]
	private void CmdDaviMenuAppear(){
		table.xControll.appear ();
	}

	[ClientRpc]
	private void RpcDaviMenuAppear(){
		if (isServer) {
			return;
		} else {
			table.xControll.appear ();
		}
	}

	public void forfeitRound(){
		finishRound ();
		player.lostToForfeit ();
	}

	private void finishRound(){
		GetComponent<TimerController> ().stopTimer ();
		if (isServer) {
			flow.Player1.popAllCards ();
			flow.Player2.popAllCards ();
			flow.shuffledDeck = new List<Card> ();
		} else {
			CmdpopAllCards ();
		}
		Invoke ("nextRound",3);

	}

	private void nextRound(){
		ClearTable ();
		DealHand (0);
	}

	[Command]
	private void CmdRemomeKozir(){
		placements.removeKozir ();
	}

	public void removeKozir(){
		placements.removeKozir ();
		if (!isServer) {
			CmdRemomeKozir ();
		}
	}

	[Command]
	private void CmdpopAllCards(){
		flow.Player1.popAllCards ();
		flow.Player2.popAllCards ();
		flow.shuffledDeck = new List<Card> ();
	}

	[ClientRpc]
	private void RpcPopAllCards(){
		player.popAllCards ();
	}

	private void attackWithAllCards(){
		for(int i=0;i<player.cardsInHand.Count();i++){
			Card card = player.popCard ();
			attackWithCard (card);
		}

	}

	private void returnAttackedCards(){
		
		flow.Player2.returnCardsFromTable ();
	}

	[Command]
	private void CmdReturnAttackedCards(){
		flow.Player1.returnCardsFromTable ();
	}

	public void GetAttackedWIthCard(int index){
		var card = Instantiate (table.deck.Cards[index].gameObject,table.opponentCardPlace.position,table.opponentCardPlace.rotation);
		card.transform.localScale = table.opponentCardPlace.localScale;

		placements.placeAttackCard (card.GetComponent<Card>());
	}

	[Command]
	void CmdGetAttackedWithCard(int selIndex){
		GetAttackedWIthCard (selIndex);
	}

	[ClientRpc]
	void RpcGetAttackedWithCard(int selIndex){
		if(isServer){
			return;
		}
		GetAttackedWIthCard (selIndex);
	}

	public void attackWithCard(Card card){

		if (isServer) {
			RpcGetAttackedWithCard (card.indexInDeck);
		} else {
			CmdGetAttackedWithCard (card.indexInDeck);
		}
		placements.placeAttackCard (card);

	}

	public void GetAnsweredWithCard(int index,int pos){
		var card = Instantiate (table.deck.Cards[index].gameObject,table.opponentCardPlace.position,table.opponentCardPlace.rotation);
		card.transform.localScale = table.opponentCardPlace.localScale;
		placements.placeAnswerCard (card.GetComponent<Card>(),pos);
	}

	[Command]
	void CmdGetAnsweredWithCard(int selIndex,int pos){
		GetAnsweredWithCard (selIndex,pos);
	}

	[ClientRpc]
	void RpcGetAnsweredWithCard(int selIndex, int pos){
		if(isServer){
			return;
		}
		GetAnsweredWithCard (selIndex,pos);
	}

	public void AnswerWithCard(Card card, int pos){
		if(isServer){
			RpcGetAnsweredWithCard (card.indexInDeck,pos);
		}else{
			CmdGetAnsweredWithCard (card.indexInDeck,pos);
		}
		placements.placeAnswerCard (card,pos);
	}

	[ClientRpc]
	private void RpcClear(){
		if (isServer)
			return;
		placements.ClearTable ();
	}

	[Command]
	private void CmdClear(){
		placements.ClearTable ();
	}

	private void ClearTable(){
		placements.ClearTable ();

		if(isServer){
			RpcClear ();

		}else{
			CmdClear ();
		}
	}


	private void ClearTableWeak(){
		if(isServer){
			RpcClearTableWeak ();
		}else{
			CmdClearTableWeak ();
		}
	}

	[Command]
	private void CmdClearTableWeak(){
		placements.ClearTableWeak ();
	}
	[ClientRpc]
	private void RpcClearTableWeak(){
		if(!isServer){
			placements.ClearTableWeak ();
		}
	}

	private void TakeCards(int player){
		placements.TakeCards (player);
		if(isServer){
			RpcTakeCards (player % 2 + 1);
		}else{
			CmdTakeCards (player % 2 + 1);
		}
	}

	[Command]
	private void CmdTakeCards(int player){
		placements.TakeCards (player);
	}

	[ClientRpc]
	private void RpcTakeCards(int player){
		if(!isServer)
			placements.TakeCards (player);
	}

	private void DealHand(int winner){
		//Debug.Log ("gamoidzaxa Deal hand");
		if (isServer) {
			flow.DealHand (winner);
		} else {
			CmdDealHand (winner);
		}
	}

	[Command]
	private void CmdDealHand(int winner){
		flow.DealHand (winner);
	}

	[Command]
	private void CmdSetScore(int pl,int score){
		if (pl == 1) {
			table.playerOneScore += score;
		} else {
			table.playerTwoScore += score;
		}
	}

	private int handleScores(bool defSucc,int score){
		int winner = 1;
		if (defSucc) {
			if (isServer) {
				table.playerOneScore += score;
			} else {
				winner = 2;
				CmdSetScore (2,score);
			}
		} else {
			if (isServer) {
				winner = 2;
				table.playerTwoScore += score;
			} else {
				CmdSetScore (1,score);
			}
		}
		return winner;

	}

	public void riseRoundValue(){
		if (isServer) {
			flow.winningValue++;
			messanger.setWinningValue (flow.winningValue);
		} else {
			CmdRiseRoundValue ();
		}
	}

	[Command]
	public void CmdRiseRoundValue(){
		flow.winningValue++;
		messanger.setWinningValue (flow.winningValue);
	}


	public void setCanRaise(bool val){
		if (isServer) {
			RpcSetCanRaise (val);
		} else {
			CmdSetCanRaise (val);
		}
	}

	public void setWaitingForAnswer(bool val){
		if (isServer) {
			RpcSetWaitingForAnswer (val);
		} else {
			CmdSetWaitingForAnswer (val);
		}
	}

	public void refreshRaisingStates(){
		table.waitingForAnswer = false;
		table.canRaise = true;
		if (isServer) {
			RpcSetWaitingForAnswer (false);
			RpcSetCanRaise (true);
		} else {
			CmdSetWaitingForAnswer (false);
			CmdSetCanRaise (true);
		}

	}

	[Command]
	private	 void CmdSetWaitingForAnswer(bool val){
		table.waitingForAnswer = val;
	}

	[ClientRpc]
	private void RpcSetWaitingForAnswer(bool val){
		if (isServer)
			return;
		table.waitingForAnswer = val;
	}

	[Command]
	private void CmdSetCanRaise(bool val){
		if (isServer)
			return;
		table.canRaise = val;
	}

	[ClientRpc]
	private void RpcSetCanRaise(bool val){
		table.canRaise = val;
	}

	public void revertDeck(){
		table.deck.revert ();
		placements.cardAboveDeck = table.deck.getAboveCard ();

		RpcRevertDeck ();

	}

	[ClientRpc]
	public void RpcRevertDeck(){
		if(!isServer){
			table.deck.revert ();
			placements.cardAboveDeck = table.deck.getAboveCard ();
		}
	}

	private bool cardsFinishedAnimating(){
		for(int i=0;i<player.cardCountInHand;i++){
			if (player.cardsInHand [i].animate) {
				return false;
			}
		}
		return true;
	}



}
