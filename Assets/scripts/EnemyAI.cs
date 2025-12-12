using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Combat Settings")]
    public int damage = 10;
    public float attackRate = 1.5f; // Saniyede kaç vuruþ?
    public float attackRange = 3.0f; // Binaya ne kadar yaklaþýnca vursun?

    private NavMeshAgent agent;
    private Transform target;
    private float _nextAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // "TownHall" etiketli binayý bul
        GameObject bina = GameObject.FindGameObjectWithTag("TownHall");
        if (bina != null)
        {
            target = bina.transform;

            // NavMesh'in durma mesafesini bizim saldýrý mesafemizle eþitleyelim
            // Böylece düþman tam içine girmeye çalýþmaz, vurabileceði yerde durur.
            agent.stoppingDistance = attackRange - 0.5f;
        }
    }

    void Update()
    {
        if (target == null) return; // Hedef yoksa (Bina yýkýldýysa) dur

        // Hedef ile aradaki mesafeyi ölç
        float distance = Vector3.Distance(transform.position, target.position);

        // 1. SALDIRI MENZÝLÝNDE MÝYÝZ?
        if (distance <= attackRange)
        {
            // Menzildeyiz -> Dur ve Saldýr
            agent.isStopped = true; // Yürümeyi kes
            AttackBase();
        }
        else
        {
            // Uzaktayýz -> Yürümeye Devam Et
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    private void AttackBase()
    {
        // Saldýrý hýzý kontrolü (Cooldown)
        if (Time.time >= _nextAttackTime)
        {
            // Binadaki "IDamageable" özelliðini bul (BuildingHealth scripti)
            IDamageable building = target.GetComponent<IDamageable>();

            if (building != null && !building.IsDead)
            {
                building.TakeDamage(damage);
                // Debug.Log("Düþman binaya vurdu!");

                // Vuruþ efekti/animasyonu buraya eklenecek
            }

            _nextAttackTime = Time.time + attackRate;
        }
    }
}