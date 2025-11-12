using System;
using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;
using Random = UnityEngine.Random;

public class PioManager
{
    private const bool IS_DEVELOPMENT_SERVER = false;
    private const string GAME_ID = "dbd-new-qrdesbn2rku1glgjblamhq";
    public const string DEFAULT_ROOM_TYPE = "UnityMushrooms";

    private Client _client;
    private string _userId;
    private Connection _pioConnection;
    private MsgReciever _msgReciever = new MsgReciever();
    private Action _onConnectedToServer;

    private bool _isConnected;

    public bool IsConnected => _isConnected;

    //public Connection PioConnection => _pioConnection;

    public Client Client => _client;

    public string UserId => _userId;

    public event Action OnSuccessConnected; 
    public event Action OnJoinRoomSuccess; 
    public event Action OnJoinRoomFailed; 
    public event Action OnLeaveRoom; 
    

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
        _isConnected = true;
        Debug.Log("PIO: OnConnectSuccess Successfully connected to Player.IO");
        if (IS_DEVELOPMENT_SERVER)
            client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);


        OnSuccessConnected?.Invoke();
    }
    
    private void JoinedRoomSuccess(Connection connection)
    {
        Debug.Log("PIO: JoinedRoomSuccess");
        _pioConnection = connection;
        _pioConnection.OnMessage += MsgHandler;
        _onConnectedToServer?.Invoke();
        OnJoinRoomSuccess?.Invoke();
    }
    
    private void OnConnectFailed(PlayerIOError error)
    {
        Debug.Log("PIO: Error connecting: " + error.ToString());
        OnJoinRoomFailed?.Invoke();
    }

    private void JoinedRoomFailed(PlayerIOError error)
    {
        Debug.Log("PIO: Error Joining Room: " + error.ToString());
    }

    private void MsgHandler(object sender, Message m)
    {
        _msgReciever.Recieve(m);
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        _client.Multiplayer.CreateJoinRoom(roomInfo.Id, DEFAULT_ROOM_TYPE, true, null,
            null,
            JoinedRoomSuccess, JoinedRoomFailed
        );
    }

    public void CreateRoom(string serverName)
    {
        _client.Multiplayer.CreateJoinRoom(serverName, DEFAULT_ROOM_TYPE, true, null,
            null, JoinedRoomSuccess, JoinedRoomFailed
        );
    }

    public void LeaveRoom()
    {
        Debug.Log("PIO: Leave Room");
        _pioConnection.Disconnect();
        Debug.Log("PIO: Leave Room 2");
    }
    
    public void Send(string type, params object[] parameters)
    {
        Debug.Log($"ClSend: {type}");
        _pioConnection.Send(type,parameters);
    }
}
