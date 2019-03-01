using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	// Use this for initialization

	public List<Material> matList;

	public Vector3 increaseValues = new Vector3(0.01f, 0,0);
	public Transform leftEdge;
	public Transform RightEdge;

	private Vector3 targetPos;

	public Vector3 speed = new Vector3 (0,0,0);
	public float changeDirectionAt = 0.1f;

	private CardCreator creator;

	void Start () {
		creator = GetComponent<CardCreator> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(speed * Time.deltaTime);

	}

	public Vector3 MarbleOffset;

	void OnTriggerEnter(Collider col){
		if (col.tag == "Marble") {
			col.transform.parent.position += MarbleOffset;


		} else {
			creator.adjustPosition (col.transform);
			if (col.tag != "Back") {
				col.transform.GetChild (0).GetComponent<MeshRenderer> ().material = matList [Random.Range (0, 36)];
			}
		}


	}

	private IEnumerator createCard(){
		yield return new WaitForSeconds (0.5f);
		GetComponent<CardCreator> ().createCard ();
	}
}
