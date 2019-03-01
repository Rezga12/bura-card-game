using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TimerController : NetworkBehaviour {

	Timer timer;

	// Use this for initialization
	void Start () {
		timer = GameObject.Find ("Table").GetComponent<Timer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[ClientRpc]
	private void RpcStartTImer(){
		if(!isServer)
			timer.startTimer ();
	}

	[Command]
	private void CmdStartTimer(){
		timer.startTimer ();
	}

	public void startTimer(){
		timer.startTimer ();
		if(isServer){
			RpcStartTImer ();
		}else{
			CmdStartTimer ();
		}
	}

	public void stopTimer(){
		timer.stopTimer ();
		if(isServer){
			RpcStopTimer ();
		}else{
			CmdStopTimer ();
		}
	}

	[ClientRpc]
	private void RpcStopTimer(){
		if(!isServer)
			timer.stopTimer ();
	}

	[Command]
	private void CmdStopTimer(){
		timer.stopTimer ();
	}


}
