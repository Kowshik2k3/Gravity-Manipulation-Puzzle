using UnityEngine;

public class GravityController : MonoBehaviour
{
    public Transform hologram;
    public Transform cameraTransform;
    public float hologramDistance = 2f;

    [Header("Hologram")]
    public float hologramHeightOffset = 1.2f;

    private Vector3 selectedDirection;
    private bool hasSelection = false;

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
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    Vector3 GetCameraRight()
    {
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
