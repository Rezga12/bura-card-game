using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
	

	public Material material;

	public int indexInDeck;

	public int points;
	public int kind;
	public int power;

	public bool selected;
	// Use this for initialization

	//public Transform parent;
	public Transform finish;
	public bool animate = false;

	public float speed;
	private float actualSpeed;
	void Start () {
		if(material)
			material.SetInt ("_Default",1);
		
	}
	
	// Update is called once per frame
	void Update () {
		if(animate){
			if (transform.parent == null) {
				//transform.SetParent (parent);
			}
			if (weakAnim) {
				transform.position = Vector3.Lerp (transform.position, finishPosition, speed * Time.deltaTime);
				transform.rotation = Quaternion.Lerp (transform.rotation, finishRotation, speed * Time.deltaTime);
				transform.localScale = Vector3.Lerp (transform.localScale, finishScale, speed * Time.deltaTime);
			} else {
				transform.position = Vector3.Lerp (transform.position, finish.position, speed * Time.deltaTime);
				transform.rotation = Quaternion.Lerp (transform.rotation, finish.rotation, speed * Time.deltaTime);
				transform.localScale = Vector3.Lerp (transform.localScale, finish.localScale, speed * Time.deltaTime);
			}
			float dist = Vector3.Distance (transform.position, finish.position);




			if(dist < 0.01f){
				if (weakAnim) {
					transform.position = finishPosition;
					transform.rotation = finishRotation;
					transform.localScale = finishScale;
				} else {
					transform.position = finish.position;
					transform.rotation = finish.rotation;
					transform.localScale = finish.localScale;
				}
				animate = false;

			}
		}
	}


	public void Select(){
		if(selected){
			return;
		}
		material.SetInt ("_Default",0);
		gameObject.transform.localScale = gameObject.transform.localScale * 1.2f;
		var pos = gameObject.transform.localPosition;
		pos.y += 0.1f;
		gameObject.transform.localPosition = pos;
		selected = true;
	}

	public void Unselect(){
		if(!selected){
			return;
		}
		var pos = gameObject.transform.localPosition;
		pos.y -= 0.1f;
		gameObject.transform.localPosition = pos;
		material.SetInt ("_Default",1);
		gameObject.transform.localScale = gameObject.transform.localScale / 1.2f;

		selected = false;
	}

	public void TableSelect(){
		if(selected){
			return;
		}
		material.SetInt ("_Default",0);
		selected = true;
	}

	public void TableUnselect(){
		if(!selected){
			return;
		}
		material.SetInt ("_Default",1);

		selected = false;
	}

	private Vector3 finishPosition;
	private Quaternion finishRotation;
	private Vector3 finishScale;


	public void activateAnimation(int animationSpeed){
		speed = animationSpeed;

		finishPosition = finish.position;
		finishRotation = finish.rotation;
		finishScale = finish.localScale;

		animate = true;
	}

	private bool weakAnim;

	public void activateAnimation(int animationSpeed,bool anim){

		weakAnim = anim;

		speed = animationSpeed;

		finishPosition = finish.position;
		finishRotation = finish.rotation;
		finishScale = finish.localScale;

		animate = true;

	}

}
