using UnityEngine;
using System.Collections;

public class move_analysis : MonoBehaviour {
	public static int strategy_point_p1 = 0;
	public static int strategy_point_p2 = 0;
	public static int learning_point_p1 = 0; 
	public static int learning_point_p2 = 0; 

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void learning_modifier(string move,int playerNo){
		switch (move) {
		case "swap":
			learning_modifier (10, playerNo);
			break;
		case "wrong_swap":
			learning_modifier (-5, playerNo);
			break;
		case "plastic_swap":
			learning_modifier (-20, playerNo);
			break;
		default:
			Debug.Log ("Move not found for learning_modifier (" + move + "," + playerNo + ")");
			break;
		}
		
	}

	public static void learning_modifier(int num,int playerNo){
		if (playerNo == 1) {
			learning_point_p1 += num;
		} else {
			learning_point_p2 += num;
		}
	}
}
