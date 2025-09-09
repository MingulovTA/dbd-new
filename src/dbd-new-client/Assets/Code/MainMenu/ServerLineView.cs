using System;
using PlayerIOClient;
using UnityEngine;
using UnityEngine.UI;

public class ServerLineView : MonoBehaviour
{
    [SerializeField] private Text _serverNameText;
    [SerializeField] private Text _playersText;
    [SerializeField] private Text _mapText;

    [SerializeField] private Button _btnJoin;

    private RoomInfo _roomInfo;
    private Action<RoomInfo> _onJoinClick;
    public void Init(RoomInfo roomInfo, Action<RoomInfo> onJoinClick)
    {
        _onJoinClick = onJoinClick;
        _roomInfo = roomInfo;
        _serverNameText.text = roomInfo.Id;
        _playersText.text = roomInfo.OnlineUsers.ToString();
    }

    private void Awake()
    {
        _btnJoin.onClick.AddListener(BtnJoinClickHangled);
    }

    private void BtnJoinClickHangled()
    {
        _onJoinClick?.Invoke(_roomInfo);
    }
}
