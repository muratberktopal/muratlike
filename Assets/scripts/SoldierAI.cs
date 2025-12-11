// SoldierAI.cs
using UnityEngine;

public class SoldierAI : MonoBehaviour, IRecruitable
{
    [Header("Settings")]
    public float followSpeed = 8f;
    public float stoppingDistance = 1.5f; // Öndeki kiþiyle ne kadar mesafe kalsýn?
    public float rotationSpeed = 10f;

    private Transform _target; // Takip edilecek obje (Player veya bir önceki asker)
    private bool _isRecruited = false;

    // Interface implementation
    public bool IsRecruited => _isRecruited;

    public void OnRecruit(Transform targetToFollow)
    {
        _isRecruited = true;
        _target = targetToFollow;

        // Asker gruba katýldýðýnda ufak bir görsel/ses efekti eklenebilir.
        // GetComponent<Animator>().SetBool("IsWalking", true); gibi.
    }

    void Update()
    {
        if (!_isRecruited || _target == null) return;

        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        // Hedef ile aradaki mesafeyi ölç
        float distance = Vector3.Distance(transform.position, _target.position);

        // Eðer mesafe, durma mesafesinden büyükse hareket et
        if (distance > stoppingDistance)
        {
            // Pozisyonu yumuþak bir þekilde hedefe doðru kaydýr (Lerp)
            // Tren efekti için Lerp çok önemlidir.
            transform.position = Vector3.Lerp(transform.position, _target.position, followSpeed * Time.deltaTime);

            // Yüzünü hedefe dön
            Vector3 direction = (_target.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}