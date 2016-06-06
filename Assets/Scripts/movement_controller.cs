using UnityEngine;
using System.Collections;

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
}
