using UnityEngine;

public class FragmentPickup : MonoBehaviour, ICollectible
{
    [SerializeField] private string fragmentID;

    private bool collected = false;

    public void Collect(GameObject collector)
    {
        if (collected) return;

        collected = true;
        FragmentManager.Instance.CollectFragment(fragmentID);

        Destroy(gameObject);
    }
}
