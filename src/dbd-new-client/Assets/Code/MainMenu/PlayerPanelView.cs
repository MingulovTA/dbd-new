using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelView : MonoBehaviour
{
    private const string PLAYER_NAME_PREFS_KEY = "PlayerName";
    
    [SerializeField] private InputField _ifPlayerName;

    private void OnEnable()
    {
        _ifPlayerName.text = PlayerPrefs.GetString(PLAYER_NAME_PREFS_KEY);
        _ifPlayerName.onValueChanged.AddListener(PlayerNameChanged);
    }

    private void PlayerNameChanged(string newPlayerName)
    {
        PlayerPrefs.SetString(PLAYER_NAME_PREFS_KEY,newPlayerName);
    }
}
