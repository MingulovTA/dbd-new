using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class OtherPlayer : MonoBehaviour
{
    [SerializeField] private string _userId;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private List<Animation> _plModels;
    [SerializeField] private Transform _target;

    private int _currentTeamId;
    private Animation _plModel;
    private Vector3 _lastPos;
    public string UserId => _userId;
    public int CurrentTeamId => _currentTeamId;

    private void Awake()
    {
        _plModel = _plModels[0];
        ChangeTeam(0);
    }

    public void Init(string userId)
    {
        _userId = userId;
        _targetMovePoint = transform.position;
    }

    public void Move(Vector3 point)
    {
        Debug.Log("Other player Move");
        //_navMeshAgent.SetDestination(point);
        SetDestination(point);
    }

    private Vector3 _targetMovePoint;

    private void SetDestination(Vector3 targetMovePoint)
    {
        _targetMovePoint = targetMovePoint;
    }
    

    public void ChangeTeam(int teamId)
    {
        _currentTeamId = teamId;
        _plModel = _plModels[teamId];
        
        foreach (var plModel in _plModels)
            plModel.gameObject.SetActive(false);
        _plModel.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _lastPos)>.001f)
            _plModel.CrossFade("Jog Forward",.25f);
        else
            _plModel.CrossFade("Idle",.25f);
        _lastPos = transform.position;

        transform.position = Vector3.MoveTowards(transform.position, _targetMovePoint, Time.deltaTime * 5);
    }

    public void Revive()
    {
        ChangeTeam(0);
    }

    public void Kill()
    {
        ChangeTeam(1);
    }

    private Vector3 _targetAngle;
    private Tween _rotateTween;
    public void Turn(float angleY)
    {
        _targetAngle.y = angleY;
        _rotateTween?.Kill();
        _rotateTween = _plModel.transform.DORotate(new Vector3(0, angleY, 0), .4f, RotateMode.Fast)
            .SetEase(Ease.InOutSine);
    }
}
