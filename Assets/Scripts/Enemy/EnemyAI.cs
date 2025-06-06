using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PatrolPoint
{
    public Transform point;
    public float waitTime;
}

public enum EnemyState
{
    Patrolling,
    Chasing,
    PreAttack,
    Attacking
}

public class EnemyAI : MonoBehaviour
{
    // Movimiento
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _chaseSpeed = 4f;
    [SerializeField] private List<PatrolPoint> _patrolPoints;
    private int _currentPointIndex = 0;
    private bool _isChasing = false;
    private Vector3 _currentTarget;

    // Ataque
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackCooldown = 2f;
    private bool _isWindingUp = false;

    // Vida
    [SerializeField] private float _maxHealth = 50f;
    private float _currentHealth;

    private Transform _playerTransform;
    private Rigidbody _rb;
    private EnemyState _state = EnemyState.Patrolling;



    private void Awake()
    {
        _currentHealth = _maxHealth;
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(PatrolRoutine());
    }

    private void FixedUpdate()
    {
        switch (_state)
        {
            case EnemyState.Patrolling:
    MoveTowardsTarget(_currentTarget, _patrolSpeed);
    break;

case EnemyState.Chasing:
    if (_playerTransform != null)
    {
        MoveTowardsTarget(_playerTransform.position, _chaseSpeed);

        if (!_isWindingUp && IsPlayerInAttackRange())
        {
            StartCoroutine(AttackRoutine());
        }
    }
    break;

        }
    }



    private IEnumerator PatrolRoutine()
    {
        while (!_isChasing)
        {
            PatrolPoint pp = _patrolPoints[_currentPointIndex];
            float sqrTol = 0.3f;

            _currentTarget = pp.point.position;
            while ((_currentTarget - transform.position).sqrMagnitude > sqrTol)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(pp.waitTime);
            _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Count;
        }
    }


    private IEnumerator AttackRoutine()
    {
        _isWindingUp = true;
        _state = EnemyState.PreAttack;

        float timer = 0.3f;
        while (timer > 0f)
        {
            if (!IsPlayerInAttackRange())
            {
                _state = EnemyState.Chasing;
                _isWindingUp = false;
                yield break;
            }

            LookAtTarget(_playerTransform.position);
            timer -= Time.deltaTime;
            yield return null;
        }

        _state = EnemyState.Attacking;
        Debug.Log("Enemy is now attacking!");
        TryAttack();

        timer = _attackCooldown;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        _state = EnemyState.Chasing;
        _isWindingUp = false;
    }

    // Movimiento hacia target
    private void MoveTowardsTarget(Vector3 target, float speed)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;

        Vector3 move = dir.normalized * speed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + move);

        Vector3 lookAt = new Vector3(target.x, transform.position.y, target.z);
        transform.LookAt(lookAt);
    }


    public void SetTarget(Transform player)
    {
        _playerTransform = player;
        _isChasing = true;
        _state = EnemyState.Chasing;
    }

    private void LookAtTarget(Vector3 target)
    {
        Vector3 la = new Vector3(target.x, transform.position.y, target.z);
        transform.LookAt(la);
    }

    private bool IsPlayerInAttackRange()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Collider[] hits = Physics.OverlapSphere(origin, _attackRange, LayerMask.GetMask("Player"));
        Debug.DrawLine(origin, origin + Vector3.up * 0.1f, Color.red, 1f);

        foreach (var h in hits)
            if (h.CompareTag("Player"))
                return true;
        return false;
    }

    private void TryAttack()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Collider[] hits = Physics.OverlapSphere(origin, _attackRange);

        foreach (var h in hits)
        {
            if (h.CompareTag("Player") && h.TryGetComponent<IDamageable>(out var d))
            {
                d.TakeDamage(999f);
                Debug.Log("Player destroyed!");
            }
        }
    }



    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        Debug.Log($"Enemy took damage: {amount} | Health: {_currentHealth}");
        if (_currentHealth <= 0f) Die();
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}


