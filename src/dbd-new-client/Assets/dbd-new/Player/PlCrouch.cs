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
            return Physics.SphereCast(castOrigin, _characterController.radius, transform.up, out _hit, 0.01f);
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
