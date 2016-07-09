using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class movement_controller : MonoBehaviour {

	public RaycastHit hit_latest, hit_latest2;
	public Material glass_mat;
	public Material glow_neon_mat;
	public Material glow_red_mat;
	public int hit_latest_column, hit_latest_row;
	public int hit_latest2_column, hit_latest2_row;
	private bool correct_pawn_selected = false;	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			glowOff();
			correct_pawn_selected = false;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit_latest,Mathf.Infinity)){
				// Debug: Selection of glass in hit_latest instead of pawn
				// Re-RayCast using current x and z
				Physics.Raycast( new Vector3 (hit_latest.transform.position.x, 5f, hit_latest.transform.position.z), Vector3.down,out hit_latest,Mathf.Infinity);
				// Check if it is a Self Destruction Cell
				if(hit_latest.transform.position.x < 0.5f || hit_latest.transform.position.z < -1.5f || hit_latest.transform.position.x > 5.5f || hit_latest.transform.position.z > 3.5f){
					//Debug.Log("Skipped");
					goto skip_update_hit_fire1;
				}

				// Check if it is a pawn of the current team
				hit_latest_column = (int)(hit_latest.transform.position.x + 0.5f);
				hit_latest_row = (int)(hit_latest.transform.position.z + 2.5f);
				Debug.Log("[" + hit_latest_row + ", " + hit_latest_column + "] = " + game_controller.boardMatrix[hit_latest_row,hit_latest_column]);
				if(game_controller.boardMatrix[hit_latest_row,hit_latest_column] >= 1 && game_controller.boardMatrix[hit_latest_row,hit_latest_column] <= 3 && game_controller.playerNo == 1
					|| game_controller.boardMatrix[hit_latest_row,hit_latest_column] >= 5 && game_controller.boardMatrix[hit_latest_row,hit_latest_column] <= 7 && game_controller.playerNo == 2 ){
					//Debug.Log(hit_latest.transform.gameObject.name);
					correct_pawn_selected = true;
					//Debug.Log(hit_latest.transform.position);
					int i = (int)((hit_latest.transform.position.x - 0.5f)*6 + (hit_latest.transform.position.z + 2.5f));
					if(i + 6 <= 36){
						glowOn(i + 6);
					}
					if(i - 6 > 0){
						glowOn(i - 6);
					}
					if(i % 6 != 0){
						glowOn(i + 1);
					} else {
						redGlowOn(i);
					}
					if((i-1) % 6 != 0){
						glowOn(i - 1);
					} else {
						redGlowOn(i);
					}
					if(i <= 6 || i >= 31){
						redGlowOn(i);
					}
				}
			}
		}
		skip_update_hit_fire1:
		if (Input.GetButtonDown("Fire2")) {
			glowOff();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit_latest2,Mathf.Infinity)){
				hit_latest2_column = (int)(hit_latest2.transform.position.x + 0.5f);
				hit_latest2_row = (int)(hit_latest2.transform.position.z + 2.5f);
				float distance = Mathf.Abs(hit_latest2.transform.position.x - hit_latest.transform.position.x) + Mathf.Abs(hit_latest2.transform.position.z - hit_latest.transform.position.z);
				if(distance == 1 && correct_pawn_selected){
					// If selected cell is empty, move to that cell
					if(game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] == 0){
						Debug.Log("boardMatrix["+hit_latest_row+","+hit_latest_column+"] = " + game_controller.boardMatrix[hit_latest_row,hit_latest_column] + " moving to empty cell");
						game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] = game_controller.boardMatrix[hit_latest_row,hit_latest_column];
						game_controller.boardMatrix[hit_latest_row,hit_latest_column] = 0;
						StartCoroutine(moveAnimStep(hit_latest.transform.gameObject, new Vector3((hit_latest2.transform.position.x - hit_latest.transform.position.x),0,(hit_latest2.transform.position.z - hit_latest.transform.position.z)), 1f));
						Debug.Log(hit_latest.transform.gameObject.name + " moveAnimStep (" + (hit_latest2.transform.position.x - hit_latest.transform.position.x) + ",0," + (hit_latest2.transform.position.z - hit_latest.transform.position.z) +")");
						game_controller.changePlayer();
					}
				}
			}
			//StartCoroutine(moveAnimStep(GameObject.Find("MagnetPawnA"), new Vector3(0,0,1), 1f));
		}
	}

	void glowOn(int i){
		GameObject go = GameObject.Find("GlowGlass" + i);
		go.GetComponent<Renderer>().material = glow_neon_mat;
	}

	// Red Glow shows positions where self destruction occurs
	void redGlowOn(int i){
		GameObject go = GameObject.Find("GlowGlass-" + i);
		go.GetComponent<Renderer>().material = glow_red_mat;
		if(i == 1|| i == 6 || i == 31 || i == 36){
			GameObject go2 = GameObject.Find("GlowGlass-" + i + "(1)");
			go2.GetComponent<Renderer>().material = glow_red_mat;
		}
	}

	void glowOff(){
		for(int i = 1; i <= 36; i++){
			GameObject go = GameObject.Find("GlowGlass" + i);
			go.GetComponent<Renderer>().material = glass_mat;
		}
		for(int i = -1; i >= -6; i--){
			GameObject go = GameObject.Find("GlowGlass" + i);
			go.GetComponent<Renderer>().material = glass_mat;
		}
		for(int i = -31; i >= -36; i--){
			GameObject go = GameObject.Find("GlowGlass" + i);
			go.GetComponent<Renderer>().material = glass_mat;
		}
		for(int i = 1; i <= 36; i++){
			if(i%6 == 0 || i%6 == 1){
				GameObject go = GameObject.Find("GlowGlass-" + i);
				go.GetComponent<Renderer>().material = glass_mat;
			}
			if(i == 1|| i == 6 || i == 31 || i == 36){
				GameObject go = GameObject.Find("GlowGlass-" + i +"(1)");
				go.GetComponent<Renderer>().material = glass_mat;
			}
		}
	}

	IEnumerator moveAnimStep(GameObject gameObject, Vector3 v, float time){
		float t = 0f;
		Vector3 vectorStart = gameObject.transform.position;
		while (t < time) {
			t += Time.deltaTime; // Sweeps from 0 to time in seconds
			gameObject.transform.position = Vector3.Lerp(vectorStart, vectorStart + v, t);
			yield return null;         // Leave the routine and return here in the next frame
		}
	}
}
