using UnityEngine;

namespace App.Player
{
    public class PlMoveSettings : MonoBehaviour
    {
        [SerializeField] private float walkingSpeed = 6f;
        [SerializeField] private float runningSpeed = 8f;
        [SerializeField] private float jumpSpeed = 6;
        [SerializeField] private float gravity = 17;


        public float WalkingSpeed => walkingSpeed;
        public float RunningSpeed => runningSpeed;
        public float JumpSpeed => jumpSpeed;
        public float Gravity => gravity;
    }
}
