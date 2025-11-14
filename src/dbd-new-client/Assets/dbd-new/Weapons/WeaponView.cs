using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private float _attackDelay = 0.5f;
    [SerializeField] private bool _autoAttack = false;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _fireWp;

    private float _attackTimer;
    
    private void Update()
    {
        if (_autoAttack)
        {
            if (Input.GetMouseButton(0))
                TryToAttack();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                TryToAttack();
        }

        if (_attackTimer >= 0)
            _attackTimer -= Time.deltaTime;
    }

    private void TryToAttack()
    {
        if (_attackTimer>0) return;
        Attack();
        _attackTimer = _attackDelay;
    }

    private void Attack()
    {
        Bullet bullet = Instantiate(_bulletPrefab,_fireWp);
        bullet.transform.SetParent(null);
    }
}
