using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class network_player_search : NetworkBehaviour {
	public Text IPText;
	public Text PortText;
	public Text StopReturnText;
	public Text OutputText;
	public Text NetworkText;
	public Button HostServerButton;
	public Button JoinServerButton;
	public Button StopReturnButton;

	public string connectionIp = "127.0.0.1";
	public int portNumber = 10001;
	private bool search_on = false;

	public void hostServerClick(){
		IPPortUpdater ();
		Network.InitializeServer (1, portNumber, false);
		OutputText.text = "Game hosted at " + connectionIp + ":" + portNumber.ToString() 
			+ "\nWaiting for opponent to join . . .";
		HostServerButton.interactable = false;
		JoinServerButton.interactable = false;
		search_on = true;
		StopReturnText.text = "Stop !";
	}

	public void joinServerClick(){
		IPPortUpdater ();
		Network.Connect (connectionIp, portNumber);
		OutputText.text = "Connecting to " + connectionIp + ":" + portNumber.ToString() 
			+ "\nWaiting for host . . .";
		HostServerButton.interactable = false;
		JoinServerButton.interactable = false;
		search_on = true;
		StopReturnText.text = "Stop !";
	}

	public void stopOrReturnClick(){
		if (search_on) {
			// Stop current hosting or joining
			IPPortUpdater ();
			OutputText.text = "Host or join a multiplayer game";
			HostServerButton.interactable = true;
			JoinServerButton.interactable = true;
			search_on = false;
			Network.Disconnect ();
			StopReturnText.text = "Return";
		} else {
			// Return to main menu
			//SceneManager.LoadScene ("main_menu");
		}
	}

	public void IPPortUpdater(){
		if (IPText.text != "") {
			//if ip is in proper format
			connectionIp = IPText.text;
		} else {
			connectionIp = "127.0.0.1";
		}

		if (PortText.text != "") {
			portNumber = Convert.ToInt32(PortText.text);
		} else {
			portNumber = 10001;
		}
	}

	// Use this for initialization
	void Start () {
		
	}

	void OnConnectedToServer(){
		print ("the player id: " + Network.connections.Length);
	}

	// Update is called once per frame
	void Update () {
		if (Network.peerType == NetworkPeerType.Client) {
			NetworkText.text = "Client peer";
			SceneManager.LoadScene ("board_field");
		} else if (Network.peerType == NetworkPeerType.Server) {
			if (Network.connections.Length == 1) {
				NetworkText.text = "Server with 1 connection peer";
				SceneManager.LoadScene ("board_field");
			} else {
				NetworkText.text = "Server with " + Network.connections.Length.ToString () + " connection peer";
			}
		} else if (Network.peerType == NetworkPeerType.Connecting) {
			NetworkText.text = "Connecting peer";
		} else if (Network.peerType == NetworkPeerType.Disconnected) {
			NetworkText.text = "Disconnected peer";
		} else {
			NetworkText.text = "Unknown peer";
		}
	}
}

/*
//script for menu scene
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Menu : MonoBehaviour {

	public string connectionIp = "127.0.0.1";
	public int portNumber = 10000;

	public void startServerButton(){
		Network.InitializeServer (1, portNumber, false);
		SceneManager.LoadScene ("Waiting");
	}

	public void joinServer(){
		Network.Connect (connectionIp, portNumber);
		SceneManager.LoadScene ("Waiting");
	}


}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AllPlayerConnected : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	//void OnConnectedToServer(){
	//	print ("the player id: "+Network.connections.Length);
	//}
	// Update is called once per frame
	void Update () {
		//if (Network.peerType == NetworkPeerType.Client) {
		//SceneManager.LoadScene ("Level");
		//} else if (Network.peerType == NetworkPeerType.Server) {
		if (Network.connections.Length == 1) {
			SceneManager.LoadScene ("Level");
		} 
		//}
	}
} */