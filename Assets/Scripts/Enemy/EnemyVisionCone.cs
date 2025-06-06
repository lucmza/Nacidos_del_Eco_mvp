using UnityEngine;

public class EnemyVisionCone : MonoBehaviour
{
    [SerializeField] private float _visionRange = 10f;
    [SerializeField] private float _visionAngle = 45f;
    [SerializeField] private float _detectionRangeWhenLoud = 15f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private EnemyAI _enemyAI;

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRangeWhenLoud, _playerLayer);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            // Verificar si esta en sigilo
            PlayerMovement pm = hit.GetComponent<PlayerMovement>();
            if (pm == null) continue;

            Vector3 directionToPlayer = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            bool isInCone = angle < _visionAngle && Vector3.Distance(transform.position, hit.transform.position) <= _visionRange;
            bool isMakingNoise = !pm.OnStealth;

            if (isInCone || isMakingNoise)
            {
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, directionToPlayer, out RaycastHit hitInfo, _detectionRangeWhenLoud))
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
