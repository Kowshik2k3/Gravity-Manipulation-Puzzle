using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;                 // The player to follow
    public Vector3 offset = new Vector3(0f, 4f, -8f); // Camera position relative to player
    public float followSpeed = 10f;          // How quickly camera follows
    public float rotationSpeed = 0f;         // Set to 0 for no rotation

    [Header("Collision Settings")]
    public float collisionRadius = 0.3f;     // Size of sphere for collision check
    public LayerMask collisionLayers;        // Which layers can block camera
    public float minDistance = 2f;           // Minimum distance camera can get to player

    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    private Vector3 originalOffset;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("SimpleFollowCamera: No target assigned! Please assign the player.");
            return;
        }

        originalOffset = offset;
        desiredRotation = transform.rotation; // Keep the starting rotation
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        desiredPosition = target.position + offset;

        // Handle camera collision
        HandleCameraCollision();

        // Smoothly move to desired position
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // Keep rotation fixed (no rotation)
        transform.rotation = desiredRotation;
    }

    void HandleCameraCollision()
    {
        // Direction from target to camera
        Vector3 direction = desiredPosition - target.position;
        float distance = direction.magnitude;

        if (distance > 0.1f)
        {
            direction.Normalize();

            // Check for collision between camera and player
            if (Physics.SphereCast(
                target.position,
                collisionRadius,
                direction,
                out RaycastHit hit,
                distance,
                collisionLayers))
            {
                // If hit, adjust the offset to avoid the wall
                float hitDistance = hit.distance - collisionRadius;
                Vector3 adjustedOffset = direction * Mathf.Max(hitDistance, minDistance);
                offset = adjustedOffset;
            }
            else
            {
                // No collision, use original offset
                offset = Vector3.Lerp(offset, originalOffset, followSpeed * Time.deltaTime);
            }
        }
    }

    // Public method to update camera references in other scripts
    public Transform GetCameraTransform()
    {
        return transform;
    }
}