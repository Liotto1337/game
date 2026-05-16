using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera _cinCam;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 75f;
    [SerializeField, Range(0f, 10f)] private float sensitivityX = 3f;
    [SerializeField, Range(0f, 10f)] private float sensitivityY = 3f;
    [SerializeField, Range(0f, 10f)] private float zoomSpeed = 5f;

    private float verticalVelocity;
    private float currentSpeed;
    private Vector2 _move;
    private CinemachineInputAxisController _axisController;
    
    public void Start()
    {
        currentSpeed = walkSpeed;

        _axisController = _cinCam.GetComponent<CinemachineInputAxisController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
    }

    public void OnJump()
    {
        if (_characterController.isGrounded) verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
    public void OnSprint(InputValue value)
    {
        if(value.Get<float>() > 0.5f)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    private void Update()
    {
        if (_characterController.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;
        Vector3 gravityMove = new Vector3(0, verticalVelocity, 0);
        _characterController.Move(((GetForward() * _move.y + GetRight() * _move.x) * currentSpeed + gravityMove) * Time.deltaTime);
        float targetFOV = currentSpeed == sprintSpeed ? sprintFOV : normalFOV;
        _cinCam.Lens.FieldOfView = Mathf.Lerp(_cinCam.Lens.FieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        UpdateSensitivity(sensitivityX, sensitivityY);
    }
    public void UpdateSensitivity(float newSensitivityX, float newSensitivityY)
    {
        if (_axisController == null) return;

        for (int i = 0; i < _axisController.Controllers.Count; i++)
        {
            var controllerInstance = _axisController.Controllers[i];
            
            if (controllerInstance.Name == "Look X (Pan)") controllerInstance.Input.Gain = newSensitivityX;

            else if (controllerInstance.Name == "Look Y (Tilt)") controllerInstance.Input.Gain = newSensitivityY * -1f;

            _axisController.Controllers[i] = controllerInstance;
        }
    }
    private Vector3 GetForward()
    {
        Vector3 forward = _cinCam.transform.forward;
        forward.y = 0;

        return forward.normalized;
    }
    private Vector3 GetRight()
    {
        Vector3 right = _cinCam.transform.right;
        right.y = 0;

        return right.normalized;
    }
}
