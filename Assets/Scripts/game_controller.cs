using UnityEngine;
using System.Collections;

public class game_controller : MonoBehaviour {
	// boardMatrix stores data about the board, where the pawns are placed and helps to decide valid moves
	public static int[,] boardMatrix = new int[,]{ { -1, -1, -1, -1, -1, -1, -1, -1}, { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, 0, 0, 0, 0, 0, 0, -1},  { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, 0, 0, 0, 0, 0, 0, -1},  { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, 0, 0, 0, 0, 0, 0, -1}, { -1, -1, -1, -1, -1, -1, -1, -1}};
	// playerNo decides which players turn it is (1 or 2)
	public static int playerNo;
	// Use this for initialization
	void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}
