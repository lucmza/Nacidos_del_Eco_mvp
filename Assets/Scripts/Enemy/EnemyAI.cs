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
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _idealDistance = 1.2f;
    [SerializeField] private float _orbitSpeed = 1f;
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
                MoveTowardsTarget(_currentTarget);
                break;

            case EnemyState.Chasing:
                if (_playerTransform != null)
                {
                    float dist = Vector3.Distance(transform.position, _playerTransform.position);

                    // Ataca si está en rango y no está haciendo wind-up
                    if (!_isWindingUp && dist <= _attackRange)
                    {
                        StartCoroutine(AttackRoutine());
                    }
                    else
                    {
                        MoveAroundPlayer();
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

    private void MoveAroundPlayer()
    {
        if (_playerTransform == null) return;

        Vector3 toPlayer = _playerTransform.position - transform.position;
        toPlayer.y = 0;

        float distance = toPlayer.magnitude;

        // Movimiento radial (acercarse o alejarse)
        float distanceDelta = distance - _idealDistance;
        Vector3 radialDir = toPlayer.normalized * distanceDelta;

        // Movimiento tangencial (orbitar)
        Vector3 tangentDir = Vector3.Cross(toPlayer.normalized, Vector3.up);

        // Combinar ambos
        Vector3 finalMove = (radialDir + tangentDir * _orbitSpeed).normalized;

        _rb.MovePosition(transform.position + finalMove * _speed * Time.fixedDeltaTime);
        LookAtTarget(_playerTransform.position);
    }


    private IEnumerator AttackRoutine()
    {
        _isWindingUp = true;
        _state = EnemyState.PreAttack;

        float timer = 0.75f;
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
    private void MoveTowardsTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;

        Vector3 move = dir.normalized * _speed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + move);

        Vector3 lookAt = new Vector3(target.x, transform.position.y, target.z);
        transform.LookAt(lookAt);
    }

    // Orbitar al jugador manteniendo la distancia ideal
    private void OrbitPlayer()
    {
        Vector3 toPlayer = _playerTransform.position - transform.position;
        float dist = toPlayer.magnitude;

        // Si está demasiado lejos, acercarse
        if (dist > _idealDistance + 0.1f)
        {
            Vector3 move = toPlayer.normalized * _speed * Time.fixedDeltaTime;
            _rb.MovePosition(transform.position + move);
        }

        // Agregamos leve movimiento orbital
        Vector3 orbitDirection = Vector3.Cross(toPlayer.normalized, Vector3.up); // giro horizontal
        Vector3 orbitMove = orbitDirection * _orbitSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + orbitMove);

        LookAtTarget(_playerTransform.position);
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
        Debug.Log("TRYATTACK CALLED");
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Collider[] hits = Physics.OverlapSphere(origin, _attackRange);
        Debug.Log($"Checking for targets in range: {hits.Length} colliders found.");

        foreach (var h in hits)
        {
            Debug.Log($"Detected collider: {h.name}");
            if (h.CompareTag("Player") && h.TryGetComponent<IDamageable>(out var d))
            {
                d.TakeDamage(20f);
                Debug.Log("Damage applied to player!");
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
