using UnityEngine;

public class BuildingHealth : MonoBehaviour, IDamageable
{
    [Header("Base Stats")]
    public int maxHealth = 500;
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
         Debug.Log($"Ana Üs Saldýrý Altýnda! Kalan Can: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            DestroyBase();
        }
    }

    private void DestroyBase()
    {
        Debug.Log("OYUN BÝTTÝ! (GAME OVER)");
        // Buraya daha sonra "Game Over" paneli açma kodu gelecek

        // Þimdilik binayý yok edelim ki test edebilelim
        Destroy(gameObject);
    }
}