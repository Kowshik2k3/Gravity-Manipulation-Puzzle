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
/*
using UnityEngine;

public class GravityController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform hologram;
    public Transform cameraTransform;

    [Header("Hologram Settings")]
    public float hologramDistance = 2f;
    public float hologramHeightOffset = 1.2f;
    public float hologramVisibleTime = 5f;

    Vector3 selectedDirection;
    bool hasSelection;
    float hologramTimer;

    void Update()
    {
        ReadArrowInput();
        UpdateHologramTimer();
        UpdateHologramVisual();

        if (hasSelection && Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmGravitySelection();
        }
    }

    // -------------------------
    // INPUT (ARROWS ONLY)
    // -------------------------
    void ReadArrowInput()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            dir = -cameraTransform.right;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            dir = cameraTransform.right;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            dir = cameraTransform.forward;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            dir = -cameraTransform.forward;

        if (dir == Vector3.zero) return;

        dir.y = 0f;
        selectedDirection = dir.normalized;

        hasSelection = true;
        hologramTimer = hologramVisibleTime;
    }

    // -------------------------
    // HOLOGRAM TIMER
    // -------------------------
    void UpdateHologramTimer()
    {
        if (!hasSelection) return;

        hologramTimer -= Time.deltaTime;

        if (hologramTimer <= 0f)
        {
            hasSelection = false;
            if (hologram) hologram.gameObject.SetActive(false);
        }
    }

    // -------------------------
    // HOLOGRAM VISUAL
    // -------------------------
    void UpdateHologramVisual()
    {
        if (!hasSelection || !hologram)
        {
            if (hologram) hologram.gameObject.SetActive(false);
            return;
        }

        hologram.gameObject.SetActive(true);

        hologram.position =
            player.position +
            selectedDirection * hologramDistance +
            Vector3.up * hologramHeightOffset;

        hologram.rotation =
            Quaternion.LookRotation(-selectedDirection, Vector3.up) *
            Quaternion.Euler(90f, 0f, 0f);
    }

    // -------------------------
    // CONFIRM (ENTER)
    // -------------------------
    void ConfirmGravitySelection()
    {
        hasSelection = false;
        hologramTimer = 0f;

        if (hologram)
            hologram.gameObject.SetActive(false);

        // 🔑 World rotation will be called here in next step
    }
}

*/

using UnityEngine;
using System.Collections;

public class GravityController : MonoBehaviour
{
    [Header("References")]
    public Transform worldRoot;
    public Transform player;
    public Transform hologram;
    public Transform cameraTransform;

    [Header("Hologram Settings")]
    public float hologramDistance = 2f;
    public float hologramHeightOffset = 1.2f;
    public float hologramVisibleTime = 5f;

    [Header("World Rotation")]
    public float rotationDuration = 0.5f; // tweak in Inspector

    Vector3 selectedDirection;
    Vector3 rotationAxis;
    float rotationAngle;

    bool hasSelection;
    bool isRotating;
    float hologramTimer;

    void Update()
    {
        if (isRotating) return;

        ReadArrowInput();
        UpdateHologramTimer();
        UpdateHologramVisual();

        if (hasSelection && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(RotateWorld());
        }
    }

    // -------------------------
    // ARROW INPUT
    // -------------------------
    void ReadArrowInput()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir = -cameraTransform.right;
            rotationAxis = Vector3.forward;
            rotationAngle = 90f;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir = cameraTransform.right;
            rotationAxis = Vector3.forward;
            rotationAngle = -90f;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            dir = cameraTransform.forward;
            rotationAxis = Vector3.right;
            rotationAngle = 90f;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            dir = -cameraTransform.forward;
            rotationAxis = Vector3.right;
            rotationAngle = -90f;
        }

        if (dir == Vector3.zero) return;

        dir.y = 0f;
        selectedDirection = dir.normalized;

        hasSelection = true;
        hologramTimer = hologramVisibleTime;
    }

    // -------------------------
    // HOLOGRAM TIMER
    // -------------------------
    void UpdateHologramTimer()
    {
        if (!hasSelection) return;

        hologramTimer -= Time.deltaTime;
        if (hologramTimer <= 0f)
        {
            hasSelection = false;
            if (hologram) hologram.gameObject.SetActive(false);
        }
    }

    // -------------------------
    // HOLOGRAM VISUAL
    // -------------------------
    void UpdateHologramVisual()
    {
        if (!hasSelection || !hologram)
        {
            if (hologram) hologram.gameObject.SetActive(false);
            return;
        }

        hologram.gameObject.SetActive(true);

        hologram.position =
            player.position +
            selectedDirection * hologramDistance +
            Vector3.up * hologramHeightOffset;

        hologram.rotation =
            Quaternion.LookRotation(-selectedDirection, Vector3.up) *
            Quaternion.Euler(90f, 0f, 0f);
    }

    // -------------------------
    // WORLD ROTATION
    // -------------------------
    IEnumerator RotateWorld()
    {
        isRotating = true;
        hasSelection = false;

        if (hologram)
            hologram.gameObject.SetActive(false);

        float elapsed = 0f;
        float rotated = 0f;

        while (elapsed < rotationDuration)
        {
            float step = (rotationAngle / rotationDuration) * Time.deltaTime;
            worldRoot.RotateAround(player.position, rotationAxis, step);

            rotated += step;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap exact remaining angle
        worldRoot.RotateAround(
            player.position,
            rotationAxis,
            rotationAngle - rotated
        );

        isRotating = false;
    }
}
