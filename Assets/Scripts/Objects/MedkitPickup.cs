using UnityEngine;

public class MedkitPickup : MonoBehaviour, ICollectible
{
    [SerializeField] private string fragmentID = "medkit_01";

    private bool collected = false;

    public void Collect(GameObject collector)
    {
        if (collected) return;
        collected = true;

        FragmentManager.Instance.CollectFragment(fragmentID);

        Destroy(gameObject);
    }
}
