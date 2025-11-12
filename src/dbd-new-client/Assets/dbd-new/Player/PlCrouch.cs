using UnityEngine;

namespace App.Player
{
    public class PlCrouch : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;

        private RaycastHit _hit;
        public bool IsCeilingDetected()
        {
            Vector3 castOrigin = transform.position;
            castOrigin.y += _characterController.height - _characterController.radius;
            var isHit = Physics.SphereCast(castOrigin, _characterController.radius-0.01f, transform.up, out _hit, 0.02f);
            if (!isHit) return false;
            return !_hit.collider.isTrigger;

        }

        public bool IsGrounded
        {
            get
            {
                Vector3 castOrigin = transform.position;
                castOrigin.y += _characterController.radius;
                var isHit = Physics.SphereCast(castOrigin, _characterController.radius-0.01f, Vector3.down, out _hit, 0.02f);
                if (!isHit) return false;
                return !_hit.collider.isTrigger;
            }
        }

        [SerializeField] private bool _isGrounded;
        [SerializeField] private bool _isCeiling;

        private void Update()
        {
            _isGrounded = IsGrounded;
            _isCeiling = IsCeilingDetected();
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
