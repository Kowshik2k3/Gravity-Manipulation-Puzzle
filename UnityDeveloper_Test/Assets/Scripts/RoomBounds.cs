using UnityEngine;

public class RoomBounds : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameManager.Instance.OnPlayerFellIntoVoid();


    }
}
