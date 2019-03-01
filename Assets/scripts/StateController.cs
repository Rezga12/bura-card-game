using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StateController : NetworkBehaviour {


	private GameController table;
	// Use this for initialization
	void Start () {
		table = GameObject.Find ("Table").GetComponent<GameController> ();
		if(isServer)
			table.attackMode = true;


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AttackComplete(){
		table.attackMode = false;
		if(isServer){
			RpcSetDefense (true);
		}else{
			CmdSetDefense (true);
		}
	}

	public void DefenceComplete(bool IWin){
		table.defenseMode = false;
		if (isServer) {
			RpcSetAttack (!IWin);
			table.attackMode = IWin;
		} else {
			CmdSetAttack (!IWin);
			table.attackMode = IWin;
		}
	}

	[ClientRpc]
	private void RpcSetAttack(bool val){
		if (isServer)
			return;
		table.attackMode = val;
	}

	[Command]
	private void CmdSetAttack(bool val){
		table.attackMode = val;
	}

	[ClientRpc]
	private void RpcSetDefense(bool val){
		if (isServer)
			return;
		table.defenseMode = val;
	}

	[Command]
	private void CmdSetDefense(bool val){
		table.defenseMode = val;
	}


	public void startNewRound(bool val){
		
		if (val) {
			table.attackMode = true;
			table.defenseMode = false;
			RpcSetDefense (false);
			RpcSetAttack (false);
		} else {
			table.attackMode = false;
			table.defenseMode = false;
			RpcSetDefense (false);
			RpcSetAttack (true);
		}


	}

	public void setOpponentOffered(bool val){
		if(isServer){
			RpcSetOffered (val);
		}else{
			CmdSetOffered (val);
		}
	}

	[Command]
	private void CmdSetOffered(bool val){
		table.offeredMode = val;
	}

	[ClientRpc]
	private void RpcSetOffered(bool val){
		if(!isServer)
			table.offeredMode = val;
	}


}
