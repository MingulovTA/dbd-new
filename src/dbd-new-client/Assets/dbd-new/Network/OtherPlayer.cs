using System;
using System.Collections.Generic;
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

    private void Awake()
    {
        _plModel = _plModels[0];
        ChangeTeam(0);
    }

    public void Init(string userId)
    {
        _userId = userId;
    }

    public void Move(Vector3 point)
    {
        _navMeshAgent.SetDestination(point);
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
    }
}
