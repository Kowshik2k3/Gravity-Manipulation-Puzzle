/*
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public PlayerMovement playerMovement;
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

    void ApplyGravity()
    {
        if (!playerMovement) return;

        playerMovement.AlignToGravity(selectedDirection);

        hasSelection = false;
        if (hologram) hologram.gameObject.SetActive(false);
    }

}
*/


/*
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public PlayerMovement player;
    public Transform hologram;
    public Transform cameraTransform;

    public float hologramDistance = 2f;
    public float hologramHeightOffset = 1.2f;

    Vector3 selectedGravity;
    bool hasSelection;

    void Update()
    {
        ReadArrowInput();
        UpdateHologram();

        if (hasSelection && Input.GetKeyDown(KeyCode.Return))
        {
            ApplyGravity();
        }
    }

    void ReadArrowInput()
    {
        hasSelection = true;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            selectedGravity = -cameraTransform.right;

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            selectedGravity = cameraTransform.right;

        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedGravity = cameraTransform.forward;

        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selectedGravity = -cameraTransform.forward;

        else
            hasSelection = false;

        if (hasSelection)
        {
            selectedGravity.y = 0f;
            selectedGravity.Normalize();
        }
    }

    void UpdateHologram()
    {
        if (!hologram || !hasSelection)
        {
            if (hologram) hologram.gameObject.SetActive(false);
            return;
        }

        hologram.gameObject.SetActive(true);

        hologram.position =
            player.transform.position +
            selectedGravity * hologramDistance +
            Vector3.up * hologramHeightOffset;

        hologram.rotation =
            Quaternion.LookRotation(-selectedGravity, Vector3.up) *
            Quaternion.Euler(90f, 0f, 0f);
    }

    void ApplyGravity()
    {
        if (!player) return;

        player.ApplyGravity(selectedGravity);
        hasSelection = false;

        if (hologram)
            hologram.gameObject.SetActive(false);
    }
}
*/

using UnityEngine;

public class GravityController : MonoBehaviour
{
    public PlayerMovement player;
    public Transform hologram;
    public Transform cameraTransform;

    [Header("Hologram Settings")]
    public float hologramDistance = 2f;
    public float hologramHeightOffset = 1.2f;
    public float hologramVisibleTime = 5f; // ✅ 5 seconds

    private Vector3 selectedGravity;
    private bool hasSelection;

    private float hologramTimer;

    void Update()
    {
        ReadArrowInput();
        UpdateHologramTimer();
        UpdateHologramVisual();

        if (hasSelection && Input.GetKeyDown(KeyCode.Return))
        {
            ApplyGravity();
        }
    }

    // -------------------------
    // INPUT (ARROW KEYS ONLY)
    // -------------------------
    void ReadArrowInput()
    {
        Vector3 newSelection = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            newSelection = -cameraTransform.right;

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            newSelection = cameraTransform.right;

        else if (Input.GetKeyDown(KeyCode.UpArrow))
            newSelection = cameraTransform.forward;

        else if (Input.GetKeyDown(KeyCode.DownArrow))
            newSelection = -cameraTransform.forward;

        if (newSelection != Vector3.zero)
        {
            newSelection.y = 0f;
            selectedGravity = newSelection.normalized;

            hasSelection = true;
            hologramTimer = hologramVisibleTime; // ✅ reset timer
        }
    }

    // -------------------------
    // TIMER
    // -------------------------
    void UpdateHologramTimer()
    {
        if (!hasSelection) return;

        hologramTimer -= Time.deltaTime;

        if (hologramTimer <= 0f)
        {
            hasSelection = false;
            if (hologram)
                hologram.gameObject.SetActive(false);
        }
    }

    // -------------------------
    // HOLOGRAM VISUAL
    // -------------------------
    void UpdateHologramVisual()
    {
        if (!hologram || !hasSelection)
        {
            if (hologram)
                hologram.gameObject.SetActive(false);
            return;
        }

        hologram.gameObject.SetActive(true);

        hologram.position =
            player.transform.position +
            selectedGravity * hologramDistance +
            Vector3.up * hologramHeightOffset;

        hologram.rotation =
            Quaternion.LookRotation(-selectedGravity, Vector3.up) *
            Quaternion.Euler(90f, 0f, 0f);
    }

    // -------------------------
    // APPLY GRAVITY
    // -------------------------
    void ApplyGravity()
    {
        if (!player) return;

        //player.ApplyGravity(selectedGravity);

        hasSelection = false;
        hologramTimer = 0f;

        if (hologram)
            hologram.gameObject.SetActive(false);
    }
}
