using UnityEngine;

namespace App
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        public Camera Camera => _camera;
    }
}
