using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FragmentCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    // Tiene que ser la misma cantidad que en el manager, podriamos obtenerlo de ahi pero por ahora lo dejo asi
    [SerializeField] private int totalNecesario = 5;

    private void OnEnable()
    {
        FragmentManager.Instance.OnFragmentCountChanged += UpdateCounter;
        UpdateCounter(FragmentManager.Instance.FragmentsCollected);
    }

    private void OnDisable()
    {
        if (FragmentManager.Instance != null)
            FragmentManager.Instance.OnFragmentCountChanged -= UpdateCounter;
    }

    private void UpdateCounter(int nuevoConteo)
    {
        counterText.text = $"Fragmentos: {nuevoConteo} / {totalNecesario}";
    }
}
