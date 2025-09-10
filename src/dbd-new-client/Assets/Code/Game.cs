using System;
using System.Collections;
using App.Services.Runners;
using UnityEngine;
using Object = UnityEngine.Object;

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
    private ICoroutineRunner _coroutineRunner;

    public PioManager PioManager => _pioManager;
    public MsgReciever MsgReciever => _msgReciever;

    public event Action OnGameStart;


    public void Main()
    {
        _msgReciever = new MsgReciever();
        _pioManager = new PioManager(_msgReciever, OnConnectedToServer);
        _coroutineRunner = CoroutineRunner();
        
        Init();
    }

    private ICoroutineRunner CoroutineRunner()
    {
        GameObject go = new GameObject();
        Object.DontDestroyOnLoad(go);
        go.name = "CoroutineRunner";
        CoroutineRunner cr = go.AddComponent<CoroutineRunner>();
        return cr;
    }

    private void Init()
    {
        _pioManager.Init();
        _coroutineRunner.Run(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while (true)
        {
            yield return null;
            Tick();
        }
    }

    private void OnConnectedToServer()
    {
        OnGameStart?.Invoke();
    }

    private void Tick()
    {
        _msgReciever.Tick();
    }
}
