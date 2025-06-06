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
    [SerializeField] private GameObject _visionConeGO;    // Hijo con VisionConeDrawer para modo stealth
    [SerializeField] private GameObject _visionCircleGO;  // Hijo con VisionConeDrawer para modo no-stealth

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

        // Actualizamos los visuales SOLO si _playerMovement no es null
        if (_playerMovement != null)
            UpdateVisuals();
    }

    private void Update()
    {
        // Si no hemos encontrado al jugador, volvemos a intentar una vez por frame
        if (_playerMovement == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                _playerMovement = playerObj.GetComponent<PlayerMovement>();
            // Salimos para no ejecutar detección/ví­sual hasta que PlayerMovement esté disponible
            return;
        }

        // Actualizamos continuamente los visuales según OnStealth
        UpdateVisuals();

        // Ejecutamos la detección de jugador (tal como antes)
        PerformDetection();
    }

    private void UpdateVisuals()
    {
        // Si por alguna razón _playerMovement es null, salimos
        if (_playerMovement == null) return;

        // Modo stealth → solo activamos el cono
        if (_playerMovement.OnStealth)
        {
            if (_visionConeGO  != null) _visionConeGO.SetActive(true);
            if (_visionCircleGO != null) _visionCircleGO.SetActive(false);
        }
        // Modo no-stealth → solo activamos el círculo completo
        else
        {
            if (_visionConeGO  != null) _visionConeGO.SetActive(false);
            if (_visionCircleGO != null) _visionCircleGO.SetActive(true);
        }
    }

    private void PerformDetection()
    {
        // Usamos OverlapSphere para chequear presencia dentro del rango "loud"
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
