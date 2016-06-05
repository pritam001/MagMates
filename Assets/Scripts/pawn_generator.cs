using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class pawn_generator : MonoBehaviour {

	public GameObject IronPawn;
	public Material blackMaterial;

	// Use this for initialization
	void Awake () {
		Resources.UnloadUnusedAssets();
		for(int i = 0; i < 4; i++){
			Instantiate(IronPawn, new Vector3(4.5f, 1.11f, -0.5f + (float)i), Quaternion.identity);
			GameObject tempObj = Instantiate(IronPawn, new Vector3(1.5f, 1.11f, -0.5f + (float)i), Quaternion.identity) as GameObject;
			tempObj.GetComponent<Renderer>().material = blackMaterial;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
