
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Generation;

public class AircraftCollider : MonoBehaviour {
	
	private AircraftMaster _master;

	void Start()
	{
		_master = transform.root.GetComponent<AircraftMaster>();
	}
	
	

	void OnCollisionEnter(Collision col){
		Debug.Log("Aircraft crashed!");
		_master.DestroyShip ();
	}
}
