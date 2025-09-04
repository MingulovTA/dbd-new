using System;
using App.Player;
using UnityEngine;
using UnityEngine.Serialization;

public class Actor : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private ActorTeam _team;

    [SerializeField] private CharacterController _plChar;
    [SerializeField] private PlMoveSettings _plMoveSettings;
    [SerializeField] private PlCrouch _plCrouch;
    [SerializeField] private Transform _transform;

    public ActorTeam Team => _team;
    public string Name => _name;
    public CharacterController PlChar => _plChar;

    public PlMoveSettings PlMoveSettings => _plMoveSettings;

    public PlCrouch PlCrouch => _plCrouch;

    public Transform T => _transform;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnValidate()
    {
        _transform = transform;
        _plChar = GetComponent<CharacterController>();
        _plMoveSettings = GetComponent<PlMoveSettings>();
        _plCrouch = GetComponentInChildren<PlCrouch>();
    }
}
