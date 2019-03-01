using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour {

	public GameController table;

	private Camera cam;
	private Rigidbody rigidBody;
	private float currentRotX = 0;
	//private Rigidbody rigidBody;
	// Use this for initialization

	public float InitialFieldOfView = 60;
	public float zoomedFieldOfView = 30;
	public float zoomSpeed = 10;
	public float clipSize = 0.01f;
	void Start () {
		rigidBody = gameObject.GetComponent<Rigidbody> ();
		cam = transform.Find ("Camera").GetComponent<Camera> ();
		cam.fieldOfView = InitialFieldOfView;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(table.paused){
			return;
		}
		float rotY = Input.GetAxisRaw("Mouse X");
		float rotX = Input.GetAxisRaw("Mouse Y");

		Vector3 _rotation = new Vector3(0, rotY, 0f) * 2f;


		rigidBody.MoveRotation (rigidBody.rotation * Quaternion.Euler(_rotation));

		//currentCameraRotationX -= cameraRotationX;

		currentRotX -= rotX;
		currentRotX = Mathf.Clamp(currentRotX, -40, 60);
		//Apply our rotation to the transform of our camera
		cam.transform.localEulerAngles = new Vector3(currentRotX, 0f, 0f) * 2f;
	}

	void Update(){

		if(table.paused){
			return;
		}


		if (Input.GetKey (KeyCode.Mouse1)) {
			if(cam.fieldOfView > zoomedFieldOfView){
				if (cam.fieldOfView - zoomedFieldOfView < clipSize) {
					cam.fieldOfView = zoomedFieldOfView;
				}else{
					cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,zoomedFieldOfView,zoomSpeed * Time.deltaTime);
				}

			}
		} else {
			if(cam.fieldOfView < InitialFieldOfView){
				if (InitialFieldOfView - cam.fieldOfView < clipSize) {
					cam.fieldOfView = InitialFieldOfView;
				} else {
					cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,InitialFieldOfView,zoomSpeed * Time.deltaTime);
				}

			}
		}
	}

}
