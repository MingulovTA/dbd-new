using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    private Game _game;
    private void Awake()
    {
        if (_game!=null) return;
        Debug.Log("EntryPoint - Awake");
        _game = new Game();
        _game.Main();
    }
}
