using UnityEngine;

public enum SoldierState { Idle, Following, Working }

public class SoldierAI : MonoBehaviour, IRecruitable
{
    [Header("Settings")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 12f;
    public float defaultStoppingDistance = 1.2f;

    [Header("Debug")]
    [SerializeField] private SoldierState currentState = SoldierState.Idle;
    private Transform _target; // Takip edilen (Player veya Slot)
    private float _stoppingDistance;

    // Referanslar
    private SoldierCombat _combatModule;

    // Propertyler
    public bool IsRecruited => currentState == SoldierState.Following;
    public bool IsWorking => currentState == SoldierState.Working;

    void Awake()
    {
        _combatModule = GetComponent<SoldierCombat>();
    }

    void Start()
    {
        _stoppingDistance = defaultStoppingDistance;
    }

    void Update()
    {
        // 1. ÖNCE SAVAÞ KONTROLÜ
        // Hedef var mý? Varsa ateþ et (Hareketten baðýmsýz)
        bool hasEnemy = _combatModule != null && _combatModule.HasTarget;
        if (hasEnemy)
        {
            _combatModule.AttackTarget();
        }

        // 2. HAREKET VE ROTASYON MANTIÐI
        if (_target == null) return;

        if (currentState == SoldierState.Working)
        {
            HandleWorkingState(hasEnemy);
        }
        else if (currentState == SoldierState.Following)
        {
            HandleFollowingState(hasEnemy);
        }
    }

    // --- DURUM 1: ÇALIÞMA MODU (SABÝT SAVUNMA) ---
    // --- DURUM 1: ÇALIÞMA MODU (YERÝNE GÝT VE SAVUN) ---
    // --- DURUM 1: ÇALIÞMA MODU (YERÝNE GÝT VE SAVUN) ---
    private void HandleWorkingState(bool hasEnemy)
    {
        // 1. HAREKET: Önce atandýðým slota gitmeliyim!
        // Havada asýlý kalma sorununun çözümü burasý:
        float distance = Vector3.Distance(transform.position, _target.position);

        // OnDeploy içinde stoppingDistance 0.1f yapýlmýþtý, yani tam noktaya gidene kadar yürü.
        if (distance > _stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, moveSpeed * Time.deltaTime);
        }

        // 2. ROTASYON: Nereye bakacaðým?
        if (hasEnemy)
        {
            // Düþman varsa, yürürken bile ona bak (Ateþ ederek yerine git)
            RotateTowards(_combatModule.CurrentTarget.position);
        }
        else
        {
            // Düþman yoksa...
            if (distance > _stoppingDistance)
            {
                // Henüz yerime varmadým, yürüdüðüm yere (Slota) bakayým
                RotateTowards(_target.position);
            }
            else
            {
                // Yerime vardým, artýk slotun baktýðý yöne (Nöbet yönüne) dönebilirim
                Quaternion targetRot = _target.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // --- DURUM 2: TAKÝP MODU (KOÞ VE ATEÞ ET) ---
    private void HandleFollowingState(bool hasEnemy)
    {
        // A. HAREKET (Her zaman oyuncuyu takip et)
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance > _stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, moveSpeed * Time.deltaTime);
        }

        // B. ROTASYON (Duruma göre deðiþir)
        if (hasEnemy)
        {
            // Hareket etsem bile yüzüm düþmana baksýn (Ateþ etmek için)
            RotateTowards(_combatModule.CurrentTarget.position);
        }
        else
        {
            // Düþman yoksa, gittiðim yöne (oyuncuya) bakayým
            if (distance > _stoppingDistance) // Sadece hareket ediyorsam döneyim
            {
                RotateTowards(_target.position);
            }
        }
    }

    // --- YARDIMCI: DÖNÜÞ FONKSÝYONU ---
    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Yere paralel kal, havaya/yere bakma

        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }
    }

    // --- Interface Metotlarý ---
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

    private void SetPhysics(bool isKinematic)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }
}