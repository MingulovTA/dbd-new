using System;
using App.Player.Use;
using UnityEngine;

public class PlayerUseCaster : MonoBehaviour
{
    private static bool _isUseAvailiable;
    private static bool _isUseAvailiableLast;

    public static bool IsUseAvailiable => _isUseAvailiable;

    public static event Action UsingStateUpdated;

    private RaycastHit _hit;
    private Transform _targetTransform;
    private static IPlayerUsable _targetUsable;
    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out _hit, 1.5f))
        {
            if (_hit.transform != _targetTransform)
            {
                _targetTransform = _hit.transform;
                _targetUsable = _targetTransform.GetComponent<IPlayerUsable>();
                _isUseAvailiable = _targetUsable != null;
            }
        }
        else
        {
            _targetTransform = null;
            _isUseAvailiable = false;
        }
        
        if (_isUseAvailiable!=_isUseAvailiableLast)
            UsingStateUpdated?.Invoke();
        _isUseAvailiableLast = _isUseAvailiable;

        if (_isUseAvailiable && Input.GetButtonDown("Use"))
        {
            _targetUsable.Use();
        }
    }

    public static void Use()
    {
        if (_isUseAvailiable)
            _targetUsable.Use();
    }
}