using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class photon_connect : Photon.PunBehaviour {
	string gameVersion = "1.0";
	public Button ConnectToPUN;
	public Button CreateRoom;
	public Button JoinRandomRoom;
	public Button JoinRoom;

	public Text RoomNameInput;
	public Text OutputText;
	public Text NetworkText;
 

	void Awake() {
		PhotonNetwork.automaticallySyncScene = true;
		disable_all ();
	}

	void disable_all() {
		ConnectToPUN.interactable = true;
		CreateRoom.interactable = false;
		JoinRandomRoom.interactable = false;
		JoinRoom.interactable = false;
	}

	void enable_all() {
		ConnectToPUN.interactable = false;
		CreateRoom.interactable = true;
		JoinRandomRoom.interactable = true;
		JoinRoom.interactable = true;
	}

	// Use this for initialization
	void Start () {
		OutputText.text = "Connect to Photon Unity Network";
	}

	// on click connect to network button
	public void connect() {
		if (!PhotonNetwork.connected) {
			// connect to photon master-server
			PhotonNetwork.ConnectUsingSettings (gameVersion);
		}
	}

	public void createRoom() {
		if (RoomNameInput.text.ToString () == "") {
			PhotonNetwork.CreateRoom (null, new RoomOptions () { MaxPlayers = 2 }, TypedLobby.Default);
			OutputText.text = "Creating room with RANDOM NAME . . .";
		} else {
			PhotonNetwork.CreateRoom (RoomNameInput.text.ToString (), new RoomOptions () { MaxPlayers = 2 }, TypedLobby.Default);
			OutputText.text = "Creating room : " + RoomNameInput.text.ToString () + " . . .";
		}
	}

	public void joinRandomRoom() {
		PhotonNetwork.JoinRandomRoom();
	}

	public void joinRoom() {
		if (RoomNameInput.text.ToString () == "") {
			OutputText.text = "Enter ROOM NAME first !";
		} else {
			PhotonNetwork.JoinRoom (RoomNameInput.text.ToString ());
			OutputText.text = "Connecting to room : " + RoomNameInput.text.ToString () + " . . .";
		}
	}

	public void onclick_return() {
		// return to home menu scene
		OutputText.text = "Returning to start menu . . .";
	}

	public override void OnConnectedToMaster()
	{
		enable_all();
		Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
		NetworkText.text = "Connection established to Photon Unity Network";
		OutputText.text = "Create or Join a Game Room . . .";
	}


	public override void OnDisconnectedFromPhoton()
	{
		Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");        
		disable_all();
		NetworkText.text = "Disconnected from Photon Unity Network";
	}

	void OnPhotonRandomJoinFailed(){
		Debug.Log("Random join failed! No free rooms available.");
		OutputText.text = "Photon : Random join failed! No free rooms available. Please create a new game room.";
	}

	void OnCreatedRoom() {
    	Debug.Log("Created A New Game Room! Waiting for players . . .");
		OutputText.text = "Photon : Created A New Game Room! Waiting for players . . .";
	}

	void OnPhotonCreateRoomFailed() {
	    Debug.LogWarning("Oh No! Creating room failed!");
	    OutputText.text = "Photon : Room creation failed!";
	}

	// Update is called once per frame
	void Update () {
		if (PhotonNetwork.playerList.Length == 2) {
			// Load next scene
			SceneManager.LoadScene("pun_multiplayer");
			OutputText.text = "Match found! Loading next scene . . .";
		}
	}
}
