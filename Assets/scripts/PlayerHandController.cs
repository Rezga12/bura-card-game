using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHandController : NetworkBehaviour {

	private GameController table;
	public Card[] cardsInHand;
	private CardPlacements placements;

	public int roundScore;

	public int malute;
	private int[] kinds;


	public bool bura;
	public bool forfeit;
	// Use this for initialization
	void Start () {
		table = GameObject.Find ("Table").GetComponent<GameController> ();
		placements = table.GetComponent<CardPlacements> ();
		cardsInHand = new Card[5];
		cardCountInHand = 0;

		kinds = new int[4];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GivePlayerCard(int index){
		
		if (isLocalPlayer) {
			RpcGiveOpponentCard (index,-1);
			giveCardByIndex (index,cardCountInHand,null);
		} else {
			CmdGiveOpponentCard (index,-1);
			RpcGiveCardByIndex (index,cardCountInHand);
		}

	}

	public void GivePlayerCard(Card card,int indexInAttack){
		giveCardByIndex (0, cardCountInHand, card.gameObject);

		if (isServer) {
			RpcGiveOpponentCard (card.indexInDeck,indexInAttack);

		} else {
			CmdGiveOpponentCard (card.indexInDeck,indexInAttack);
		}

	}

	[Command]
	void CmdGiveOpponentCard (int index,int indexInAttack){
		
		giveOppoentCard (index,indexInAttack);
	}

	[ClientRpc]
	void RpcGiveOpponentCard(int index,int indexInAttack){
		if(isServer){
			return;
		}
		giveOppoentCard (index,indexInAttack);
	}

	void giveOppoentCard(int index,int indexInAttack){

		table.audioManager.playPickFromDeck ();

		if(index == -1){
			cardsInHand [cardCountInHand] = placements.kozir;
		}else{
			cardsInHand [cardCountInHand] = table.deck.Cards [index];
		}

		//Debug.Log (cardsInHand [cardCountInHand]);
		if (indexInAttack != -1) {
			//var card = placements.cardsInAttack [indexInAttack];
			//card.finish = table.opponentCardPlace;
			//card.GetComponent<Card> ().activateAnimation (1);
		} else {
			GameObject card;
			if (index == -1) {
				card = placements.kozir.gameObject;
				card.transform.localScale = placements.cardAboveDeck.localScale;
				card.transform.SetParent (table.opponentCardPlace.transform.parent);
			} else {
				card = Instantiate (cardsInHand [cardCountInHand].gameObject,placements.cardAboveDeck.position, placements.cardAboveDeck.rotation,table.opponentCardPlace.transform.parent);
				table.deck.removeCard ();
				placements.cardAboveDeck = table.deck.getAboveCard ();
				card.transform.localScale = placements.cardAboveDeck.localScale;
			}



			card.GetComponent<Card> ().finish = table.opponentCardPlace;
			card.GetComponent<Card> ().activateAnimation (1);
		}
		cardCountInHand++;
	}




	[Command]
	public void CmdGivePlayerCard(int index){
		GivePlayerCard (index);
	}

	void giveCardByIndex(int index, int handPos,GameObject card){

		table.audioManager.playPickFromDeck ();

		if (!card) {
			if (index == -1) {
				card = placements.kozir.gameObject;
				card.transform.SetParent (placements.handPosition);
			} else {
				card = Instantiate (table.deck.Cards [index].gameObject, placements.cardAboveDeck.position, placements.cardAboveDeck.rotation);
				table.deck.removeCard ();
				placements.cardAboveDeck = table.deck.getAboveCard ();
				card.transform.SetParent (placements.handPosition);
				card.transform.localScale = placements.cardAboveDeck.localScale * 2;
			}


		}

		card.transform.SetParent (placements.handPosition);
		cardsInHand [cardCountInHand] = card.GetComponent<Card> ();
		placements.placeCardInHand (cardsInHand [cardCountInHand],cardCountInHand,true);
		cardCountInHand++;

		if(++kinds [card.GetComponent<Card>().kind] == 5){
			malute = 1;
			if (cardsInHand [0].kind == placements.kozir.kind) {
				malute = 2;
			}
		}
	}

	[ClientRpc]
	void RpcGiveCardByIndex(int index,int handPos){
		if(isServer){
			return;
		}
		giveCardByIndex (index,handPos,null);


	}


	public bool selected;
	private int selectedIndex;

	public int cardCountInHand;
	public int maximumCardsInHand = 5;



	public void highlightNext(){
		if (selected) {
			cardsInHand [selectedIndex].Unselect ();
			selectedIndex--;
			selectedIndex = (selectedIndex + cardCountInHand) % cardCountInHand;
			cardsInHand [selectedIndex].Select ();
		}

	}

	public void highlightPrevious(){
		if (selected) {
			cardsInHand [selectedIndex].Unselect ();
			selectedIndex++;
			selectedIndex = (selectedIndex + cardCountInHand) % cardCountInHand;
			cardsInHand [selectedIndex].Select ();
		}
	}

	public void toggleSelected(){
		if (cardCountInHand == 0) {
			selected = false;
			return;
		}
		if (!selected) {
			cardsInHand [selectedIndex].Select ();
			selected = true;
		} else {
			cardsInHand [selectedIndex].Unselect ();
			selected = false;
		}

	}

	public void popAllCards(){
		if (!isLocalPlayer) {
			if (isServer) {
				RpcPopAll ();
			} else {
				CmdPopAll ();
			}
		} else {
			while(cardCountInHand > 0){
				GameObject.Destroy(popCard ().gameObject);
			}
		}

	}

	[Command]
	private void CmdPopAll(){
		popAllCards ();
	}

	[ClientRpc]
	private void RpcPopAll(){
		if (isServer)
			return;
		popAllCards ();
	}

	public Card popCard(){
		var resultCard = cardsInHand[selectedIndex];
		kinds [resultCard.kind]--;
		malute = 0;
		resultCard.Unselect ();
		cardsInHand [selectedIndex] = null;

		for(int i=selectedIndex+1;i<maximumCardsInHand;i++){
			if(cardsInHand [i])
				placements.placeCardInHand (cardsInHand [i], i - 1,false);
			
			var temp = cardsInHand [i - 1];
			cardsInHand [i - 1] = cardsInHand [i];
			cardsInHand [i] = temp;

		}
		cardCountInHand--;
		if (isServer) {
			RpcsynchCount (cardCountInHand);
		} else {
			CmdsynchCount (cardCountInHand);
		}
		selectedIndex = selectedIndex == 0 ? selectedIndex : selectedIndex - 1;

		if(cardCountInHand > 0){
			cardsInHand [selectedIndex].Select ();
		}

		return resultCard;
	}

	[Command]
	void CmdsynchCount(int count){
		cardCountInHand = count;
	}
	[ClientRpc]
	void RpcsynchCount(int count){
		if(isServer){
			return;
		}
		cardCountInHand = count;
	}

	public int selectedCardKind(){
		return cardsInHand [selectedIndex].kind ;
	}

	public void returnCardsFromTable(){

		for (int i=0;i<placements.numAttackCards;i++) {
			GivePlayerCard (placements.cardsInAttack[i],i);
			placements.cardsInAttack [i] = null;
		}
		if(malute == 1){
			malute = 0;
		}

		placements.numAttackCards = 0;
		//table.defenseMode = false;
	}

	public void wonWithBura(){
		if (isServer) {
			bura = true;
		} else {
			CmdWonWithBura ();
		}
	}

	[Command]
	private void CmdWonWithBura(){
		bura = true;
	}

	public void lostToForfeit(){
		if (isServer) {
			forfeit = true;
		} else {
			CmdLostToForfeit();
		}
	}
	[Command]
	private void CmdLostToForfeit(){
		forfeit = true;
	}


	public void switchWithLeft(){
		if (!selected || selectedIndex == 0) {
			return;
		}

		switcher (-1);
	}

	public void switchWithRight(){
		if (!selected || selectedIndex >= cardCountInHand - 1) {
			return;
		}

		switcher (1);
	}

	private void switcher(int distance){
		cardsInHand [selectedIndex].Unselect ();

		var pos = cardsInHand [selectedIndex].transform.position;
		var rot = cardsInHand [selectedIndex].transform.rotation;

		cardsInHand [selectedIndex].transform.position = cardsInHand [selectedIndex + distance].transform.position;
		cardsInHand [selectedIndex].transform.rotation = cardsInHand [selectedIndex + distance].transform.rotation;

		cardsInHand [selectedIndex + distance].transform.position = pos;
		cardsInHand [selectedIndex + distance].transform.rotation = rot;

		switchInArray (selectedIndex, selectedIndex + distance);
		if(isServer){
			RpcSwitchInArray (selectedIndex, selectedIndex + distance);
		}else{
			CmdSwitchInArray (selectedIndex,selectedIndex + distance);
		}

		selectedIndex+=distance;
		cardsInHand [selectedIndex].Select ();
	}

	private void switchInArray(int indexOne, int indexTwo){
		var tempCard = cardsInHand [indexOne];
		cardsInHand[indexOne] = cardsInHand[indexTwo];
		cardsInHand [indexTwo] = tempCard;
	}

	[Command]
	private void CmdSwitchInArray(int indexOne, int indexTwo){
		switchInArray (indexOne,indexTwo);
	}
	[ClientRpc]
	private void RpcSwitchInArray(int indexOne, int indexTwo){
		if(isServer){
			return;
		}
		switchInArray (indexOne,indexTwo);
	}

	public void ClearTakenCards(){
		placements.clearTakenCards ();
		RpcClearTakenCards ();
	}

	[ClientRpc]
	private void RpcClearTakenCards(){

		if(!isServer)
			placements.clearTakenCards ();
	}

}
