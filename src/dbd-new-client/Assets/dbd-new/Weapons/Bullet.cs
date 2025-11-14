using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GameObject _exposion;

    private void OnValidate()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var exp = Instantiate(_exposion, null);
        exp.transform.position = collision.GetContact(0).point;
        Destroy(gameObject);
    }
    
    private void FixedUpdate()
    {
        _rb.linearVelocity = transform.forward*20;
    }
}
