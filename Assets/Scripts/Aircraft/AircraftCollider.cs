/* Copyright (C) Luaek - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Avi Vajpeyi
 */

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
		_master.DestroyShip ();
	}
}
