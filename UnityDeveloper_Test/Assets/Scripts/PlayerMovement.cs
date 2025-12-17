/*
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
*/
/*
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 6f;

    [Header("Gravity")]
    public float gravityStrength = 30f;
    public Vector3 gravityDirection = Vector3.down;

    [Header("Rotation")]
    public float rotationSpeed = 12f;

    CharacterController controller;
    Animator animator;

    Vector3 velocity;
    bool physicsGrounded;
    bool animGrounded;
    bool isFalling;
    bool hasMoveInput;
    bool jumpRequested;
    bool wasGroundedLastFrame;

    float groundedGraceTime = 0.15f;
    float groundedGraceTimer;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleGrounding();
        UpdateCapsuleForGravity();
        HandleMovement();
        HandleGravity();
        UpdateAnimator();
    }


    void HandleGrounding()
    {
        physicsGrounded = controller.isGrounded;

        if (physicsGrounded)
        {
            groundedGraceTimer = groundedGraceTime;

            controller.Move(gravityDirection * 0.05f);

            // ✅ LANDING DETECTED (first grounded frame)
            if (!wasGroundedLastFrame)
            {
                isFalling = false;
                velocity = Vector3.zero;
                jumpRequested = false;
            }
        }
        else
        {
            groundedGraceTimer -= Time.deltaTime;
            StartFalling();
        }

        animGrounded = groundedGraceTimer > 0f;
        wasGroundedLastFrame = physicsGrounded;
    }



    void HandleMovement()
    {
        float x = 0f;
        float z = 0f;

        // ✅ ONLY WASD — NO ARROW KEYS
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        Vector3 up = -gravityDirection;
        Vector3 right = Vector3.Cross(up, Vector3.forward).normalized;
        Vector3 forward = Vector3.Cross(right, up).normalized;

        Vector3 move = (right * x + forward * z);
        hasMoveInput = move.sqrMagnitude > 0.01f;

        if (hasMoveInput)
        {
            move.Normalize();
            controller.Move(move * moveSpeed * Time.deltaTime);
        }


        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move, up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        if (animGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity = -gravityDirection * jumpForce;
            groundedGraceTimer = 0f;
            jumpRequested = true;
            StartFalling();
        }


    }
    void StartFalling()
    {
        if (!isFalling)
        {
            isFalling = true;
        }
    }

    void HandleGravity()
    {
        if (!physicsGrounded)
        {

            velocity += gravityDirection * gravityStrength * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

    }



    // 🔑 CALLED ONLY BY GravityController
    public void ApplyGravity(Vector3 newGravity)
    {
        gravityDirection = newGravity.normalized;

        Vector3 up = -gravityDirection;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up);

        if (forward.sqrMagnitude < 0.01f)
            forward = Vector3.ProjectOnPlane(Vector3.forward, up);

        transform.rotation = Quaternion.LookRotation(forward, up);

        // 🔑 HARD RESET PHYSICS STATE
        velocity = Vector3.zero;

        // 🔑 FORCE LANDING GRACE
        groundedGraceTimer = groundedGraceTime;
        UpdateCapsuleForGravity();

    }
    void UpdateCapsuleForGravity()
    {
        // gravityDirection is normalized
        Vector3 up = -gravityDirection;

        // Capsule stays vertical, so we fake alignment by shifting center
        float halfHeight = controller.height * 0.5f;

        // Project gravity onto world Y
        float verticalDot = Vector3.Dot(up, Vector3.up);

        // When on ground → normal center
        if (Mathf.Abs(verticalDot) > 0.9f)
        {
            controller.center = new Vector3(0f, halfHeight, 0f);
        }
        else
        {
            // On wall → push capsule outward from wall
            Vector3 horizontal = new Vector3(up.x, 0f, up.z).normalized;
            controller.center = horizontal * (controller.radius * 0.9f) + Vector3.up * halfHeight;
        }
    }


    void UpdateAnimator()
    {
        animator.SetFloat("Speed", hasMoveInput ? 1f : 0f);
        animator.SetBool("IsGrounded", animGrounded);
        animator.SetBool("IsFalling", isFalling);
    }


}

*/
/*
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float gravity = -25f;
    public float rotationSpeed = 12f;

    [Header("References")]
    public Transform cameraTransform;

    CharacterController controller;
    Animator animator;

    Vector3 velocity;
    bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleGravity();
        UpdateAnimator();
    }

    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // keep grounded
        }
    }

    void HandleMovement()
    {
        float x = 0f;
        float z = 0f;

        // ✅ ONLY WASD — NO ARROW KEYS
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camForward * z + camRight * x).normalized;

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // Jump
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpForce;
        }
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        animator.SetFloat(
            "Speed",
            controller.velocity.magnitude > 0.1f ? 1f : 0f
        );

        animator.SetBool("IsGrounded", isGrounded);
    }
}
*/

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float gravity = -25f;
    public float rotationSpeed = 12f;

    [Header("References")]
    public Transform cameraTransform;

    CharacterController controller;
    Animator animator;

    Vector3 velocity;
    bool isGrounded;

    // 🔑 NEW: drive animation from input, not controller.velocity
    float currentMoveAmount;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.IsGameRunning)
            return;

        HandleGroundCheck();
        HandleMovement();
        HandleGravity();
        UpdateAnimator();
    }


    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;
    }

    void HandleMovement()
    {
        float x = 0f;
        float z = 0f;

        // ✅ ONLY WASD
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camForward * z + camRight * x);

        // 🔑 THIS FIXES RUN ANIMATION
        currentMoveAmount = move.magnitude;

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpForce;
        }
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        animator.SetFloat("Speed", currentMoveAmount);
        animator.SetBool("IsGrounded", isGrounded);
    }
}
