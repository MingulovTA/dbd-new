//-----------------------------------------------------------------------------------------------------------------
// 
//	Mushroom Example
//	Created by : Luis Filipe (filipe@seines.pt)
//	Dec 2010
//
//	Source code in this example is in the public domain.
//  The naruto character model in this demo is copyrighted by Ben Mathis.
//  See Assets/Models/naruto.txt for more details
//
//-----------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerIOClient;
using Random = UnityEngine.Random;

public class ChatEntry {
	public string text = "";
	public bool mine = true;
}

public class GameManager : MonoBehaviour 
{

	public GameObject target;
	public GameObject PlayerPrefab;
	public GameObject ToadPrefab;
	private Connection pioconnection;
	private List<Message> msgList = new List<Message>(); //  Messsage queue implementation
	private bool joinedroom = false;

	// UI stuff
	private Vector2 scrollPosition;
	private ArrayList entries = new ArrayList();
	private string inputField = "";
	private Rect window = new Rect(10, 10, 300, 150);
	private int toadspicked = 0;

	private string infomsg = "";

	private Client _client;
	private string _userId;

	private void Start() 
	{
		Debug.Log("Starting");
		
		Application.runInBackground = true;
		_userId = $"Guest {Random.Range(0, 10000)}";
		PlayerIO.Authenticate(
			"dbd-new-qrdesbn2rku1glgjblamhq", "public", 
			new Dictionary<string, string> {{ "userId", _userId },},
			null, OnConnectSuccess, OnConnectFailed
		);
	}

	private void OnConnectSuccess(Client client)
	{
		_client = client;
		Debug.Log("Successfully connected to Player.IO");
		infomsg = "Successfully connected to Player.IO";

		target.transform.Find("NameTag").GetComponent<TextMesh>().text = _userId;
		target.transform.name = _userId;

		Debug.Log("Create ServerEndpoint");
		client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);

		Debug.Log("CreateJoinRoom");
		//Create or join the room 
		client.Multiplayer.CreateJoinRoom(
			"UnityDemoRoom",                    //Room id. If set to null a random roomid is used
			"UnityMushrooms",                   //The room type started on the server
			true,                               //Should the room be visible in the lobby?
			null,
			null,
			delegate (Connection connection) {
				Debug.Log("Joined Room.");
				infomsg = "Joined Room.";
				// We successfully joined a room so set up the message handler
				pioconnection = connection;
				pioconnection.OnMessage += handlemessage;
				joinedroom = true;
			},
			delegate (PlayerIOError error) {
				Debug.Log("Error Joining Room: " + error.ToString());
				infomsg = error.ToString();
			}
		);
	}

	private void OnConnectFailed(PlayerIOError error)
	{
		Debug.Log("Error connecting: " + error.ToString());
		infomsg = error.ToString();
	}

	private void OnDisable()
	{
		_client?.Logout();
	}

	void handlemessage(object sender, Message m) {
		msgList.Add(m);
	}

	void FixedUpdate() {
		foreach (Message m in msgList) {
			switch (m.Type) {
				case "PlayerJoined":
					GameObject newplayer = GameObject.Instantiate(target) as GameObject;
					newplayer.transform.position = new Vector3(m.GetFloat(1), 0, m.GetFloat(2));
					newplayer.name = m.GetString(0);
					newplayer.transform.Find("NameTag").GetComponent<TextMesh>().text = m.GetString(0);
					break;
				case "Move":
					GameObject upplayer = GameObject.Find(m.GetString(0));
					upplayer.transform.LookAt(new Vector3(m.GetFloat(1), 0, m.GetFloat(2)));
					upplayer.transform.eulerAngles = new Vector3(0, upplayer.transform.eulerAngles.y, upplayer.transform.eulerAngles.z);

					float dist = Vector3.Distance(upplayer.transform.position, new Vector3(m.GetFloat(1), 0, m.GetFloat(2)));
					//iTween.MoveTo(upplayer, iTween.Hash("x", m.GetFloat(1), "z", m.GetFloat(2), "onstart", "startwalk", "oncomplete", "stopwalk", "time", dist, "delay", 0, "easetype", iTween.EaseType.linear));
					break;

				case "Harvest":
					GameObject hvplayer = GameObject.Find(m.GetString(0));
					hvplayer.transform.LookAt(new Vector3(m.GetFloat(1), .5f, m.GetFloat(2)));
					hvplayer.transform.eulerAngles = new Vector3(0, hvplayer.transform.eulerAngles.y, hvplayer.transform.eulerAngles.z);
					float distance = Vector3.Distance(hvplayer.transform.position, new Vector3(m.GetFloat(1), 0, m.GetFloat(2)));
					//iTween.MoveTo(hvplayer, iTween.Hash("x", m.GetFloat(1), "z", m.GetFloat(2), "onstart", "startwalk", "oncomplete", "stopharvest", "time", distance, "delay", 0, "easetype", iTween.EaseType.linear));
					break;
				case "Picked":
					// remove the object when it's picked up
					GameObject removetoad = GameObject.Find("Toad" + m.GetInt(0));
					Destroy(removetoad);

					break;
				case "Chat":
					if (m.GetString(0) != "Server") {
						GameObject chatplayer = GameObject.Find(m.GetString(0));
						chatplayer.transform.Find("Chat").GetComponent<TextMesh>().text = m.GetString(1);
						chatplayer.transform.Find("Chat").GetComponent<MeshRenderer>().material.color = Color.white;
						chatplayer.transform.Find("Chat").GetComponent<chatclear>().lastupdate = Time.time;
					}
					ChatText(m.GetString(0) + " says: " + m.GetString(1), false);
					break;
				case "PlayerLeft":
					// remove characters from the scene when they leave
					GameObject playerd = GameObject.Find(m.GetString(0));
					Destroy(playerd);
					break;
				case "Toad":
					// adds a toadstool to the scene
					GameObject newtoad = GameObject.Instantiate(ToadPrefab) as GameObject;
					newtoad.transform.position = new Vector3(m.GetFloat(1), 0.1f, m.GetFloat(2));
					newtoad.name = "Toad" + m.GetInt(0);
					break;
				case "ToadCount":
					// updates how many toads have been picked up by the player
					toadspicked = m.GetInt(0);
					break;
			}
		}

		msgList.Clear();
	}

	void OnMouseDown() 
	{
		if (!joinedroom) {
			return;
		}

		Vector3 targetPosition = new Vector3(0, 0, 0);

		var playerPlane = new Plane(Vector3.up, target.transform.position);
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		var hitdist = 0.0f;
		if (playerPlane.Raycast(ray, out hitdist)) {
			targetPosition = ray.GetPoint(hitdist);
			pioconnection.Send("Move", targetPosition.x, targetPosition.z);
		}
	}


	void OnGUI() {
		window = GUI.Window(1, window, GlobalChatWindow, "Chat");
		GUI.Label(new Rect(10, 160, 150, 20), "Toadstools picked: " + toadspicked);
		if (infomsg != "") {
			GUI.Label(new Rect(10, 180, Screen.width, 20), infomsg);
		}
	}

	public void HarvestAt(float posx, float posz) {
		pioconnection.Send("MoveHarvest", posx, posz);
	}

	public void TryPickup(string id) {
		pioconnection.Send("Pickup", id);
	}

	void GlobalChatWindow(int id) {

		if (!joinedroom) {
			return;
		}

		GUI.FocusControl("Chat input field");
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		foreach (ChatEntry entry in entries) {
			GUILayout.BeginHorizontal();
			if (!entry.mine) {
				GUILayout.Label(entry.text);
			} else {
				GUI.contentColor = Color.yellow;
				GUILayout.Label(entry.text);
				GUI.contentColor = Color.white;
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(3);

		}
		// End the scrollview we began above.
		GUILayout.EndScrollView();

		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && inputField.Length > 0) {

			GameObject chatplayer = GameObject.Find(target.transform.name);
			chatplayer.transform.Find("Chat").GetComponent<TextMesh>().text = inputField;
			chatplayer.transform.Find("Chat").GetComponent<MeshRenderer>().material.color = Color.white;
			chatplayer.transform.Find("Chat").GetComponent<chatclear>().lastupdate = Time.time;

			ChatText(target.transform.name + " says: " + inputField, true);
			pioconnection.Send("Chat", inputField);
			inputField = "";
		}
		GUI.SetNextControlName("Chat input field");
		inputField = GUILayout.TextField(inputField);

		GUI.DragWindow();
	}


	void ChatText(string str, bool own) {
		var entry = new ChatEntry();
		entry.text = str;
		entry.mine = own;

		entries.Add(entry);

		if (entries.Count > 50)
			entries.RemoveAt(0);

		scrollPosition.y = 1000000;
	}
}
