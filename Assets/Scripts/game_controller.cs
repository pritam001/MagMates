using UnityEngine;
using System.Collections;

public class game_controller : MonoBehaviour {
	// boardMatrix stores data about the board, where the pawns are placed and helps to decide valid moves
	public static Vector3[,] boardMatrix = new Vector3[8,8];
	// Use this for initialization
	void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}
