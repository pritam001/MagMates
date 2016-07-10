﻿using UnityEngine;
using System.Collections;

public class game_controller : MonoBehaviour {
	// boardMatrix stores data about the board, where the pawns are placed and helps to decide valid moves
	public static int[,] boardMatrix = new int[,]{ { -1, -1, -1, -1, -1, -1, -1, -1}, { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, 0, 0, 0, 0, 0, 0, -1},  { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, 0, 0, 0, 0, 0, 0, -1},  { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, -1, -1, -1, -1, -1, -1, -1}};
	// playerNo decides which players turn it is (1 or 2)
	public static int playerNo;
	public static bool game_started = false;
	public static bool game_ended = false;
	public static bool game_paused = false;

	public Transform target;//the target object
	public GameObject mainCamera;
	private float speedMod = 10.0f;//a speed modifier
	private Vector3 point;//the coord to the point where the camera looks at
	private static float total_deg = 180f;

	void Start () {//Set up things on the start method
		point = target.transform.position;//get target's coords
		mainCamera.transform.LookAt(point);//makes the camera look to it 
	}
	
	void Update () {
		if(total_deg == 0f){
			StartCoroutine(rotateCam());
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
}
