using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class movement_controller : MonoBehaviour {

	public RaycastHit hit_latest;
	public Material glass_mat;
	public Material glow_neon_mat;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			glowOff();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit_latest,Mathf.Infinity)){
				//Debug.Log(hit_latest.transform.gameObject.name);
				//Debug.Log(hit_latest.transform.position);
				int i = (int)((hit_latest.transform.position.x - 0.5f)*4 + (3.5f - hit_latest.transform.position.z));
				if(i + 4 <= 24){
					glowOn(i + 4);
				}
				if(i - 4 > 0){
					glowOn(i - 4);
				}
				if(i % 4 != 0){
					glowOn(i + 1);
				}
				if((i-1) % 4 != 0){
					glowOn(i - 1);
				}
			}
		}
		if (Input.GetButtonDown("Fire2")) {
			glowOff();
			StartCoroutine(moveAnimStep(GameObject.Find("MagnetPawnA"), new Vector3(0,0,1), 1f));
		}
	}

	void glowOn(int i){
		GameObject go = GameObject.Find("Cube" + i + " (1)");
		go.GetComponent<Renderer>().material = glow_neon_mat;
	}

	void glowOff(){
		for(int i = 1; i <= 24; i++){
			GameObject go = GameObject.Find("Cube" + i + " (1)");
			go.GetComponent<Renderer>().material = glass_mat;
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
