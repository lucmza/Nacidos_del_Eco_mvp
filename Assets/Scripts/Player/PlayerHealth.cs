using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }
    
    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        Debug.Log("Player took damage: " + amount + " | Health: " + _currentHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }
}
