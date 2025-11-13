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
        [SerializeField] private Vector3 _gravity = Vector3.zero;

        public Vector3 Center => transform.position + _controller.center;


        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("OnCollisionEnter");
            _force = Vector3.down*10;
        }
        

        private void Update()
        {
            GroundedUpdate();
            ForceTick();
        }

        private Vector3 _lastFrameInput;
        [SerializeField] private Vector3 _vel;
        private void GroundedUpdate()
        {
            Vector3 moveDirection = transform.right * Input.GetAxis("Horizontal") +
                                    transform.forward * Input.GetAxis("Vertical");
            if (moveDirection.magnitude > 1f) 
                moveDirection.Normalize();
            moveDirection = moveDirection * walkSpeed;

            //Calc vel as input
            _vel -= _lastFrameInput;
            _vel += moveDirection;
            _lastFrameInput = moveDirection;
            
            if (Input.GetButton("Jump") && _plCrouch.IsGrounded)
                _vel += Vector3.up*10f;

            //gravity
            _vel = Vector3.MoveTowards(_vel, new Vector3(_vel.x,0,_vel.z), Time.deltaTime*10);
            
            if (_plCrouch.IsGrounded)
            {
                _gravity = Vector3.down*10;
                _vel.y = 0;
            }
            else
            {
                
            }
            
            _controller.Move(_vel*Time.deltaTime);
            return;
            
            
            
            
            //moveDirection.y = -10;
            _force -= _lastFrameInput;
            _force += moveDirection * walkSpeed * Time.deltaTime;
            _lastFrameInput = moveDirection * walkSpeed * Time.deltaTime;
            Vector3 pos = transform.position;
            _controller.Move(_gravity*Time.deltaTime+_force * Time.deltaTime);

            Vector3 correction = pos-transform.position;
            //_force = correction;
            Debug.Log("correction = "+correction.magnitude);
            Debug.Log("correction = "+correction.magnitude);
            if (Input.GetButton("Jump")&&_plCrouch.IsGrounded)
            {
                transform.position = new Vector3(transform.position.x,transform.position.y+.1f,transform.position.z);
                AddForce(Vector3.up*10);
            }
        }

        private void AirUpdate()
        {
            
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

            Vector3 pos = transform.position;
            
            _controller.Move(velocity * Time.deltaTime);
            
            Vector3 correction = transform.position-pos;
            _force = correction;
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
            _force = Vector3.MoveTowards(_force, new Vector3(_force.x,0,_force.z), Time.deltaTime*10);

            if (_plCrouch.IsGrounded)
                _force = Vector3.MoveTowards(_force, new Vector3(0,_force.y,0), Time.deltaTime*100);

            
            
            
            /*if (!_lastGrnd && !_plCrouch.IsGrounded)
            {
                _gravity += Vector3.down*10*Time.deltaTime;
            }

            if (_lastGrnd && _plCrouch.IsGrounded)
            {
                _gravity = Vector3.down*10;
            }

            if (_lastGrnd && !_plCrouch.IsGrounded)
            {
                //_force = _controller.velocity;
                _gravity = Vector3.zero;
            }*/

            /*if (_plCrouch.IsWallDetected())
            {
                _force.x = 0;
                _force.z = 0;
            }*/

            if (_force.y > 0 && _plCrouch.IsCeilingDetected())
            {
                _force.y = 0;
            }
            _lastGrnd = _plCrouch.IsGrounded;
        }
    }
    
}