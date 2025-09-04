using UnityEngine;

namespace App.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private const float InputHorisontalVelocityMax = 0.1f;
        
        [SerializeField] private Actor _actor;
        [SerializeField] private PlayerController _playerController;
        
        private Vector3 _moveDirection = Vector3.zero;
        private float _inputHorisontalVelocity;
        private bool _isCrouch;
        private Vector3 _posInLastFrame;
        private Vector3 _lastVelocity;
        private bool _inputJumpPressed;

        private void Update()
        {
            if (Input.GetAxis("Jump") > 0.01f)
                _inputJumpPressed = true;
            else
                _inputJumpPressed = false;
            
            bool isRunning = Input.GetKey(KeyCode.LeftShift);


            float dMoveX = Input.GetAxis("Horizontal");
            float dMoveY = Input.GetAxis("Vertical");
            
            
            Vector2 v = new Vector2(dMoveX, dMoveY);
            float ioMovePower = Mathf.Clamp01(Vector2.Distance(Vector2.zero, v));
            v = v.normalized;
            _inputHorisontalVelocity = v.magnitude*ioMovePower*0.1f;

            Vector3 forward = _actor.T.TransformDirection(Vector3.forward);
            Vector3 right = _actor.T.TransformDirection(Vector3.right);
            
            
            
            float curSpeedX = (isRunning ? _actor.PlMoveSettings.RunningSpeed : _actor.PlMoveSettings.WalkingSpeed) * v.y;
            float curSpeedY = (isRunning ? _actor.PlMoveSettings.RunningSpeed : _actor.PlMoveSettings.WalkingSpeed) * v.x;
            float movementDirectionY = _moveDirection.y;
            
            if (_actor.PlChar.isGrounded)
            {
                _moveDirection = (forward * curSpeedX * (_inputHorisontalVelocity / InputHorisontalVelocityMax)) +
                                 (right * curSpeedY * (_inputHorisontalVelocity / InputHorisontalVelocityMax));
                if (_isCrouch)
                    _moveDirection = _moveDirection * 0.5f;
                _lastVelocity = _moveDirection;
                

                /*if (_moveDirection.x > 0.1f || _moveDirection.x < -0.1f || _moveDirection.z > 0.1f ||
                    _moveDirection.z < -0.1f)
                {
                    _stepTimer -= Time.deltaTime;
                    if (_stepTimer <= 0)
                    {
                        _stepTimer = isRunning?0.3f:0.4f;
//                        if (!_isCrouch)
//                            _soundService.Play(_stepSfxKeys.GetRandomItem());
                    }
                }
                else
                {
                    _stepTimer = isRunning?0.3f:0.4f;
                }*/

            }
            else
            {
                //_stepTimer = isRunning?0.3f:0.4f;
                Vector3 moveAirCorrection =
                    (forward * curSpeedX * (_inputHorisontalVelocity / InputHorisontalVelocityMax)) +
                    (right * curSpeedY * (_inputHorisontalVelocity / InputHorisontalVelocityMax));
                _moveDirection = _lastVelocity + moveAirCorrection * 0.25f;
            }
            
            if (_inputJumpPressed && _actor.PlChar.isGrounded)
            {
                //_soundService.Play("Player/Jump");
                _inputJumpPressed = false;
                _moveDirection.y = _actor.PlMoveSettings.JumpSpeed;
            }
            else
            {
                _moveDirection.y = movementDirectionY;
            }
            
            if (Input.GetAxis("Crouch")>0.5f)
                TryToCrouch();
            else
                TryToUnCrouch();
            MoveTick();
        }

        private void MoveTick()
        {
            
            _actor.PlChar.center = Vector3.up * _actor.PlChar.height / 2;
            if (!_actor.PlChar.isGrounded)
            {
                if (_moveDirection.y>0&&_actor.PlCrouch.IsCeilDetectedForceCheck())
                    _moveDirection.y = 0;
                _moveDirection.y -= _actor.PlMoveSettings.Gravity * Time.deltaTime;
            }
            _actor.PlChar.Move(_moveDirection * Time.deltaTime);
        }
        
        private void TryToCrouch()
        {
            if (_isCrouch) return;
            _actor.PlCrouch.Enable();
            _isCrouch = true;
            _actor.PlChar.height = 0.85f;
            _playerController.Camera.transform.localPosition = Vector3.up * 0.75f;
        }
        
        private void TryToUnCrouch()
        {
            if (!_isCrouch) return;
            if (_actor.PlCrouch.UnCrouchIsAvaliliable)
                UnCrouch();
        }

        private void UnCrouch()
        {
            _actor.PlCrouch.Disable();
            _isCrouch = false;
            _actor.PlChar.height = 1.7f;
            _playerController.Camera.transform.localPosition = Vector3.up * 1.5f;
        }
    }
}
