using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Inject] private IPlayerInput playerInput;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private float verticalRotation = 0f;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        ApplyGravity();
    }

    private void HandleMovement()
    {

        Vector3 move = transform.right * playerInput.MoveDitection().x 
            + transform.forward * playerInput.MoveDitection().z;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void HandleCamera()
    {
        float mouseX = playerInput.CameraDitection().x * mouseSensitivity;
        float mouseY = playerInput.CameraDitection().y * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ApplyGravity()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, controller.height / 2f, 0), groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
