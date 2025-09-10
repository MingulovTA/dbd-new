using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneView : MonoBehaviour
{
    [SerializeField] private Actor _actor;
    [SerializeField] private OtherPlayer _otherPlayerPrefab;
    [SerializeField] private List<OtherPlayer> _players = new List<OtherPlayer>();
    private void OnEnable()
    {
        Debug.Log("SceneView - OnEnable");
        Game.I.MsgReciever.SvUserJoined += SvUserJoinedHandler;
        Game.I.MsgReciever.SvUserLeft += SvUserLeftHandler;
        Game.I.MsgReciever.SvMove += SvMoveHandler;
        Game.I.MsgReciever.SvKill += SvKillHandler;
        Game.I.MsgReciever.SvRestartGame += SvRestartGameHandler;
        Game.I.MsgReciever.SvRevive += SvReviveHandler;
        Game.I.MsgReciever.SvTurnY += SvTurnYHandler;
        
        Game.I.MsgReciever.RecivingStart();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _actor.Init(Game.I.PioManager.UserId);
    }

    private void OnDisable()
    {
        Game.I.MsgReciever.SvUserJoined -= SvUserJoinedHandler;
        Game.I.MsgReciever.SvUserLeft -= SvUserLeftHandler;
        Game.I.MsgReciever.SvMove -= SvMoveHandler;
        Game.I.MsgReciever.SvKill -= SvKillHandler;
        Game.I.MsgReciever.SvRestartGame -= SvRestartGameHandler;
        Game.I.MsgReciever.SvRevive -= SvReviveHandler;
        Game.I.MsgReciever.SvTurnY -= SvTurnYHandler;
        Game.I.MsgReciever.RecivingStop();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SvTurnYHandler(string turnerId, float angleY)
    {
        var turner = _players.FirstOrDefault(pl => pl.UserId == turnerId);
        if (turner != null)
            turner.Turn(angleY);
    }

    private void SvReviveHandler(string reviverId)
    {
        if (Game.I.PioManager.UserId == reviverId)
        {
            _actor.Revive();
        }
        else
        {
            var reviver = _players.FirstOrDefault(pl => pl.UserId == reviverId);
            if (reviver != null)
                reviver.Revive();
        }
    }

    private void SvRestartGameHandler()
    {
        _actor.Revive();
        foreach (var otherPlayer in _players)
            otherPlayer.Revive();
    }

    private void SvKillHandler(string killerId, string victumId)
    {
        if (victumId == Game.I.PioManager.UserId)
        {
            _actor.Kill();
        }
        var victum = _players.FirstOrDefault(pl => pl.UserId == victumId);
        if (victum != null)
            victum.Kill();
        
        
    }

    private void SvUserJoinedHandler(string userId, Vector3 pos, int teamId)
    {
        var newPlayer = Instantiate(_otherPlayerPrefab);
        newPlayer.transform.position = pos;
        newPlayer.Init(userId);
        newPlayer.ChangeTeam(teamId);
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
        var movedPlayer = _players.FirstOrDefault(pl => pl.UserId == userId);
        if (movedPlayer != null)
        {
            movedPlayer.Move(newPos);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Game.I.PioManager.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
