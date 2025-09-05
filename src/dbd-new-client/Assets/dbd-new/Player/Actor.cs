using System;
using System.Collections;
using App.Player;
using UnityEngine;
using UnityEngine.Serialization;

public class Actor : MonoBehaviour
{
    [SerializeField] private string _userId;
    [SerializeField] private ActorTeam _team;

    [SerializeField] private CharacterController _plChar;
    [SerializeField] private PlMoveSettings _plMoveSettings;
    [SerializeField] private PlCrouch _plCrouch;
    [SerializeField] private Transform _transform;

    public ActorTeam Team => _team;
    public string UserId => _userId;
    public CharacterController PlChar => _plChar;

    public PlMoveSettings PlMoveSettings => _plMoveSettings;

    public PlCrouch PlCrouch => _plCrouch;

    public Transform T => _transform;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Init(string userId)
    {
        _userId = userId;
        StartCoroutine(SendingPos());
    }

    private void OnValidate()
    {
        _transform = transform;
        _plChar = GetComponent<CharacterController>();
        _plMoveSettings = GetComponent<PlMoveSettings>();
        _plCrouch = GetComponentInChildren<PlCrouch>();
    }

    private IEnumerator SendingPos()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            var p = transform.position;
            Game.I.PioManager.PioConnection.Send("ClMove", p.x, p.y, p.z);
        }
    }
    
    
}
