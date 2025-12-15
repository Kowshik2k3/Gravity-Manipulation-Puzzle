using UnityEngine;

public class FixedCameraWithCollision : MonoBehaviour
{
    public Transform target;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0f, 4f, -8f);
    public float smoothSpeed = 10f;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;

    [Header("Fixed Rotation")]
    public Vector3 fixedRotation = new Vector3(-0.5f, -1f, 0f);

    void LateUpdate()
    {
        if (!target) return;

        // Desired camera position
        Vector3 desiredPosition = target.position + offset;

        // Direction from target to camera
        Vector3 direction = desiredPosition - target.position;
        float distance = direction.magnitude;
        direction.Normalize();

        // Check if wall is blocking view
        if (Physics.SphereCast(
            target.position,
            collisionRadius,
            direction,
            out RaycastHit hit,
            distance,
            collisionLayers))
        {
            desiredPosition = hit.point - direction * collisionRadius;
        }

        // Smooth camera movement
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Fixed camera angle
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}
