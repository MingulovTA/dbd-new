using System;
using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;
using Random = UnityEngine.Random;

public class PioManager
{
    private const bool IS_DEVELOPMENT_SERVER = true;
    private const string GAME_ID = "dbd-new-qrdesbn2rku1glgjblamhq";

    private Client _client;
    private string _userId;
    private Connection _pioConnection;
    private List<Message> _msgList = new List<Message>(); //  Messsage queue implementation
    private MsgReciever _msgReciever = new MsgReciever();
    private Action _onConnectedToServer;

    public PioManager(MsgReciever msgReciever, Action onConnectedToServer)
    {
        _msgReciever = msgReciever;
        _onConnectedToServer = onConnectedToServer; 
    }

    public void Init()
    {
        Debug.Log("PIO: Starting");

        Application.runInBackground = true;
        _userId = $"Guest {Random.Range(0, 10000)}";
        PlayerIO.Authenticate(GAME_ID, "public",
            new Dictionary<string, string> {{"userId", _userId},},
            null, OnConnectSuccess, OnConnectFailed
        );
    }
    
    private void OnConnectSuccess(Client client)
    {
        _client = client;
        Debug.Log("PIO: OnConnectSuccess Successfully connected to Player.IO");
        if (IS_DEVELOPMENT_SERVER)
            client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);

        client.Multiplayer.CreateJoinRoom("UnityDemoRoom", "UnityMushrooms", true, null,
            null,
            JoinedRoomSuccess, JoinedRoomFailed
        );
    }
    
    private void JoinedRoomSuccess(Connection connection)
    {
        Debug.Log("PIO: JoinedRoomSuccess");
        _pioConnection = connection;
        _pioConnection.OnMessage += MsgHandler;
    }
    
    private void OnConnectFailed(PlayerIOError error)
    {
        Debug.Log("PIO: Error connecting: " + error.ToString());
    }

    private void JoinedRoomFailed(PlayerIOError error)
    {
        Debug.Log("PIO: Error Joining Room: " + error.ToString());
    }

    private void MsgHandler(object sender, Message m)
    {
        _msgList.Add(m);
    }
}
