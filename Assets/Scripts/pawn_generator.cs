using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class pawn_generator : MonoBehaviour {

	public GameObject IronPawn;
	public Material blackMaterial;

	public Button place_plastic_button;
	public GameObject plastic1;
	public GameObject plastic2;

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 30;
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

	public void place_plastic_button_clicked(){
		if(!game_controller.placing_plastic){
			game_controller.placing_plastic = true;
			game_controller.placing_plastic_button_clicked = true;
		} else {
			game_controller.placing_plastic = false;
			game_controller.placing_plastic_button_clicked = true;
		}
	}

	// Update is called once per frame
	void Update () {
		if(game_controller.placing_plastic && game_controller.placing_plastic_button_clicked){
			// When place_plastic button is clicked and placing_plastic is activated
			if(game_controller.playerNo == 1){
				for(int i = 1; i <= 18; i++){
					int column = (int)(GameObject.Find("GlowGlass" + i).transform.position.x + 0.5f);
					int row = (int)(GameObject.Find("GlowGlass" + i).transform.position.z + 2.5f);
					if(game_controller.boardMatrix[row,column] == 0){
						GetComponent<movement_controller>().glowOn(i);
					}
				}
			} else if(game_controller.playerNo == 2){
				for(int i = 19; i <= 36; i++){
					int column = (int)(GameObject.Find("GlowGlass" + i).transform.position.x + 0.5f);
					int row = (int)(GameObject.Find("GlowGlass" + i).transform.position.z + 2.5f);
					if(game_controller.boardMatrix[row,column] == 0){
						GetComponent<movement_controller>().glowOn(i);
					}
				}
			} 
			game_controller.placing_plastic_button_clicked = false;
		} else if (!game_controller.placing_plastic && game_controller.placing_plastic_button_clicked){
			// When place_plastic button is clicked and placing_plastic is cancelled
			GetComponent<movement_controller>().glowOff();
			game_controller.placing_plastic_button_clicked = false;
		} 

		if(game_controller.placing_plastic && !game_controller.placing_plastic_button_clicked){
			// To spawn plastic, when placing_plastic is on and positions are lit
			if (Input.GetButtonDown("Fire2")) {
				GetComponent<movement_controller>().glowOff();

			}

			game_controller.placing_plastic = false;
		}
	}
}
