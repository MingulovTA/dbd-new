using UnityEngine;

public class Game : MonoBehaviour
{
    private PioManager _pioManager;
    private MsgReciever _msgReciever;
    
    public void Main()
    {
        _msgReciever = new MsgReciever();
        _pioManager = new PioManager(_msgReciever,OnConnectedToServer);
        
        Init();
    }

    private void Init()
    {
        _pioManager.Init();
    }

    private void OnConnectedToServer()
    {
        
    }
}
