using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int _currentHealth;

    public bool IsDead => _currentHealth <= 0;

    void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (IsDead) return;

        _currentHealth -= damageAmount;
         Debug.Log(gameObject.name + " hasar aldý! Kalan Can: " + _currentHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Burada ölüm efekti, ses vs. olur
        Destroy(gameObject);
    }
}