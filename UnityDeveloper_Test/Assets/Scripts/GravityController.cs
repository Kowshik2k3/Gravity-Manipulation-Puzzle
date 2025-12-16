using UnityEngine;

public class GravityController : MonoBehaviour
{
    [Header("Hologram")]
    public Transform hologram;
    public float hologramDistance = 2f;
    public float hologramHeightOffset = 1.2f;

    [Header("Camera (Optional)")]
    public Transform cameraTransform; // This will be auto-assigned

    private Vector3 selectedDirection;
    private bool hasSelection = false;

    void Start()
    {
        // Try to find camera if not assigned
        if (cameraTransform == null)
        {
            FindCamera();
        }
    }

    void FindCamera()
    {
        // Look for SimpleFollowCamera
        SimpleFollowCamera sfc = FindObjectOfType<SimpleFollowCamera>();
        if (sfc != null)
        {
            cameraTransform = sfc.transform;
            Debug.Log("GravityController: Found camera: " + sfc.gameObject.name);
        }
        else
        {
            // Fallback to main camera
            cameraTransform = Camera.main?.transform;
            if (cameraTransform != null)
            {
                Debug.Log("GravityController: Using Main Camera: " + cameraTransform.gameObject.name);
            }
            else
            {
                Debug.LogWarning("GravityController: No camera found. Arrow keys will use world directions.");
            }
        }
    }

    void Update()
    {
        ReadDirectionInput();
        UpdateHologram();
    }

    void ReadDirectionInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedDirection = -GetCameraRight();
            hasSelection = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedDirection = GetCameraRight();
            hasSelection = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedDirection = GetCameraForward();
            hasSelection = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedDirection = -GetCameraForward();
            hasSelection = true;
        }
    }

    Vector3 GetCameraForward()
    {
        if (cameraTransform == null)
        {
            return Vector3.forward; // Default world forward
        }

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    Vector3 GetCameraRight()
    {
        if (cameraTransform == null)
        {
            return Vector3.right; // Default world right
        }

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        return right.normalized;
    }

    void UpdateHologram()
    {
        if (!hologram || !hasSelection)
        {
            if (hologram) hologram.gameObject.SetActive(false);
            return;
        }

        hologram.gameObject.SetActive(true);

        // Position hologram on the target wall
        hologram.position = transform.position + selectedDirection * hologramDistance + Vector3.up * hologramHeightOffset;

        // Rotate hologram so feet face the wall, head toward player
        hologram.rotation = Quaternion.LookRotation(-selectedDirection, Vector3.up)
                      * Quaternion.Euler(90f, 0f, 0f);
    }

    public Vector3 GetSelectedGravityDirection()
    {
        return selectedDirection;
    }
}