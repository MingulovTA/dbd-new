using UnityEngine;

namespace App.Player
{
    public class PlCrouch : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private LayerMask _castMask;

        private RaycastHit _hit;

        [SerializeField] private Collider[] _wallsBuffer;
        public bool IsWallDetected()
        {
            Vector3 castOrigin = transform.position;
            castOrigin.y += _characterController.center.y;

            _wallsBuffer = Physics.OverlapSphere(castOrigin, _characterController.radius + 0.1f, _castMask);

            return _wallsBuffer.Length > 0;
        }

        private void OnDrawGizmos()
        {
            Vector3 castOrigin = transform.position;
            castOrigin.y += _characterController.center.y;
            Gizmos.DrawSphere(castOrigin, _characterController.radius + 0.1f);
        }
        public bool IsCeilingDetected()
        {
            Vector3 castOrigin = transform.position;
            castOrigin.y += _characterController.height - _characterController.radius;
            var isHit = Physics.SphereCast(castOrigin, _characterController.radius-0.01f, transform.forward, out _hit, 0.02f);
            if (!isHit) return false;
            return !_hit.collider.isTrigger;
        }

        public bool IsGrounded
        {
            get
            {
                    return _characterController.isGrounded;
                Vector3 castOrigin = transform.position;
                castOrigin.y += _characterController.radius;
                var isHit = Physics.SphereCast(castOrigin, _characterController.radius-0.01f, Vector3.down, out _hit, 0.02f);
                if (!isHit) return false;
                return !_hit.collider.isTrigger;
            }
        }

        [SerializeField] private bool _isGrounded;
        [SerializeField] private bool _isCeiling;
        [SerializeField] private bool _isWallDetected;

        private void Update()
        {
            _isGrounded = IsGrounded;
            _isCeiling = IsCeilingDetected();
            _isWallDetected = IsWallDetected();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
