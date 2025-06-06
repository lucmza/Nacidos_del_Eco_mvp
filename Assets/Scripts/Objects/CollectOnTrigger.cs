using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.Collect(other.gameObject);
        }
    }
}
