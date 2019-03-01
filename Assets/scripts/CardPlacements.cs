using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CardPlacements : NetworkBehaviour {

	private GameController table;

	public Transform attackPosition;
	public Transform answerPosition;
	public Transform handPosition;

	public List<Transform> attackPositions;
	public List<Transform> answerPositions;
	public List<Transform> handPositions;

	public Transform cardAboveDeck;

	public Card[] cardsInAnswer;
	public Card[] cardsInAttack;


	public int numAttackCards;
	public int numAnswerCards;


	public Transform kozirTransform;
	public Card kozir;

	public Transform[] TakenCardsPositions;
	public Transform[] TakenCardsPositionDummies;

	// Use this for initialization
	void Start () {
		cardsInAttack = new Card[5];
		cardsInAnswer = new Card[5];

		attackPositions = new List<Transform>();
		answerPositions = new List<Transform>();
		handPositions = new List<Transform>();


		for(int i=0;i<attackPosition.childCount;i++){
			
			attackPositions.Add (attackPosition.GetChild(i));

			answerPositions.Add (answerPosition.GetChild(i));
			handPositions.Add (handPosition.GetChild(i));
		}

		table = gameObject.GetComponent<GameController> ();

		cardAboveDeck = table.deck.getAboveCard ();


	}



	public void placeCardInHand(Card card,int handPos,bool animation){
		//card.transform.SetParent (handPosition);

		if (animation) {
			card.finish = handPositions [handPos];

			card.activateAnimation (2);
		}else{
			card.transform.position = handPositions [handPos].position;
			card.transform.rotation = handPositions [handPos].rotation;
			card.transform.localScale = handPositions [handPos].localScale;
		}




	}

	public void placeAttackCard(Card card){

		card.transform.SetParent (attackPosition);
		card.finish = attackPositions [numAttackCards];
		//card.transform.position = attackPositions [numAttackCards].position;
		//card.transform.rotation = attackPositions [numAttackCards].rotation;
		//card.transform.localScale = attackPositions [numAttackCards].localScale;

		card.activateAnimation (3);

		cardsInAttack [numAttackCards] = card;
		numAttackCards++;
		table.audioManager.playCardAttacking ();
	}

	public void placeAnswerCard(Card card, int index){

		card.transform.SetParent (answerPosition);
		card.finish = answerPositions [index];
		//card.transform.position = answerPositions [index].position;
		//card.transform.rotation = answerPositions [index].rotation;
		//card.transform.localScale = answerPositions [index].localScale;
		card.activateAnimation (3);
		cardsInAnswer [index] = card;

		numAnswerCards++;
		table.audioManager.playCardAnswering();
	}

	public void ClearTableWeak(){
		for(int i=0;i<numAttackCards;i++){
			if (cardsInAttack[i]) {
				cardsInAttack [i].transform.parent = null;
				cardsInAttack [i].finish = table.opponentCardPlace;
				cardsInAttack [i].activateAnimation (1);
				cardsInAttack [i] = null;
			} 
		}

		numAttackCards = 0;
	}

	public void ClearTable(){
		
		foreach(Card card in cardsInAttack){
			if (card) {
				Destroy(card.gameObject);
			}
		}
		foreach(Card card in cardsInAnswer){
			if (card) {
				Destroy(card.gameObject);
			}
		}

		numAttackCards = 0;
		numAnswerCards = 0;
	}

	public float takenCardsOffset;

	public void TakeCards(int player){
		player--;

		for (int i = 0; i <numAttackCards; i++) {
			cardsInAttack [i].transform.SetParent (TakenCardsPositions[player].parent);
			cardsInAnswer [i].transform.SetParent (TakenCardsPositions[player].parent);

			var yOffset = (TakenCardsPositions [player].transform.parent.childCount - 2);
			TakenCardsPositionDummies [player].position = TakenCardsPositions[player].position + new Vector3(Random.Range(-0.5f,0.5f),takenCardsOffset * yOffset,Random.Range(-0.25f,0.25f));
			TakenCardsPositionDummies [player].rotation = Quaternion.Euler (new Vector3(90,0,Random.Range(0,360)));

			cardsInAttack [i].finish = TakenCardsPositionDummies [player];
			cardsInAttack [i].activateAnimation (3,true);

			TakenCardsPositionDummies [player].position = TakenCardsPositions[player].position + new Vector3(Random.Range(-0.5f,0.5f),takenCardsOffset *(yOffset+1),Random.Range(-0.25f,0.25f));
			TakenCardsPositionDummies [player].rotation = Quaternion.Euler (new Vector3(90,0,Random.Range(0,360)));
			cardsInAnswer [i].finish = TakenCardsPositionDummies [player];
			cardsInAnswer [i].activateAnimation (3,true);

			cardsInAttack [i] = null;
			cardsInAnswer [i] = null;
		}

		numAttackCards = 0;
		numAnswerCards = 0;

		table.audioManager.playCardClearing ();
	}



	public int getScore(){
		int score = 0;
		for(int i=0;i<numAnswerCards;i++){
			score += cardsInAttack [i].points + cardsInAnswer [i].points;
		}
		return score;
	}

	public bool defenseSuccessful(){

		for (int i = 0; i < numAnswerCards; i++) {
			if (cardsInAttack [i].kind == cardsInAnswer [i].kind) {
				if (cardsInAttack [i].power > cardsInAnswer [i].power) {
					return false;
				}
			} else {
				if(cardsInAnswer[i].kind != kozir.kind){
					return false;

				}
			}
		}
		return true;
	}

	public bool validPlacement(int kind){

		if(numAttackCards == 0){
			return true;
		}
		if(kind == cardsInAttack[0].kind){
			return true;
		}
		return false;
	}




	public void placeKozir(int index){
		var card = Instantiate (table.deck.Cards[index],table.deck.transform);

		card.transform.position = kozirTransform.position;
		card.transform.rotation = kozirTransform.rotation;
		card.transform.localScale = kozirTransform.localScale;

		kozir = card;

	}

	[ClientRpc]
	public void RpcPlaceKozir(int index){
		if (isServer)
			return;
		placeKozir (index);

	}

	public void removeKozir(){
		if (kozir) {
			GameObject.Destroy (kozir.gameObject);
			if(isServer)
				RpcRemoveKozir ();
		}
			
	}

	[ClientRpc]
	private void RpcRemoveKozir(){
		if (isServer)
			return;
		if(kozir)
			GameObject.Destroy (kozir.gameObject);
	}


	public void clearTakenCards(){
		var parentOne = TakenCardsPositions [0].parent;
		var parentTwo = TakenCardsPositions [1].parent;
		for(int i=2;i<parentOne.childCount;i++){
			Destroy(parentOne.GetChild(i).gameObject);
		}

		for(int i=2;i<parentTwo.childCount;i++){
			Destroy(parentTwo.GetChild(i).gameObject);
		}
	}

}
