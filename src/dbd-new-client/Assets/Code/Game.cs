using System;
using UnityEngine;

public class Game
{
    private static Game _instance;

    public static Game I => _instance;

    public Game()
    {
        _instance = this;
    }


    private PioManager _pioManager;
    private MsgReciever _msgReciever;

    public PioManager PioManager => _pioManager;
    public MsgReciever MsgReciever => _msgReciever;

    public event Action OnGameStart;


    public void Main()
    {
        _msgReciever = new MsgReciever();
        _pioManager = new PioManager(_msgReciever, OnConnectedToServer);
        
        Init();
    }

    private void Init()
    {
        _pioManager.Init();
    }

    private void OnConnectedToServer()
    {
        OnGameStart?.Invoke();
    }

    public void Tick()
    {
        _msgReciever.Tick();
    }
}
