using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    private Game _game;
    [SerializeField] private SceneView _sceneView;
    private void Awake()
    {
        Debug.Log("EntryPoint - Awake");
        _game = new Game();
        _game.Main();
        _sceneView.gameObject.SetActive(true);
    }

    private void Update()
    {
        _game.Tick();
    }
}
