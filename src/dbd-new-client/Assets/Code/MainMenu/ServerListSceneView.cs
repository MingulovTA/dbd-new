using System.Collections.Generic;
using NUnit.Framework;
using PlayerIOClient;
using UnityEngine;
using UnityEngine.UI;

public class ServerListSceneView : MonoBehaviour
{
    [SerializeField] private MainMenuSceneView _mainMenuSceneView;
    [SerializeField] private ServerLineView _serverLineViewPrefab;
    [SerializeField] private Button _btnRefresh;

    private List<ServerLineView> _serverLineViews = new List<ServerLineView>();
    
    private void OnEnable()
    {
        _serverLineViewPrefab.gameObject.SetActive(false);
        _btnRefresh.interactable = false;
        _btnRefresh.onClick.AddListener(Refresh);
        Game.I.PioManager.OnSuccessConnected += Refresh;
        
        if (Game.I.PioManager.IsConnected)
            Refresh();
    }

    private void OnDisable()
    {
        Game.I.PioManager.OnSuccessConnected -= Refresh;
    }

    private void Refresh()
    {
        Debug.Log("Refresh");
        foreach (var slv in _serverLineViews)
            Destroy(slv.gameObject);
        _serverLineViews.Clear();
        
        _btnRefresh.interactable = false;
        var mp = Game.I.PioManager.Client.Multiplayer;
        string roomType = PioManager.DEFAULT_ROOM_TYPE;
        mp.ListRooms(roomType,null,20,0,OnRefreshSuccess, OnRefreshFailed);
    }

    private void OnRefreshFailed(PlayerIOError value)
    {
        Debug.Log("PIO: Refresh failed. Reason:" + value.Message);
        _btnRefresh.interactable = true;
    }

    private void OnRefreshSuccess(RoomInfo[] value)
    {
        _btnRefresh.interactable = true;
        foreach (var roomInfo in value)
        {
            var slv = Instantiate(_serverLineViewPrefab, _serverLineViewPrefab.transform.parent);
            slv.Init(roomInfo,OnJoinClickHandler);
            slv.gameObject.SetActive(true);
            _serverLineViews.Add(slv);
        }
    }

    private void OnJoinClickHandler(RoomInfo roomInfo)
    {
        _mainMenuSceneView.JoinTo(roomInfo);
    }
}
