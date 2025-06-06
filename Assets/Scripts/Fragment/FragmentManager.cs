using System;
using System.Collections.Generic;
using UnityEngine;

public class FragmentManager : MonoBehaviour
{
    public static FragmentManager Instance { get; private set; }

    //Lista de fragmentos ya recogidos (para evitar recoger dos veces el mismo)
    private HashSet<string> collectedFragmentIDs = new HashSet<string>();

    public int FragmentsCollected => collectedFragmentIDs.Count;

    // Notifica a los suscriptos que cambio el conteo
    public event Action<int> OnFragmentCountChanged;

    // Evento específico cuando se alcanza X fragmentos
    public event Action OnAllFragmentsCollected;

    // Total que se necesitan para completar el hito
    [SerializeField] private int totalFragmentsNeeded = 5;
    [SerializeField] private GameObject fragmentsToActivateAfterFirstPickup;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Se lo llama cuando el player colisione o agarre un fragmento
    // fragmentID es el id de ese fragmento
    public void CollectFragment(string fragmentID)
    {
        if (collectedFragmentIDs.Contains(fragmentID))
            return;

        collectedFragmentIDs.Add(fragmentID);

        Debug.Log("Fragment recogido: " + fragmentID);
        Debug.Log("Fragmentos recogidos: " + FragmentsCollected);

        // El primer fragmento es el botiquin, una vez obtenido se activan los demas
        if (FragmentsCollected == 1 && fragmentsToActivateAfterFirstPickup != null)
        {
            foreach (Transform child in fragmentsToActivateAfterFirstPickup.transform)
            {
                child.gameObject.SetActive(true);
            }

            Debug.Log("Primer fragmento recogido. Fragmentos restantes activados.");
        }

        OnFragmentCountChanged?.Invoke(FragmentsCollected);

        if (FragmentsCollected >= totalFragmentsNeeded)
        {
            Debug.Log("¡Todos los fragmentos fueron recogidos!");
            OnAllFragmentsCollected?.Invoke();
        }
    }


    // Podriamos usarlo cuando muere el player reiniciar los gragmentos, habria que expandir bien para que sepa cuantos tenia antes de empezar esa parte.
    public void ResetFragments()
    {
        collectedFragmentIDs.Clear();
        OnFragmentCountChanged?.Invoke(FragmentsCollected);
    }

    private void OnDestroy()
    {
        OnFragmentCountChanged = null;
        OnAllFragmentsCollected = null;
    }
}
