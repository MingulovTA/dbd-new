using App;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    private float lookSpeed = 1.5f;
    private float lookXLimit = 89.9f;
    private float _rotationX = 0;

    private Transform _cameraTransform;
    private Transform _playerTransform;
    private void Awake()
    {
        _cameraTransform = playerController.Camera.transform;
        _playerTransform = playerController.transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float dLookX, dLookY;

        dLookX = Input.GetAxis("Mouse X");
        dLookY = Input.GetAxis("Mouse Y");
        _rotationX += -dLookY * lookSpeed;
        _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
        _cameraTransform.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        _playerTransform.rotation *= Quaternion.Euler(0, dLookX * lookSpeed, 0);
    }
}
