using UnityEngine;

public class FixedCameraWithCollision : MonoBehaviour
{
    public Transform target;

    [Header("Camera Follow")]
    public Vector3 offset = new Vector3(0f, 4f, -8f);
    public float smoothSpeed = 10f;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;

    Quaternion desiredRotation;
    Vector3 desiredOffset;

    void Start()
    {
        desiredRotation = transform.rotation;
        desiredOffset = offset;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position
            + target.right * desiredOffset.x
            + target.up * desiredOffset.y
            + target.forward * desiredOffset.z;

        Vector3 direction = desiredPosition - target.position;
        float distance = direction.magnitude;
        direction.Normalize();

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

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            smoothSpeed * Time.deltaTime
        );
    }

    // 🔒 ONLY CALLED ON GRAVITY CHANGE
    public void SetCameraRotation(Vector3 gravityDir)
    {
        // Reset offset every time (prevents drift)
        desiredOffset = offset;

        if (gravityDir == Vector3.left)
            desiredRotation = Quaternion.Euler(90f, 0f, -90f);

        else if (gravityDir == Vector3.right)
            desiredRotation = Quaternion.Euler(90f, 0f, 90f);

        else if (gravityDir == Vector3.forward)
            desiredRotation = Quaternion.Euler(-90f, 0f, 0f);

        else if (gravityDir == Vector3.back)
            desiredRotation = Quaternion.Euler(90f, 0f, 90f);

        else
            desiredRotation = Quaternion.identity;
    }
}
