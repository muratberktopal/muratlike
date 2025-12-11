using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    [Header("Squad Settings")]
    public int maxSquadSize = 10;
    public Transform followPoint;

    [Header("Detection Settings")]
    public float detectionRadius = 3f; // Asker toplama mesafesi
    public LayerMask recruitLayer; // Sadece "Recruitable" layerýný tara (Performans için þart)

    [Header("Debug Info")]
    [SerializeField] private List<SoldierAI> currentSquad = new List<SoldierAI>();

    // Performans Optimizasyonu: Her frame tarama yapmak yerine saniyede 5-10 kere yapmak yeterlidir.
    private float _scanTimer;
    private float _scanInterval = 0.1f; // Saniyede 10 kez tarar

    void Update()
    {
        HandleRecruitmentScan();
    }

    private void HandleRecruitmentScan()
    {
        _scanTimer -= Time.deltaTime;
        if (_scanTimer > 0) return;

        _scanTimer = _scanInterval; // Sayacý sýfýrla

        // Karakterin etrafýnda hayali bir küre oluþtur ve içindeki objeleri bul
        // Bu metod, Sphere Collider'ýn kod karþýlýðýdýr.
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, recruitLayer);

        foreach (var hit in hits)
        {
            // Bulunan obje IRecruitable mý? (Interface kontrolü)
            IRecruitable recruit = hit.GetComponent<IRecruitable>();

            if (recruit != null && !recruit.IsRecruited)
            {
                if (currentSquad.Count < maxSquadSize)
                {
                    // Asker scriptine (SoldierAI) eriþ ve ekle
                    SoldierAI soldier = hit.GetComponent<SoldierAI>();
                    if (soldier != null)
                    {
                        AddSoldierToSquad(soldier);
                    }
                }
            }
        }
    }

    private void AddSoldierToSquad(SoldierAI newSoldier)
    {
        Transform targetForNewSoldier;

        if (currentSquad.Count == 0)
            targetForNewSoldier = followPoint;
        else
            targetForNewSoldier = currentSquad[currentSquad.Count - 1].transform;

        newSoldier.OnRecruit(targetForNewSoldier);
        currentSquad.Add(newSoldier);
    }

    // Editörde toplama alanýný görebilmek için (Gizmos)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}