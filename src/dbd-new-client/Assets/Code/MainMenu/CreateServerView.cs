using UnityEngine;
using UnityEngine.UI;

public class CreateServerView : MonoBehaviour
{
    [SerializeField] private InputField _ifServerName;
    [SerializeField] private Button _btnCreateServer;
    [SerializeField] private GameObject _loadScreen;

    private void Awake()
    {
        _btnCreateServer.onClick.AddListener(CreateServerBtnClick);
    }

    private void CreateServerBtnClick()
    {
        Game.I.PioManager.CreateRoom(_ifServerName.text);
        _loadScreen.SetActive(true);
    }

    private void Update()
    {
        _btnCreateServer.interactable = Game.I.PioManager.IsConnected && _ifServerName.text.Length >= 3;
    }
}
