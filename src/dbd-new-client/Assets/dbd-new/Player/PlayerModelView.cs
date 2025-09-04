using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelView : MonoBehaviour
{
    private readonly Dictionary<MoveState, string> MoveStateAnimations = new Dictionary<MoveState, string>
    {
        {MoveState.Idle,"bowIdle2"},
        {MoveState.Walk,"bowWalk"},
        {MoveState.Run,"bowRun"},
    };

    [SerializeField] private Animation _animation;
    [SerializeField] private float _walkSpeedBound = 0.005f;

    private MoveState _moveState;
    private Vector3 _lastPos;
    private Transform _transform;


    private void Awake()
    {
        _transform = transform;
        _lastPos = _transform.position;
    }

    private void FixedUpdate()
    {
        if (Math.Abs(Vector3.Distance(_transform.position, _lastPos)) < 0.0001f)
        {
            ChangeState(MoveState.Idle);
            _lastPos = _transform.position;
            return;
        }
        
        if (Math.Abs(Vector3.Distance(_transform.position, _lastPos)) < _walkSpeedBound)
        {
            ChangeState(MoveState.Walk);
            _lastPos = _transform.position;
            return;
        }
        
        ChangeState(MoveState.Run);
        _lastPos = _transform.position;
    }
    

    private void ChangeState(MoveState newState)
    {
        if (_moveState==newState) return;
        _moveState = newState;
        _animation.CrossFade(MoveStateAnimations[_moveState],0.2f);
    }
}