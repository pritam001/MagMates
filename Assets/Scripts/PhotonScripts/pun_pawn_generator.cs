using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class pun_pawn_generator : MonoBehaviour {

	public GameObject IronPawn;
	public Material blackMaterial;

	public Button place_plastic_button;
	public GameObject plastic1;
	public GameObject plastic2;
	public GameObject plastic_placing_anim;

	public AudioSource pawn_appearing_sound;

	// NetworkView used for RPC calls
	public PhotonView photonView;

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
			pawn_appearing_sound.Play();
		}

		// Mark playerNo 1 for starting player
		// Debug : for multiplayer game, randomize the starter
		pun_game_controller.playerNo = 1;

		pun_game_controller.boardMatrix[3,1] = 1;
		pun_game_controller.boardMatrix[4,6] = 5;
		for(int j = 1; j <= 6; j++){
			pun_game_controller.boardMatrix[j,2] = 2;
			pun_game_controller.boardMatrix[j,5] = 6;
		}

		// Initialize plastic count text for both players
		GetComponent<pun_game_controller>().update_plastic_count();
		// Set game_started flag on
		pun_game_controller.game_started = true;
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
		// If plastic count is > zero, enter place plastic phase
		if((pun_game_controller.playerNo == 1 && pun_game_controller.p1_plastic_remaining > 0) || (pun_game_controller.playerNo == 2 && pun_game_controller.p2_plastic_remaining > 0)){
			// Debugged : if plastic is placed, not moved and button is clicked
			if(!pun_game_controller.placing_plastic && pun_game_controller.placed_plastic_moved){
				Debug.Log("Place plastic mode on. Place plastic clicked.");
				pun_game_controller.placing_plastic = true;
				pun_game_controller.placing_plastic_button_clicked = true;
			} else if(pun_game_controller.placing_plastic && pun_game_controller.placed_plastic_moved){
				Debug.Log("Place plastic mode off. Place plastic clicked.");
				pun_game_controller.placing_plastic = false;
				pun_game_controller.placing_plastic_button_clicked = true;
			}
		} else {
			Debug.Log("No more plastic remaining for player"+pun_game_controller.playerNo+".");
		}
	}

	IEnumerator placing_plastic(Vector3 temp_hit_transform_position){
		// Debugged : if right clicked on correct position, glow off
		GetComponent<pun_movement_controller>().glowOff();
		pawn_appearing_sound.Play();

		GameObject go = Instantiate(plastic_placing_anim, new Vector3(temp_hit_transform_position.x, 1.2f, temp_hit_transform_position.z), Quaternion.identity) as GameObject;
		// Update currently placed plastic's position (to be used for movement of that plastic)
		pun_game_controller.latest_plastic_position = temp_hit_transform_position;
		// Show available places to move plastic
		int i = (int)((temp_hit_transform_position.x - 0.5f)*6 + (temp_hit_transform_position.z + 2.5f));
		if(i + 6 <= 36){
			GetComponent<pun_movement_controller>().glowOn(i + 6);
		}
		if(i - 6 > 0){
			GetComponent<pun_movement_controller>().glowOn(i - 6);
		}
		if(i % 6 != 0){
			GetComponent<pun_movement_controller>().glowOn(i + 1);
		} else {
			GetComponent<pun_movement_controller>().redGlowOn(i);
		}
		if((i-1) % 6 != 0){
			GetComponent<pun_movement_controller>().glowOn(i - 1);
		} else {
			GetComponent<pun_movement_controller>().redGlowOn(i);
		}
		if(i <= 6 || i >= 31){
			GetComponent<pun_movement_controller>().redGlowOn(i);
		}

		// Reduce remaining plastic count
		if(pun_game_controller.playerNo == 1){
			pun_game_controller.p1_plastic_remaining -= 1;
			Debug.Log("p1_plastic_remaining -= 1");
		} else if(pun_game_controller.playerNo == 2){
			pun_game_controller.p2_plastic_remaining -= 1;
			Debug.Log("p2_plastic_remaining -= 1");
		}

		pun_game_controller.placed_plastic_moved = false;
		yield return new WaitForSeconds(1f);
		Destroy(go);
		yield return null;
	}

	// Update is called once per frame
	void Update () {
		// Call RPC from host or guest when it is their turn
		//if ((PhotonNetwork.player.ToString () == "#01 (master)" && pun_game_controller.playerNo == 1) || (PhotonNetwork.player.ToString () == "#02 " && pun_game_controller.playerNo == 2)) {

		//}
		pawn_generator_update();

	}

	public void pawn_generator_update(){
		if(pun_game_controller.placing_plastic && pun_game_controller.placing_plastic_button_clicked){
			// When place_plastic button is clicked and placing_plastic is activated
			if(pun_game_controller.playerNo == 1){
				Debug.Log("Showing available places to place plastic1.");
				for(int i = 1; i <= 18; i++){
					int column = (int)(GameObject.Find("GlowGlass" + i).transform.position.x + 0.5f);
					int row = (int)(GameObject.Find("GlowGlass" + i).transform.position.z + 2.5f);
					if(pun_game_controller.boardMatrix[row,column] == 0){
						GetComponent<pun_movement_controller>().glowOn(i);
					}
				}
			} else if(pun_game_controller.playerNo == 2){
				Debug.Log("Showing available places to place plastic1.");
				for(int i = 19; i <= 36; i++){
					int column = (int)(GameObject.Find("GlowGlass" + i).transform.position.x + 0.5f);
					int row = (int)(GameObject.Find("GlowGlass" + i).transform.position.z + 2.5f);
					if(pun_game_controller.boardMatrix[row,column] == 0){
						GetComponent<pun_movement_controller>().glowOn(i);
					}
				}
			} 
			pun_game_controller.placing_plastic_button_clicked = false;
		} else if (!pun_game_controller.placing_plastic && pun_game_controller.placing_plastic_button_clicked){
			// When place_plastic button is clicked and placing_plastic is cancelled
			GetComponent<pun_movement_controller>().glowOff();
			pun_game_controller.placing_plastic_button_clicked = false;
		} 

		// While placing_plastic is on and right clicked to place the plastic pawn
		if (Input.GetButtonDown("Fire2") && pun_game_controller.placing_plastic && pun_game_controller.placed_plastic_moved) {
			RaycastHit t_hit, temp_hit;
			Ray ray = pun_game_controller.activeCam.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out t_hit,Mathf.Infinity)){
				Physics.Raycast( new Vector3 (t_hit.transform.position.x, 5f, t_hit.transform.position.z), Vector3.down,out temp_hit,Mathf.Infinity);
				int temp_column = (int)(temp_hit.transform.position.x + 0.5f);
				int temp_row = (int)(temp_hit.transform.position.z + 2.5f);
				int temp_i = (int)((temp_hit.transform.position.x - 0.5f)*6 + (temp_hit.transform.position.z + 2.5f));
				if(pun_game_controller.boardMatrix[temp_row, temp_column] == 0){
					if(pun_game_controller.playerNo == 1 && temp_i <= 18 && temp_i > 0){
						Vector3 spawn_point = new Vector3(temp_hit.transform.position.x, 0.8f, temp_hit.transform.position.z);
						// Call network to place plastic on all clients
						if ((PhotonNetwork.player.ToString () == "#01 (master)" && pun_game_controller.playerNo == 1) || (PhotonNetwork.player.ToString () == "#02 " && pun_game_controller.playerNo == 2)) {
							photonView.RPC ("plastic_placing_rpc", PhotonTargets.All, temp_i, temp_row, temp_column, spawn_point, temp_hit.transform.position);
						}

					} else if(pun_game_controller.playerNo == 2 && temp_i > 18 && temp_i <= 36){
						Vector3 spawn_point = new Vector3(temp_hit.transform.position.x, 0.8f, temp_hit.transform.position.z);
						// Call network to place plastic on all clients
						if ((PhotonNetwork.player.ToString () == "#01 (master)" && pun_game_controller.playerNo == 1) || (PhotonNetwork.player.ToString () == "#02 " && pun_game_controller.playerNo == 2)) {
							photonView.RPC ("plastic_placing_rpc", PhotonTargets.All, temp_i, temp_row, temp_column, spawn_point, temp_hit.transform.position);
						}
					} else {
						Debug.Log("Can not place plastic there.");
					}
				} else {
					Debug.Log("Can not place plastic there.");
				}
			}
			GetComponent<pun_game_controller>().update_plastic_count();
		}

	}

	[PunRPC]
	public void plastic_placing_rpc(int temp_i, int temp_row, int temp_column, Vector3 spawn_point, Vector3 temp_hit_transform_position){
		if (pun_game_controller.playerNo == 1 && temp_i <= 18 && temp_i > 0) {
			Debug.Log("Player" + pun_game_controller.playerNo + " placed plastic at [" + temp_row + "," + temp_column + "]");
			Instantiate(plastic1, spawn_point, Quaternion.identity);
			pun_game_controller.boardMatrix[temp_row, temp_column] = 3;

			StartCoroutine(placing_plastic(temp_hit_transform_position));
		} else if(pun_game_controller.playerNo == 2 && temp_i > 18 && temp_i <= 36) {
			Debug.Log("Player" + pun_game_controller.playerNo + " placed plastic at [" + temp_row + "," + temp_column + "]");
			Instantiate(plastic2, spawn_point, Quaternion.identity);
			pun_game_controller.boardMatrix[temp_row, temp_column] = 7;

			StartCoroutine(placing_plastic(temp_hit_transform_position));
		}
	}

}
