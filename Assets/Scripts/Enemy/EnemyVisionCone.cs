using UnityEngine;

public class EnemyVisionCone : MonoBehaviour
{
    [Header("Configuración de detección")]
    [SerializeField] private float _visionRange = 10f;
    [SerializeField] private float _visionAngle = 45f;
    [SerializeField] private float _detectionRangeWhenLoud = 15f;

    [Header("Referencias")]
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private EnemyAI _enemyAI;

    [Header("Visuales")]
    [SerializeField] private GameObject _visionConeGO;
    [SerializeField] private GameObject _visionCircleGO;

    private PlayerMovement _playerMovement;

    private void Start()
    {
        // Buscamos en escena el objeto con tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _playerMovement = playerObj.GetComponent<PlayerMovement>();
            if (_playerMovement == null)
                Debug.LogWarning("El GameObject con tag 'Player' no tiene PlayerMovement.");
        }
        else
        {
            Debug.LogWarning("No se encontró ningún GameObject con tag 'Player'. " +
                             "Asegurate de que el jugador esté etiquetado correctamente.");
        }
        if (_playerMovement != null)
            UpdateVisuals();
    }

    private void Update()
    {
        if (_playerMovement == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                _playerMovement = playerObj.GetComponent<PlayerMovement>();
            return;
        }

        UpdateVisuals();

        PerformDetection();
    }

    private void UpdateVisuals()
    {
        if (_playerMovement == null) return;

        if (_playerMovement.OnStealth)
        {
            if (_visionConeGO  != null) _visionConeGO.SetActive(true);
            if (_visionCircleGO != null) _visionCircleGO.SetActive(false);
        }
        else
        {
            if (_visionConeGO  != null) _visionConeGO.SetActive(false);
            if (_visionCircleGO != null) _visionCircleGO.SetActive(true);
        }
    }

    private void PerformDetection()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRangeWhenLoud, _playerLayer);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            PlayerMovement pm = hit.GetComponent<PlayerMovement>();
            if (pm == null) continue;

            Vector3 dirToPlayer = (hit.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            bool isInCone = angleToPlayer < _visionAngle
                            && Vector3.Distance(transform.position, hit.transform.position) <= _visionRange;
            bool isMakingNoise = !pm.OnStealth;

            if (isInCone || isMakingNoise)
            {
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dirToPlayer,
                                    out RaycastHit hitInfo, _detectionRangeWhenLoud))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        _enemyAI.SetTarget(hitInfo.transform);
                        break;
                    }
                }
            }
        }
    }
}
