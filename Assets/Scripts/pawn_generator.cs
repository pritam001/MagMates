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
		for(int i = 0; i < 6; i++){
			GameObject tempObj1 = Instantiate(IronPawn, new Vector3(4.5f, 0.5f, -1.5f + (float)i), Quaternion.identity) as GameObject;
			StartCoroutine(MoveFromTo(tempObj1, new Vector3(4.5f, 0.5f, -1.5f + (float)i), new Vector3(4.5f, 1.12f, -1.5f + (float)i), 3f));
			GameObject tempObj2 = Instantiate(IronPawn, new Vector3(1.5f, 0.5f, -0.5f + (float)i), Quaternion.identity) as GameObject;
			tempObj2.GetComponent<Renderer>().material = blackMaterial;
			StartCoroutine(MoveFromTo(tempObj2, new Vector3(1.5f, 0.5f, -1.5f + (float)i), new Vector3(1.5f, 1.12f, -1.5f + (float)i), 3f));
		}

		game_controller.playerNo = 1;

		game_controller.boardMatrix[3,1] = 1;
		game_controller.boardMatrix[4,6] = 5;
		for(int j = 1; j <= 6; j++){
			game_controller.boardMatrix[j,2] = 2;
			game_controller.boardMatrix[j,5] = 6;
		}

		// Set game_started flag on
		game_controller.game_started = true;
	}

	IEnumerator MoveFromTo(GameObject gameObject, Vector3 vectorFrom, Vector3 vectorTo, float time){
		float t = 0f;
		while (t < time) {
			t += Time.deltaTime; // Sweeps from 0 to time in seconds
			gameObject.transform.position = Vector3.Lerp(vectorFrom, vectorTo, t);
			yield return null;         // Leave the routine and return here in the next frame
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
