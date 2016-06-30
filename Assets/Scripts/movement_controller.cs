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
	public Material glow_red_mat;	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			glowOff();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit_latest,Mathf.Infinity)){
				// Check if it is a Self Destruction Cell
				if(hit_latest.transform.position.x < 0.5f || hit_latest.transform.position.z < -1.5f || hit_latest.transform.position.x > 3.5f || hit_latest.transform.position.z > 5.5f){
					goto skip_update_hit_fire1;
				}
				//Debug.Log(hit_latest.transform.gameObject.name);
				//Debug.Log(hit_latest.transform.position);
				int i = (int)((hit_latest.transform.position.x - 0.5f)*6 + (hit_latest.transform.position.z + 2.5f));
				if(i + 6 <= 30){
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
		skip_update_hit_fire1:
		if (Input.GetButtonDown("Fire2")) {
			glowOff();
			StartCoroutine(moveAnimStep(GameObject.Find("MagnetPawnA"), new Vector3(0,0,1), 1f));
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
