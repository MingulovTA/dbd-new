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
            Vector3 cur = transform.position;
            //cur.y = 0;
            
            GroundedUpdate();
            ForceTick();
            
            var newVel =  (transform.position-cur) / Time.deltaTime;
            
            Debug.Log("");
            Debug.Log("_force = "+_force);
            Debug.Log("_controller = "+_controller.velocity);
            Debug.Log("newVel = "+newVel);
            
            Debug.Log("d = "+ (Vector3.Distance(new Vector3(_force.x,0,_force.z), newVel)));
            //_force = newVel;
        }

        private void GroundedUpdate()
        {
            Vector3 moveDirection = transform.right * Input.GetAxis("Horizontal") +
                                    transform.forward * Input.GetAxis("Vertical");
            if (moveDirection.magnitude > 1f) 
                moveDirection.Normalize();
            //moveDirection.y = -10;

            _controller.Move(moveDirection * walkSpeed * Time.deltaTime);
            _controller.Move(_gravity*Time.deltaTime);
            
            if (_force.magnitude > 0)
            {
                // Толкание применяется в том случае, если оно не равно нулю
                _controller.Move(_force * Time.deltaTime);
                //_force = Vector3.zero; // Обнуляем силу после применения
            }

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
            /*if (!_lastGrnd && _plCrouch.IsGrounded)
            {
                _force.x = 0;
                _force.z = 0;
            }*/
            
            if (!_lastGrnd && !_plCrouch.IsGrounded)
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
            }

            if (_plCrouch.IsWallDetected())
            {
                _force.x = 0;
                _force.z = 0;
            }

            if (_force.y > 0 && _plCrouch.IsCeilingDetected())
            {
                _force.y = 0;
            }
            _lastGrnd = _plCrouch.IsGrounded;
        }
    }
    
}