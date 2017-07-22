using UnityEngine;
using System.Collections;

public class pun_move_analysis : MonoBehaviour {
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
			Debug.Log ("Analysis: swap => 10 points to player" + playerNo);
			break;
		case "wrong_swap":
			learning_modifier (-5, playerNo);
			Debug.Log ("Analysis: wrong_swap => -5 points to player" + playerNo);
			break;
		case "plastic_swap":
			learning_modifier (-20, playerNo);
			Debug.Log ("Analysis: plastic_swap => -20 points to player" + playerNo);
			break;
		case "plastic_blocks":
			learning_modifier (-10, playerNo);
			Debug.Log ("Analysis: plastic_blocks => -10 points to player" + playerNo);
			break;
		case "iron_capture":
			learning_modifier (15, playerNo);
			Debug.Log ("Analysis: iron_capture => 15 points to player" + playerNo);
			break;
		case "miss_iron_capture":
			learning_modifier (-5, playerNo);
			Debug.Log ("Analysis: miss_iron_capture => -5 points to player" + playerNo);
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
