using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Notify GameManager later (for now just destroy)
        GameManager.Instance?.OnCollectablePicked();

        Destroy(gameObject);
    }
}
