using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Collider))]
public class PuzzleZoneSwitcher : MonoBehaviour
{
    [Tooltip("La Virtual Camera que enfoca este puzzle")]
    public CinemachineVirtualCamera puzzleCam;
    [Tooltip("Prioridad alta mientras el jugador está en zona")]
    public int activePriority = 20;
    private int originalPriority;

    void Awake()
    {
        
        if (puzzleCam == null)
            Debug.LogError("PuzzleZoneSwitcher necesita asignar puzzleCam.");
        else
            originalPriority = puzzleCam.Priority;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            puzzleCam.Priority = activePriority;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            puzzleCam.Priority = originalPriority;
    }
}