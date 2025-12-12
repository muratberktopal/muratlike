using UnityEngine;

public enum SoldierState { Idle, Following, Working, Attacking } // Attacking eklendi

public class SoldierAI : MonoBehaviour, IRecruitable
{
    [Header("Settings")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 12f;
    public float defaultStoppingDistance = 1.2f;

    [Header("Debug")]
    [SerializeField] private SoldierState currentState = SoldierState.Idle;
    private Transform _target;
    private float _stoppingDistance;

    // BÝLEÞEN REFERANSI
    private SoldierCombat _combatModule;

    public bool IsRecruited => currentState == SoldierState.Following;
    public bool IsWorking => currentState == SoldierState.Working;

    void Awake()
    {
        // Ayný obje üzerindeki Combat scriptini bul
        _combatModule = GetComponent<SoldierCombat>();
    }

    void Start()
    {
        _stoppingDistance = defaultStoppingDistance;
    }

    void Update()
    {
        // --- YENÝ SAVAÞ MANTIÐI ---
        if (_combatModule != null && _combatModule.HasTarget)
        {
            // Eðer hedef varsa, normal iþleri býrakýp SAVAÞ MODUNA geç
            HandleCombat();
            return; // Aþaðýdaki hareket kodlarýný çalýþtýrma
        }
        // ---------------------------

        if (_target == null) return;
        MoveToTarget();
    }

    private void HandleCombat()
    {
        currentState = SoldierState.Attacking;

        // 1. Hedefe Dön (Yüzünü düþmana çevir)
        Transform enemy = _combatModule.CurrentTarget;
        Vector3 direction = (enemy.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        // 2. Saldýr
        _combatModule.AttackTarget();
    }

    // --- DÝÐER FONKSÝYONLAR AYNEN KALIYOR ---
    public bool OnRecruit(Transform targetToFollow)
    {
        if (currentState == SoldierState.Following) return false;
        currentState = SoldierState.Following;
        _target = targetToFollow;
        _stoppingDistance = defaultStoppingDistance;
        SetPhysics(true);
        return true;
    }

    public void OnDeploy(Transform workSlot)
    {
        currentState = SoldierState.Working;
        _target = workSlot;
        _stoppingDistance = 0.1f;
    }

    public void LeaveWork()
    {
        currentState = SoldierState.Idle;
        _target = null;
    }

    private void MoveToTarget()
    {
        // Eðer savaþtan çýktýysak durumu düzelt
        if (currentState == SoldierState.Attacking)
        {
            // Eski duruma dön (Working veya Following)
            // Basitçe: Target neyse ona uygun state'i seçebilirsin ama
            // þimdilik IsRecruited mantýðýyla otomatik düzelir.
            if (_stoppingDistance < 1f) currentState = SoldierState.Working;
            else currentState = SoldierState.Following;
        }

        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance > _stoppingDistance)
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
            }
            transform.position = Vector3.MoveTowards(transform.position, _target.position, moveSpeed * Time.deltaTime);
        }
        else if (currentState == SoldierState.Working)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void SetPhysics(bool isKinematic)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }
}