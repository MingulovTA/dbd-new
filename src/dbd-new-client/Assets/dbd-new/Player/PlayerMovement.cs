using System;
using UnityEngine;

namespace App.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private const float walkSpeed = 6f;
        private const float jumpHeight = 2f;
        [SerializeField] private PlCrouch _plCrouch;
        [SerializeField] private CharacterController _controller;
        private float ySpeed = 0f;
        private float gravity = -9.81f;

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("OnCollisionEnter");
            _force = Vector3.down*10;
        }

        private void Update()
        {
            
            MovePlayer();
            ForceTick();
        }
        private void MovePlayer()
        {
            Vector3 moveDirection = transform.right * Input.GetAxis("Horizontal") +
                                    transform.forward * Input.GetAxis("Vertical");
            if (moveDirection.magnitude > 1f) 
                moveDirection.Normalize();

            Vector3 velocity;
            if (_plCrouch.IsGrounded)
            {
                if (Input.GetButton("Jump"))
                    AddForce(Vector3.up*15);
                else
                    _force.y = -10;
                
                
            }
            else
            {
                velocity = _force;
            }
            velocity = moveDirection * walkSpeed + _force;
            
            
            if (!_plCrouch.IsGrounded && _force.y > 0 && _plCrouch.IsCeilingDetected())
            {
                _force.y = 0;
            }
            _controller.Move(velocity * Time.deltaTime);
        }

        [SerializeField] private Vector3 _force;

        public void SetForceY(float y)
        {
            _force.y = y;
            //_controller.Move(Vector3.up * 0.1f);
        }

        public void AddForce(Vector3 force)
        {
            _force += force;
            //_controller.Move(Vector3.up * 0.1f);
        }

        private float _airTime;
        private bool _lastGrnd;
        private void ForceTick()
        {
            _force = Vector3.MoveTowards(_force, Vector3.down*10, Time.deltaTime*10f);
            
            /*if (_plCrouch.IsGrounded)
                _force = Vector3.MoveTowards(_force, Vector3.down*10, Time.deltaTime*10f);
            else
            {
                _force = Vector3.MoveTowards(_force, new Vector3(_force.x,_force.y>-10?-10:_force.y,_force.z), Time.deltaTime*10f);
            }*/

            if (_lastGrnd && !_plCrouch.IsGrounded)
                _force = _controller.velocity;

            if (_lastGrnd && !_plCrouch.IsGrounded&&_force.y<=0)
                _force.y = 0;

            if (!_lastGrnd && _plCrouch.IsGrounded)
            {
                _force.y = -10;
            }
            
            _lastGrnd = _plCrouch.IsGrounded;
        }
    }
    
}