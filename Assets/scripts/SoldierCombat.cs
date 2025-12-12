using UnityEngine;

public class SoldierCombat : MonoBehaviour
{
    [Header("Combat Stats")]
    public int damage = 10;
    public float attackRate = 1.0f; // Saniyede kaç vuruþ?
    public float attackRange = 5f;  // Radar menzili
    public LayerMask enemyLayer;    // Sadece düþmanlarý tara (Performans için kritik!)

    [Header("Debug")]
    [SerializeField] private Transform currentTarget;
    private float _attackCooldown;
    private float _scanTimer;

    // Dýþarýdan okumak için property
    public bool HasTarget => currentTarget != null;
    public Transform CurrentTarget => currentTarget;

    void Update()
    {
        // 1. ZAMANLAYICILAR
        if (_attackCooldown > 0) _attackCooldown -= Time.deltaTime;

        _scanTimer -= Time.deltaTime;

        // 2. HEDEF KONTROLÜ (Mevcut hedef öldü mü veya menzilden çýktý mý?)
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);
            IDamageable damageable = currentTarget.GetComponent<IDamageable>();

            // Hedef öldüyse, yok olduysa veya menzilden çýktýysa UNUT
            if (damageable == null || damageable.IsDead || dist > attackRange)
            {
                currentTarget = null;
            }
        }

        // 3. TARAMA (Eðer hedef yoksa veya tarama zamaný geldiyse)
        // Saniyede 5 kere tarama yapmak yeterlidir (0.2f), her frame yapmaya gerek yok.
        if (_scanTimer <= 0)
        {
            _scanTimer = 0.2f;
            ScanForNearestEnemy();
        }
    }

    // --- SALDIRI EMRÝ (SoldierAI çaðýracak) ---
    public void AttackTarget()
    {
        if (currentTarget == null) return;

        if (_attackCooldown <= 0)
        {
            IDamageable enemy = currentTarget.GetComponent<IDamageable>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(damage);
                // Efekt/Ses buraya
            }
            _attackCooldown = attackRate;
        }
    }

    // --- RADAR SÝSTEMÝ (OverlapSphere) ---
    private void ScanForNearestEnemy()
    {
        // Eðer zaten geçerli bir hedefim varsa ve menzildeyse tekrar aramaya gerek yok (Sticky Target)
        // Amaç sürekli hedef deðiþtirmesini engellemek.
        if (currentTarget != null) return;

        // Hayali küreyi oluþtur ve çarpanlarý al
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            // Bulunan þey gerçekten bir düþman mý? (Caný var mý?)
            IDamageable target = hit.GetComponent<IDamageable>();

            if (target != null && !target.IsDead)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);

                // En yakýný kaydet
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = hit.transform;
                }
            }
        }

        currentTarget = nearest;
    }

    // Editörde menzili görmek için
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}