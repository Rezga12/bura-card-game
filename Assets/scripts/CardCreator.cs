using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCreator : MonoBehaviour {

	public Transform LeftCard;
	public Transform RightCard;

	public Transform backLeftCard;
	public Transform backRightCard;

	public float distance = 1;

	public int cardNum;
	private CameraMovement movements;
	// Use this for initialization
	void Start () {
		movements = GetComponent<CameraMovement>();
		createAllCards ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void createCard(){
		
		create (LeftCard,false);
		create (RightCard,false);
		create (backRightCard,true);
		create (backLeftCard,true);

		cardNum++;
	}

	public void createAllCards(){
		for(int i=0;i<40;i++){
			createCard ();
		}
	}

	public void adjustPosition(Transform trans){
		trans.position +=  new Vector3 (distance * cardNum,0,0); 
	}

	public void create(Transform trans,bool back){

		var card = Instantiate (trans);
		adjustPosition (card.transform);
		card = card.transform.GetChild (0);
		if(!back){
			card.GetComponent<MeshRenderer>().material = movements.matList[Random.Range(0,36)];
		}
	}
}
