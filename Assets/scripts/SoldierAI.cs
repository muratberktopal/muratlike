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
    private Transform _target;
    private float _stoppingDistance;

    public bool IsRecruited => currentState == SoldierState.Following;
    public bool IsWorking => currentState == SoldierState.Working;

    void Start()
    {
        _stoppingDistance = defaultStoppingDistance;
    }

    void Update()
    {
        if (_target == null) return;
        MoveToTarget();
    }

    // --- TAKIMA KATILMA ---
    public bool OnRecruit(Transform targetToFollow)
    {
        // Zaten takýmdaysa iþlem yapma
        if (currentState == SoldierState.Following) return false;

        currentState = SoldierState.Following;
        _target = targetToFollow;
        _stoppingDistance = defaultStoppingDistance; // Mesafeyi normale çevir

        SetPhysics(true);
        return true;
    }

    // --- ÝÞE YERLEÞME ---
    public void OnDeploy(Transform workSlot)
    {
        currentState = SoldierState.Working;
        _target = workSlot;
        _stoppingDistance = 0.1f; // Tam noktaya otursun
    }

    // --- ÝÞÝ BIRAKMA ---
    public void LeaveWork()
    {
        currentState = SoldierState.Idle;
        _target = null;
    }

    private void MoveToTarget()
    {
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
            // Ýþ yerindeyse slotun baktýðý yöne dön
            transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void SetPhysics(bool isKinematic)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }
}