using PlayerIOClient;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneView : MonoBehaviour
{
    [SerializeField] private GameObject _loadScreen;

    private void Awake()
    {
        _loadScreen.SetActive(true);
        if (Game.I.PioManager.IsConnected)
            _loadScreen.SetActive(false);
    }

    private void OnEnable()
    {
        Game.I.PioManager.OnJoinRoomSuccess += JoinRoomSuccessHandler;
        Game.I.PioManager.OnJoinRoomFailed += JoinRoomFailedHandler;
        Game.I.PioManager.OnSuccessConnected += OnSuccessConnectedHandler;
    }

    private void OnDisable()
    {
        Game.I.PioManager.OnJoinRoomSuccess -= JoinRoomSuccessHandler;
        Game.I.PioManager.OnJoinRoomFailed -= JoinRoomFailedHandler;
        Game.I.PioManager.OnSuccessConnected -= OnSuccessConnectedHandler;
    }

    public void JoinTo(RoomInfo roomInfo)
    {
        _loadScreen.gameObject.SetActive(true);
        Game.I.PioManager.JoinRoom(roomInfo);
    }

    private void OnSuccessConnectedHandler()
    {
        _loadScreen.SetActive(false);
    }

    private void JoinRoomSuccessHandler()
    {
        SceneManager.LoadScene("Game");
    }

    private void JoinRoomFailedHandler()
    {
        _loadScreen.SetActive(false);
    }
}
