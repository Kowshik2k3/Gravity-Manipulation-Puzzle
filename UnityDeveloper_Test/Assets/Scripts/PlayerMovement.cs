using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 6f;

    [Header("Gravity")]
    public float gravity = -20f;
    public float groundCheckDistance = 0.3f;

    [Header("Rotation")]
    public float rotationSpeed = 12f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleGravity();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Camera-relative directions
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camForward * z + camRight * x).normalized;

        // Move character
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Smooth rotation towards movement direction
        if (move.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Jump (works even in idle)
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpForce;
        }
    }

    void HandleGravity()
    {
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // keeps controller grounded
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance);
    }

    void UpdateAnimator()
    {
        float speed = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).magnitude;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }
}
