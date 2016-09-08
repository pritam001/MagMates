using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class game_controller : MonoBehaviour {
	// boardMatrix stores data about the board, where the pawns are placed and helps to decide valid moves
	public static int[,] boardMatrix = new int[,]{ { -1, -1, -1, -1, -1, -1, -1, -1}, { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, 0, 0, 0, 0, 0, 0, -1},  { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, 0, 0, 0, 0, 0, 0, -1},  { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, -1, -1, -1, -1, -1, -1, -1}};
	// playerNo decides which players turn it is (1 or 2)
	public static int playerNo;
	public Text player_no_text;
	public static bool game_started = false;
	public static bool game_ended = false;
	public static bool game_paused = false;
	public static bool placing_plastic = false;
	public static bool placing_plastic_button_clicked = false; // true whenever the button is clicked
	public static bool placed_plastic_moved = true; // false if placed but not moved
	public static Vector3 latest_plastic_position = Vector3.zero;

	public static int p1_plastic_remaining = 3;
	public static int p2_plastic_remaining = 3;
	public Text plastic_remaining_text;

	public AudioSource player_changed_audio;

	public Animator gameEndAnimation;
	public GameObject fireworks;
	public bool firework_started = false;

	// Game cameras to switch between top and side view
	public Camera sideCam, topCam;
	public static Camera activeCam;
	public bool topViewOn = false;

	// game rule modifier variables
	public bool polarization_rule_on = true;
	public bool magnet_pole_rule_on = true;
	public bool swap_rule_on = true;
	public bool swapping_preferred = false;
	public Text swapping_text;

	public Transform target;//the target object
	public GameObject mainCamera;
	private float speedMod = 10.0f;//a speed modifier
	private Vector3 point;//the coord to the point where the camera looks at
	private static float total_deg = 180f;

	void Start () {//Set up things on the start method
		activeCam = Camera.main;

		player_no_text.text = "Player " + playerNo;
		point = target.transform.position;//get target's coords
		mainCamera.transform.LookAt(point);//makes the camera look to it 
	}
	
	void Update () {
		if(total_deg == 0f){
			player_no_text.text = "Player " + playerNo;
			player_changed_audio.Play();
			StartCoroutine(rotateCam());
			update_plastic_count();
		}
		if(game_ended && !firework_started){
			Instantiate(fireworks, new Vector3(3f, -0.5f, 1f), Quaternion.identity);
			firework_started = true;
		}
	}

	public static void changePlayer () {
		if(playerNo == 1){
			playerNo = 2;
		} else if(playerNo == 2){
			playerNo = 1;
		}
		total_deg = 0f;
		Debug.Log("Switched to Player" + playerNo);
	}

	public IEnumerator rotateCam(){
		//makes the camera rotate around "point" coords, rotating around its Y axis, 18 degrees per second times the speed modifier
		while(total_deg <= 180){
			mainCamera.transform.RotateAround (point,new Vector3(0.0f,1.0f,0.0f), 18 * Time.deltaTime * speedMod);
			total_deg += 18 * Time.deltaTime * speedMod;
			yield return null;
		}
	}

	// Call when placed plastic is moved
	public static void plastic_placed(){
		placing_plastic = false;
		placing_plastic_button_clicked = false;
		placed_plastic_moved = true;
		latest_plastic_position = Vector3.zero;
	}

	// Call when plastic is placed
	public void update_plastic_count(){
		if(playerNo == 1){
			plastic_remaining_text.text = "x " + p1_plastic_remaining.ToString();
		} else if(playerNo == 2){
			plastic_remaining_text.text = "x " + p2_plastic_remaining.ToString();
		}
	}

	public void on_swap_btnclick(){
		if (swap_rule_on && swapping_preferred) {
			swapping_preferred = false;
			swapping_text.text = "Off";
		} else if (swap_rule_on && !swapping_preferred) {
			swapping_preferred = true;
			swapping_text.text = "On";
		} else if (!swap_rule_on) {
			Debug.Log ("Swapping not allowed in this game.");
			//swapping_preferred = false;
			//swapping_text.text = "Off";
		}
	}

	public void on_viewchanger_click(){
		if (!topViewOn) {
			Vector3 nextPos = topCam.transform.position;
			nextPos.y += 5f;
			topCam.transform.position = nextPos;

			topCam.pixelRect = new Rect (0, 0, Screen.width, Screen.height);
			topCam.depth = -1;
			sideCam.pixelRect = new Rect (0, Screen.height - (float)(0.33 * Screen.height), (float)(0.25 * Screen.width), (float)(0.33 * Screen.height));
			sideCam.depth = 0;

			activeCam = topCam;
			topViewOn = true;
		} else {
			Vector3 nextPos = topCam.transform.position;
			nextPos.y -= 5f;
			topCam.transform.position = nextPos;

			sideCam.pixelRect = new Rect (0, 0, Screen.width, Screen.height);
			sideCam.depth = -1;
			topCam.pixelRect = new Rect (0, Screen.height - (float)(0.33*Screen.height), (float)(0.25*Screen.width), (float)(0.33*Screen.height));
			topCam.depth = 0;

			activeCam = sideCam;
			topViewOn = false;
		}
	}

	public void playerWon(int winCondition, int playerNum){
		if(winCondition == 1){
			Debug.Log("Game ended! Player"+playerNum+" has won the game as opponent's magnet got destroyed.");
		} else if (winCondition == 2){
			Debug.Log("Game ended! Player"+playerNum+" has won the game as opponent doesn't have any more Irons.");
		} 
		gameEndAnimation.SetBool("game_ended", true);
	}
}
