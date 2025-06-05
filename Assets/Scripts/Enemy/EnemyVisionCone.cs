using UnityEngine;

public class EnemyVisionCone : MonoBehaviour
{
    [SerializeField] private float _visionRange = 10f;
    [SerializeField] private float _visionAngle = 45f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private EnemyAI _enemyAI;

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _visionRange, _playerLayer);

        foreach (var hit in hits)
        {
            Vector3 directionToPlayer = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle < _visionAngle)
            {
                Ray ray = new Ray(transform.position, directionToPlayer);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, _visionRange))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        _enemyAI.SetTarget(hitInfo.transform);
                    }
                }
            }
        }
    }
}
