using UnityEngine;

namespace App.Player
{
    public class PlCrouch : MonoBehaviour
    {
        public bool UnCrouchIsAvaliliable => _unCrouchAvailible;
    
        private bool _unCrouchAvailible;

        public bool IsCeilDetectedForceCheck()
        {
            RaycastHit hit;
            float distanceToObstacle = 0;
            if (Physics.SphereCast(transform.position, 0.4f, transform.up, out hit, 1))
            {
                distanceToObstacle = hit.distance;
                if (distanceToObstacle > 1)
                    _unCrouchAvailible = true;
                else
                    _unCrouchAvailible = false;
            }
            else
            {
                _unCrouchAvailible = true;
            }

            return !_unCrouchAvailible;
        }
        private void Update()
        {
            IsCeilDetectedForceCheck();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            _unCrouchAvailible = false;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
