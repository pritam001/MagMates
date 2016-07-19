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
	public GameObject plastic_placing_anim;

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
		// Debugged : if plastic is placed, not moved and button is clicked
		if(!game_controller.placing_plastic && game_controller.placed_plastic_moved){
			Debug.Log("Place plastic mode on. Place plastic clicked.");
			game_controller.placing_plastic = true;
			game_controller.placing_plastic_button_clicked = true;
		} else if(game_controller.placing_plastic && game_controller.placed_plastic_moved){
			Debug.Log("Place plastic mode off. Place plastic clicked.");
			game_controller.placing_plastic = false;
			game_controller.placing_plastic_button_clicked = true;
		}
	}

	IEnumerator placing_plastic(Transform temp_hit_transform){
		// Debugged : if right clicked on correct position, glow off
		GetComponent<movement_controller>().glowOff();

		GameObject go = Instantiate(plastic_placing_anim, new Vector3(temp_hit_transform.position.x, 1.2f, temp_hit_transform.position.z), Quaternion.identity) as GameObject;
		// Update currently placed plastic's position (to be used for movement of that plastic)
		game_controller.latest_plastic_position = temp_hit_transform.position;
		// Show available places to move plastic
		int i = (int)((temp_hit_transform.position.x - 0.5f)*6 + (temp_hit_transform.position.z + 2.5f));
		if(i + 6 <= 36){
			GetComponent<movement_controller>().glowOn(i + 6);
		}
		if(i - 6 > 0){
			GetComponent<movement_controller>().glowOn(i - 6);
		}
		if(i % 6 != 0){
			GetComponent<movement_controller>().glowOn(i + 1);
		} else {
			GetComponent<movement_controller>().redGlowOn(i);
		}
		if((i-1) % 6 != 0){
			GetComponent<movement_controller>().glowOn(i - 1);
		} else {
			GetComponent<movement_controller>().redGlowOn(i);
		}
		if(i <= 6 || i >= 31){
			GetComponent<movement_controller>().redGlowOn(i);
		}

		game_controller.placed_plastic_moved = false;
		//game_controller.p2_plastic_remaining -= 1;
		yield return new WaitForSeconds(1f);
		Destroy(go);
		yield return null;
	}

	// Update is called once per frame
	void Update () {
		if(game_controller.placing_plastic && game_controller.placing_plastic_button_clicked){
			// When place_plastic button is clicked and placing_plastic is activated
			if(game_controller.playerNo == 1){
				Debug.Log("Showing available places to place plastic1.");
				for(int i = 1; i <= 18; i++){
					int column = (int)(GameObject.Find("GlowGlass" + i).transform.position.x + 0.5f);
					int row = (int)(GameObject.Find("GlowGlass" + i).transform.position.z + 2.5f);
					if(game_controller.boardMatrix[row,column] == 0){
						GetComponent<movement_controller>().glowOn(i);
					}
				}
			} else if(game_controller.playerNo == 2){
				Debug.Log("Showing available places to place plastic1.");
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

		// While placing_plastic is on and right clicked to place the plastic pawn
		if (Input.GetButtonDown("Fire2") && game_controller.placing_plastic && game_controller.placed_plastic_moved) {
			RaycastHit t_hit, temp_hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out t_hit,Mathf.Infinity)){
				Physics.Raycast( new Vector3 (t_hit.transform.position.x, 5f, t_hit.transform.position.z), Vector3.down,out temp_hit,Mathf.Infinity);
				int temp_column = (int)(temp_hit.transform.position.x + 0.5f);
				int temp_row = (int)(temp_hit.transform.position.z + 2.5f);
				int temp_i = (int)((temp_hit.transform.position.x - 0.5f)*6 + (temp_hit.transform.position.z + 2.5f));
				if(game_controller.boardMatrix[temp_row, temp_column] == 0){
					if(game_controller.playerNo == 1 && temp_i <= 18 && temp_i > 0){
						Debug.Log("Player" + game_controller.playerNo + " placed plastic at [" + temp_row + "," + temp_column + "]");
						Instantiate(plastic1, new Vector3(temp_hit.transform.position.x, 0.8f, temp_hit.transform.position.z), Quaternion.identity);
						game_controller.boardMatrix[temp_row, temp_column] = 3;
					
						StartCoroutine(placing_plastic(temp_hit.transform));
					} else if(game_controller.playerNo == 2 && temp_i > 18 && temp_i <= 36){
						Debug.Log("Player" + game_controller.playerNo + " placed plastic at [" + temp_row + "," + temp_column + "]");
						Instantiate(plastic2, new Vector3(temp_hit.transform.position.x, 0.8f, temp_hit.transform.position.z), Quaternion.identity);
						game_controller.boardMatrix[temp_row, temp_column] = 7;
						
						StartCoroutine(placing_plastic(temp_hit.transform));
					} else {
						Debug.Log("Can not place plastic there.");
					}
				} else {
					Debug.Log("Can not place plastic there.");
				}
			}
		}

	}
}
