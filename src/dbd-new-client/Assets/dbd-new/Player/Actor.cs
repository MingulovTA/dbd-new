using System.Collections;
using System.Collections.Generic;
using App.Player;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] private string _userId;
    [SerializeField] private int _teamId;
    
    [SerializeField] private CharacterController _plChar;
    [SerializeField] private PlMoveSettings _plMoveSettings;
    [SerializeField] private PlCrouch _plCrouch;
    [SerializeField] private Transform _transform;
    [SerializeField] private List<Animation> _plModels;
    
    private Animation _plModel;
    private Vector3 _lastPos;
    private bool _connected;

    public int TeamId => _teamId;
    public string UserId => _userId;
    public CharacterController PlChar => _plChar;

    public PlMoveSettings PlMoveSettings => _plMoveSettings;

    public PlCrouch PlCrouch => _plCrouch;

    public Transform T => _transform;
    
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _plModel = _plModels[0];
        ChangeTeam(0);
    }

    public void Init(string userId)
    {
        _userId = userId;
        _connected = true;
    }

    private void OnValidate()
    {
        _transform = transform;
        _plChar = GetComponent<CharacterController>();
        _plMoveSettings = GetComponent<PlMoveSettings>();
        _plCrouch = GetComponentInChildren<PlCrouch>();
    }


    public void Kill()
    {
        ChangeTeam(1);
    }

    public void Revive()
    {
        ChangeTeam(0);
    }
    
    public void ChangeTeam(int teamId)
    {
        _teamId = teamId;
        _plModel = _plModels[teamId];
        
        foreach (var plModel in _plModels)
            plModel.gameObject.SetActive(false);
        _plModel.gameObject.SetActive(true);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        
        OtherPlayer op = collision.transform.GetComponent<OtherPlayer>();
        if (op==null) return;
        if (op.UserId != Game.I.PioManager.UserId)
        {
            if (op.CurrentTeamId == 0 && _teamId == 1)
            {
                Debug.Log("Стукаем op");
                Game.I.PioManager.PioConnection.Send("ClKill", op.UserId);
                op.Kill();
            }

            if (op.CurrentTeamId == 1 && _teamId == 0)
            {
                Game.I.PioManager.PioConnection.Send("ClKilledBy",op.UserId);
                Debug.Log("Стукаем себя");
                Kill();
            }
        }
    }

    
    private void Update()
    {
        TryToAnimView();
        TryToSendSelfPos();
        TryToChangeTeam();
        TryToSendAngle();
    }

    private int _angleY;
    private int _lastAngleY;
    private void TryToSendAngle()
    {
        if (!_connected) return;
        _angleY = (int) transform.eulerAngles.y;
        if (_angleY < 0)
            _angleY += 360;
        if (_angleY > 360)
            _angleY -= 360;
        _angleY /= 8;

        if (_angleY != _lastAngleY)
        {
            _lastAngleY = _angleY;
            SendAngle();
        }
    }

    private void SendAngle()
    {
        Game.I.PioManager.PioConnection.Send("ClTurnY", transform.eulerAngles.y);
    }

    private void TryToAnimView()
    {
        if (Vector3.Distance(transform.position, _lastPos)>.001f)
            _plModel.CrossFade("Jog Forward",.25f);
        else
            _plModel.CrossFade("Idle",.25f);
        _lastPos = transform.position;
    }

    private Vector3Int _lastPosInt;
    private Vector3Int _posInt;
    private void TryToSendSelfPos()
    {
        if (!_connected) return;
        _posInt.x = (int)(_transform.position.x * 3);
        _posInt.y = (int)(_transform.position.y * 3);
        _posInt.z = (int)(_transform.position.z * 3);

        if (_posInt.x != _lastPosInt.x || _posInt.y != _lastPosInt.y || _posInt.z != _lastPosInt.z)
        {
            SendNewPos();
            _lastPosInt = _posInt;
        }
    }

    private void SendNewPos()
    {
        var p = _transform.position;
        Game.I.PioManager.PioConnection.Send("ClMove", p.x, p.y, p.z);
    }
    

    private void TryToChangeTeam()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Game.I.PioManager.PioConnection.Send("ClRevive",Game.I.PioManager.UserId);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Game.I.PioManager.PioConnection.Send("ClKill", Game.I.PioManager.UserId);
        }
    }
}
