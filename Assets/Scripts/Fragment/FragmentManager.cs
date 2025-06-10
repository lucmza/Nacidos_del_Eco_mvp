using System;
using System.Collections.Generic;
using UnityEngine;

public class FragmentManager : MonoBehaviour
{
    public static FragmentManager Instance { get; private set; }


    private HashSet<string> collectedFragmentIDs = new HashSet<string>();

    public int FragmentsCollected => collectedFragmentIDs.Count;

 
    public event Action<int> OnFragmentCountChanged;

   
    public event Action OnAllFragmentsCollected;


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

    public void CollectFragment(string fragmentID)
    {
        if (collectedFragmentIDs.Contains(fragmentID))
            return;

        collectedFragmentIDs.Add(fragmentID);

        Debug.Log("Fragment recogido: " + fragmentID);
        Debug.Log("Fragmentos recogidos: " + FragmentsCollected);

      
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
