using UnityEngine;

namespace App.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private const float walkSpeed = 3f;
        private const float jumpHeight = 2f;
        
        [SerializeField] private PlCrouch _plCrouch;
        [SerializeField] private CharacterController _controller;

        private float ySpeed = 0f;
        private float gravity = -9.81f;
        private Vector3 velocity;
        private bool isInAir = false;

        private void Update()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            Vector3 moveDirection = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

            if (moveDirection.magnitude > 1f)
                moveDirection.Normalize();

            if (_controller.isGrounded)
            {
                ySpeed = -0.5f; 
                if (Input.GetButtonDown("Jump"))
                    ySpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);

                isInAir = false;
            }
            else
            {
                ySpeed += gravity * Time.deltaTime;
                isInAir = true;
            }

            velocity = moveDirection * walkSpeed;
            velocity.y = ySpeed;

            if (!_controller.isGrounded && ySpeed > 0 && _plCrouch.IsCeilingDetected())
                ySpeed = 0;

            _controller.Move(velocity * Time.deltaTime);
        }

        public void AddForce(Vector3 force)
        {
            if (!isInAir)
            {
                velocity += force;
            }
        }
    }
}