using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform _viewCameraTransform;
    [SerializeField] private Transform _transform;
    
    private float lookSpeed = 1.5f;
    private float lookXLimit = 89.9f;
    private float _rotationX = 0;

    private void OnValudate()
    {
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        float dLookX, dLookY;

        dLookX = Input.GetAxis("Mouse X");
        dLookY = Input.GetAxis("Mouse Y");
        _rotationX += -dLookY * lookSpeed;
        _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
        _viewCameraTransform.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        _transform.rotation *= Quaternion.Euler(0, dLookX * lookSpeed, 0);
    }
}
