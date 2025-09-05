using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneView : MonoBehaviour
{
    [SerializeField] private Actor _actor;
    [SerializeField] private OtherPlayer _otherPlayerPrefab;
    [SerializeField] private List<OtherPlayer> _players = new List<OtherPlayer>();
    private void OnEnable()
    {
        Debug.Log("SceneView - OnEnable");
        Game.I.OnGameStart += GameStartHandler;
        Game.I.MsgReciever.SvUserJoined += SvUserJoinedHandler;
        Game.I.MsgReciever.SvUserLeft += SvUserLeftHandler;
        Game.I.MsgReciever.SvMove += SvMoveHandler;
    }

    private void OnDisable()
    {
        Game.I.OnGameStart -= GameStartHandler;
        Game.I.MsgReciever.SvUserJoined -= SvUserJoinedHandler;
        Game.I.MsgReciever.SvUserLeft -= SvUserLeftHandler;
        Game.I.MsgReciever.SvMove -= SvMoveHandler;
    }

    private void GameStartHandler()
    {
        _actor.Init(Game.I.PioManager.UserId);
    }

    private void SvUserJoinedHandler(string userId)
    {
        var newPlayer = Instantiate(_otherPlayerPrefab);
        newPlayer.Init(userId);
        _players.Add(newPlayer);
    }

    private void SvUserLeftHandler(string userId)
    {
        var leftPlayer = _players.FirstOrDefault(pl => pl.UserId == userId);
        if (leftPlayer != null)
        {
            _players.Remove(leftPlayer);
            Destroy(leftPlayer.gameObject);
        }
    }

    private void SvMoveHandler(string userId, Vector3 newPos)
    {
        Debug.Log("SvMoveHandler");
        var movedPlayer = _players.FirstOrDefault(pl => pl.UserId == userId);
        if (movedPlayer != null)
        {
            Debug.Log("MOVE");
            movedPlayer.Move(newPos);
        }
    }
}
