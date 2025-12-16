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

    // REMOVED: public Transform cameraTransform; // We'll get this automatically

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform cameraTransform;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Find camera automatically
        FindCamera();
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleGravity();
        UpdateAnimator();
    }

    void FindCamera()
    {
        // Look for SimpleFollowCamera or Main Camera
        SimpleFollowCamera sfc = FindObjectOfType<SimpleFollowCamera>();
        if (sfc != null)
        {
            cameraTransform = sfc.transform;
            Debug.Log("Found SimpleFollowCamera: " + sfc.gameObject.name);
        }
        else
        {
            // Fallback to main camera
            cameraTransform = Camera.main?.transform;
            if (cameraTransform != null)
            {
                Debug.Log("Using Main Camera: " + cameraTransform.gameObject.name);
            }
            else
            {
                Debug.LogError("No camera found! Please add a SimpleFollowCamera or Main Camera to the scene.");
            }
        }
    }

    void HandleMovement()
    {
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        // Get camera forward and right vectors
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // Flatten camera vectors (remove Y component)
        camForward.y = 0f;
        camRight.y = 0f;

        // Normalize to prevent faster diagonal movement
        camForward.Normalize();
        camRight.Normalize();

        // Calculate movement direction relative to camera
        Vector3 move = (camForward * z + camRight * x).normalized;

        // Move the character
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Rotate player to face movement direction
        if (move.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Jumping
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
        float speed = 0f;

        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D))
        {
            speed = 1f;
        }

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }
}   