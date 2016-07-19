using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class movement_controller : MonoBehaviour {

	public Material glass_mat;
	public Material glow_neon_mat;
	public Material glow_red_mat;

	public GameObject destructionPrefab;

	public RaycastHit hit_latest, hit_latest2;
	public int hit_latest_column, hit_latest_row;
	public int hit_latest2_column, hit_latest2_row;

	public AudioSource destroy_sound;
	public AudioSource moving_sound;

	private bool correct_pawn_selected = false;	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(game_controller.game_started && !game_controller.game_ended && !game_controller.game_paused && !game_controller.placing_plastic){	
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
					Debug.Log("Player" + game_controller.playerNo + " clicked on [" + hit_latest_row + ", " + hit_latest_column + "] = " + game_controller.boardMatrix[hit_latest_row,hit_latest_column]);
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
			if (Input.GetButtonDown("Fire2") && !game_controller.placing_plastic) {
				glowOff();
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray,out hit_latest2,Mathf.Infinity)){
					hit_latest2_column = (int)(hit_latest2.transform.position.x + 0.5f);
					hit_latest2_row = (int)(hit_latest2.transform.position.z + 2.5f);
					float distance = Mathf.Abs(hit_latest2.transform.position.x - hit_latest.transform.position.x) + Mathf.Abs(hit_latest2.transform.position.z - hit_latest.transform.position.z);
					
					int x_value = (int)(Mathf.Abs(hit_latest2.transform.position.x - hit_latest.transform.position.x));
					int z_value = (int)(Mathf.Abs(hit_latest2.transform.position.z - hit_latest.transform.position.z));
					if(x_value == 1){
						x_value = (int)(Mathf.Abs(hit_latest2.transform.position.x - hit_latest.transform.position.x)/(hit_latest2.transform.position.x - hit_latest.transform.position.x));
					}
					if(z_value == 1){
						z_value = (int)(Mathf.Abs(hit_latest2.transform.position.z - hit_latest.transform.position.z)/(hit_latest2.transform.position.z - hit_latest.transform.position.z));
					}
	
					if(distance == 1 && correct_pawn_selected){
						if(game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] == 0){
							// If selected cell is empty, move to that cell
							Debug.Log("boardMatrix["+hit_latest_row+","+hit_latest_column+"] = " + game_controller.boardMatrix[hit_latest_row,hit_latest_column] + " moving to empty cell " + "boardMatrix["+hit_latest2_row+","+hit_latest2_column+"] = " + game_controller.boardMatrix[hit_latest2_row,hit_latest2_column]);
							game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] = game_controller.boardMatrix[hit_latest_row,hit_latest_column];
							game_controller.boardMatrix[hit_latest_row,hit_latest_column] = 0;
							StartCoroutine(moveAnimStep(hit_latest.transform.gameObject, new Vector3((hit_latest2.transform.position.x - hit_latest.transform.position.x),0,(hit_latest2.transform.position.z - hit_latest.transform.position.z)), 1f));
							Debug.Log(hit_latest.transform.gameObject.name + " moveAnimStep (" + (hit_latest2.transform.position.x - hit_latest.transform.position.x) + ",0," + (hit_latest2.transform.position.z - hit_latest.transform.position.z) +")");
							game_controller.changePlayer();
						} else if(game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] == -1){
							// If selected cell is self destruction cell
							Debug.Log("boardMatrix["+hit_latest_row+","+hit_latest_column+"] = " + game_controller.boardMatrix[hit_latest_row,hit_latest_column] + " moving to self destruction cell " + "boardMatrix["+hit_latest2_row+","+hit_latest2_column+"] = " + game_controller.boardMatrix[hit_latest2_row,hit_latest2_column]);
							game_controller.boardMatrix[hit_latest_row,hit_latest_column] = 0;
							StartCoroutine(moveAnimStep(hit_latest.transform.gameObject, new Vector3((hit_latest2.transform.position.x - hit_latest.transform.position.x),0,(hit_latest2.transform.position.z - hit_latest.transform.position.z)), 1f));
							Debug.Log(hit_latest.transform.gameObject.name + " moveAnimStep (" + (hit_latest2.transform.position.x - hit_latest.transform.position.x) + ",0," + (hit_latest2.transform.position.z - hit_latest.transform.position.z) +")");
							StartCoroutine(destroyPawn(hit_latest.transform.gameObject, game_controller.playerNo));
							game_controller.changePlayer();
						} else if(game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] == 3 || game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] == 7){
							// If selected cell contains plastic
							if(game_controller.boardMatrix[hit_latest_row,hit_latest_column] == 3 || game_controller.boardMatrix[hit_latest_row,hit_latest_column] == 7){
								Debug.Log("Plastic can not move plastic.");
							} else if(game_controller.boardMatrix[hit_latest2_row + z_value,hit_latest2_column + x_value] > 0){
								Debug.Log("Cell next to plastic is not empty.");
								Debug.Log("boardMatrix[" + (hit_latest2_row + z_value) + "," + (hit_latest2_column + x_value) + "] = " + game_controller.boardMatrix[hit_latest_row + z_value,hit_latest_column + x_value]);
							} else {
								Debug.Log("Cell next to plastic is empty. Plastic gets pushed.");
								push_pawn(hit_latest_row + z_value, hit_latest_column + x_value, z_value, x_value);
								push_pawn(hit_latest_row, hit_latest_column, z_value, x_value);
								game_controller.changePlayer();
							}
							
						} else if(game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] > 0f){
							// If selected cell contains some pawn other than plastic
							bool plastic_blocks = false;
							Debug.Log("Move checking " + game_controller.boardMatrix[hit_latest2_row,hit_latest2_column]);
							int i = 1;
							//Debug.Log("x_value = " + x_value + " z_value = " + z_value);
							while(game_controller.boardMatrix[hit_latest_row + i*z_value,hit_latest_column + i*x_value] > 0){
								//Debug.Log("Checking " + game_controller.boardMatrix[hit_latest_row + i*z_value,hit_latest_column + i*x_value]);
								// Check if plastic exists in the path
								if(game_controller.boardMatrix[hit_latest_row + i*z_value,hit_latest_column + i*x_value] == 3 || game_controller.boardMatrix[hit_latest_row + i*z_value,hit_latest_column + i*x_value] == 7){
									plastic_blocks = true;
								}
								i += 1;
							}
	
							if(plastic_blocks){
								Debug.Log("Plastic blocked the desired movement.");
							} else {
								for(;i>0;i--){
									push_pawn(hit_latest_row + (i-1)*z_value, hit_latest_column + (i-1)*x_value, z_value, x_value);
								}
								Debug.Log("Pushing all the pawns in the path.");
								game_controller.changePlayer();
							}
						}
					}
				}
				// You have to re-left-click to move again
				correct_pawn_selected = false;
			}

		}

		// If placing_plastic is on and plastic pawn is placed, but not moved yet
		if (Input.GetButtonDown("Fire2") && game_controller.placing_plastic && !game_controller.placed_plastic_moved) {
			Debug.Log("Right click move plastic " + game_controller.latest_plastic_position);
			Physics.Raycast( new Vector3 (game_controller.latest_plastic_position.x, 5f, game_controller.latest_plastic_position.z), Vector3.down,out hit_latest,Mathf.Infinity);
			hit_latest_column = (int)(game_controller.latest_plastic_position.x + 0.5f);
			hit_latest_row = (int)(game_controller.latest_plastic_position.z + 2.5f);

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit_latest2,Mathf.Infinity)){
				hit_latest2_column = (int)(hit_latest2.transform.position.x + 0.5f);
				hit_latest2_row = (int)(hit_latest2.transform.position.z + 2.5f);
				float distance = Mathf.Abs(hit_latest2.transform.position.x - hit_latest.transform.position.x) + Mathf.Abs(hit_latest2.transform.position.z - hit_latest.transform.position.z);
				
				int x_value = (int)(Mathf.Abs(hit_latest2.transform.position.x - hit_latest.transform.position.x));
				int z_value = (int)(Mathf.Abs(hit_latest2.transform.position.z - hit_latest.transform.position.z));
				if(x_value == 1){
					x_value = (int)(Mathf.Abs(hit_latest2.transform.position.x - hit_latest.transform.position.x)/(hit_latest2.transform.position.x - hit_latest.transform.position.x));
				}
				if(z_value == 1){
					z_value = (int)(Mathf.Abs(hit_latest2.transform.position.z - hit_latest.transform.position.z)/(hit_latest2.transform.position.z - hit_latest.transform.position.z));
				}

				Debug.Log("x_value " + x_value + " z_value " + z_value);
				if(distance == 1){
					if(game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] == 0){
						// If selected cell is empty, move to that cell
						Debug.Log("boardMatrix["+hit_latest_row+","+hit_latest_column+"] = " + game_controller.boardMatrix[hit_latest_row,hit_latest_column] + " moving to empty cell " + "boardMatrix["+hit_latest2_row+","+hit_latest2_column+"] = " + game_controller.boardMatrix[hit_latest2_row,hit_latest2_column]);
						game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] = game_controller.boardMatrix[hit_latest_row,hit_latest_column];
						game_controller.boardMatrix[hit_latest_row,hit_latest_column] = 0;
						StartCoroutine(moveAnimStep(hit_latest.transform.gameObject, new Vector3((hit_latest2.transform.position.x - hit_latest.transform.position.x),0,(hit_latest2.transform.position.z - hit_latest.transform.position.z)), 1f));
						Debug.Log(hit_latest.transform.gameObject.name + " moveAnimStep (" + (hit_latest2.transform.position.x - hit_latest.transform.position.x) + ",0," + (hit_latest2.transform.position.z - hit_latest.transform.position.z) +")");
						glowOff();
						game_controller.plastic_placed();
						game_controller.changePlayer();
					} else if(game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] == -1){
						// If selected cell is self destruction cell
						Debug.Log("boardMatrix["+hit_latest_row+","+hit_latest_column+"] = " + game_controller.boardMatrix[hit_latest_row,hit_latest_column] + " moving to self destruction cell " + "boardMatrix["+hit_latest2_row+","+hit_latest2_column+"] = " + game_controller.boardMatrix[hit_latest2_row,hit_latest2_column]);
						game_controller.boardMatrix[hit_latest_row,hit_latest_column] = 0;
						StartCoroutine(moveAnimStep(hit_latest.transform.gameObject, new Vector3((hit_latest2.transform.position.x - hit_latest.transform.position.x),0,(hit_latest2.transform.position.z - hit_latest.transform.position.z)), 1f));
						Debug.Log(hit_latest.transform.gameObject.name + " moveAnimStep (" + (hit_latest2.transform.position.x - hit_latest.transform.position.x) + ",0," + (hit_latest2.transform.position.z - hit_latest.transform.position.z) +")");
						StartCoroutine(destroyPawn(hit_latest.transform.gameObject, game_controller.playerNo));
						glowOff();
						game_controller.plastic_placed();
						game_controller.changePlayer();
					} else if(game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] == 3 || game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] == 7){
						// If selected cell contains plastic
						if(game_controller.boardMatrix[hit_latest_row,hit_latest_column] == 3 || game_controller.boardMatrix[hit_latest_row,hit_latest_column] == 7){
							Debug.Log("Plastic can not move plastic.");
						} else if(game_controller.boardMatrix[hit_latest2_row + z_value,hit_latest2_column + x_value] > 0){
							Debug.Log("Cell next to plastic is not empty.");
							Debug.Log("boardMatrix[" + (hit_latest2_row + z_value) + "," + (hit_latest2_column + x_value) + "] = " + game_controller.boardMatrix[hit_latest_row + z_value,hit_latest_column + x_value]);
						} else {
							Debug.Log("Cell next to plastic is empty. Plastic gets pushed.");
							push_pawn(hit_latest_row + z_value, hit_latest_column + x_value, z_value, x_value);
							push_pawn(hit_latest_row, hit_latest_column, z_value, x_value);
							glowOff();
							game_controller.plastic_placed();
							game_controller.changePlayer();
						}
						
					} else if(game_controller.boardMatrix[hit_latest2_row,hit_latest2_column] > 0f){
						// If selected cell contains some pawn other than plastic
						bool plastic_blocks = false;
						Debug.Log("Move checking " + game_controller.boardMatrix[hit_latest2_row,hit_latest2_column]);
						int i = 1;
						//Debug.Log("x_value = " + x_value + " z_value = " + z_value);
						while(game_controller.boardMatrix[hit_latest_row + i*z_value,hit_latest_column + i*x_value] > 0){
							//Debug.Log("Checking " + game_controller.boardMatrix[hit_latest_row + i*z_value,hit_latest_column + i*x_value]);
							// Check if plastic exists in the path
							if(game_controller.boardMatrix[hit_latest_row + i*z_value,hit_latest_column + i*x_value] == 3 || game_controller.boardMatrix[hit_latest_row + i*z_value,hit_latest_column + i*x_value] == 7){
								plastic_blocks = true;
							}
							i += 1;
						}

						if(plastic_blocks){
							Debug.Log("Plastic blocked the desired movement.");
						} else {
							for(;i>0;i--){
								push_pawn(hit_latest_row + (i-1)*z_value, hit_latest_column + (i-1)*x_value, z_value, x_value);
							}
							Debug.Log("Pushing all the pawns in the path.");
							glowOff();
							game_controller.plastic_placed();
							game_controller.changePlayer();
						}
					}
				}
			}
			
		}

	}

	// Pushes the pawn of selected row and column using z and x as unit directions
	void push_pawn(int row_num,int col_num,int z,int x){
		RaycastHit temp_hit;
		Physics.Raycast( new Vector3 (col_num - 0.5f, 5f, row_num - 2.5f), Vector3.down,out temp_hit,Mathf.Infinity);
		if(game_controller.boardMatrix[row_num + z, col_num + x] == 0){
			game_controller.boardMatrix[row_num + z, col_num + x] = game_controller.boardMatrix[row_num, col_num];
			game_controller.boardMatrix[row_num, col_num] = 0;
			StartCoroutine(moveAnimStep(temp_hit.transform.gameObject, new Vector3(x,0,z), 1f));
		} else if(game_controller.boardMatrix[row_num + z, col_num + x] == -1){
			game_controller.boardMatrix[row_num, col_num] = 0;
			StartCoroutine(moveAnimStep(temp_hit.transform.gameObject, new Vector3(x,0,z), 1f));
			StartCoroutine(destroyPawn(temp_hit.transform.gameObject, game_controller.playerNo));
		}

	}

	public void glowOn(int i){
		GameObject go = GameObject.Find("GlowGlass" + i);
		go.GetComponent<Renderer>().material = glow_neon_mat;
	}

	// Red Glow shows positions where self destruction occurs
	public void redGlowOn(int i){
		GameObject go = GameObject.Find("GlowGlass-" + i);
		go.GetComponent<Renderer>().material = glow_red_mat;
		if(i == 1|| i == 6 || i == 31 || i == 36){
			GameObject go2 = GameObject.Find("GlowGlass-" + i + "(1)");
			go2.GetComponent<Renderer>().material = glow_red_mat;
		}
	}

	public void glowOff(){
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
		moving_sound.Play();
		while (t < time) {
			t += Time.deltaTime; // Sweeps from 0 to time in seconds
			gameObject.transform.position = Vector3.Lerp(vectorStart, vectorStart + v, t);
			yield return null;         // Leave the routine and return here in the next frame
		}
	}

	IEnumerator destroyPawn(GameObject gameObject, int playerNumber){
		yield return new WaitForSeconds(1.05f); // this will wait > 1 seconds
		GameObject blastObj = Instantiate(destructionPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
		Destroy(gameObject);
		destroy_sound.Play();
		Debug.Log("Player" + playerNumber + "'s " + hit_latest.transform.gameObject.name + " self destroyed.");
		// Check if a magnet was destroyed, then set game_ended flag + show winning animation
		bool magnet1_exists = false;
		bool iron1_exists = false;
		bool magnet2_exists = false;
		bool iron2_exists = false;
		for(int i=1; i<7; i++){
			for(int j=1; j<7; j++){
				if(game_controller.boardMatrix[i,j] == 1){
					magnet1_exists = true;
				}
				if(game_controller.boardMatrix[i,j] == 2){
					iron1_exists = true;
				}
				if(game_controller.boardMatrix[i,j] == 5){
					magnet2_exists = true;
				}
				if(game_controller.boardMatrix[i,j] == 6){
					iron2_exists = true;
				}
			}
		}
		if(!magnet2_exists){
			GetComponent<game_controller>().playerWon(1, 1);
			game_controller.game_ended = true;
		}
		if(!magnet1_exists){
			GetComponent<game_controller>().playerWon(1, 2);
			game_controller.game_ended = true;
		}
		if(!iron2_exists){
			GetComponent<game_controller>().playerWon(2, 1);
			game_controller.game_ended = true;
		}
		if(!iron1_exists){
			GetComponent<game_controller>().playerWon(2, 2);
			game_controller.game_ended = true;
		}
		// Wait 2 seconds, then destroy the Explosion Prefab
		yield return new WaitForSeconds(5f);
		Destroy(blastObj);
	} 
}
