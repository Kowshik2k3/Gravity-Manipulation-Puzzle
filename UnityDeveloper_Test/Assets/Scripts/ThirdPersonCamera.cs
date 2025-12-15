using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 3f, -6f);

    [Header("Smoothing")]
    public float smoothSpeed = 10f;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;

    void LateUpdate()
    {
        if (!target) return;

        // Desired position based on player rotation
        Vector3 desiredPosition = target.position + target.rotation * offset;

        Vector3 direction = desiredPosition - target.position;
        float distance = direction.magnitude;
        direction.Normalize();

        // Check for wall between player and camera
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

        // Smooth movement
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Always look at player
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
