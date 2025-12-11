using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    [Header("Squad Settings")]
    public int maxSquadSize = 10;
    public Transform followPoint;
    public float detectionRadius = 3f;
    public LayerMask recruitLayer;

    [SerializeField] private List<SoldierAI> currentSquad = new List<SoldierAI>();
    private float _scanTimer;

    void Update()
    {
        _scanTimer -= Time.deltaTime;
        if (_scanTimer <= 0)
        {
            _scanTimer = 0.2f;
            ScanForRecruits();
        }
    }

    private void ScanForRecruits()
    {
        if (currentSquad.Count >= maxSquadSize) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, recruitLayer);
        foreach (var hit in hits)
        {
            SoldierAI soldier = hit.GetComponent<SoldierAI>();

            // KESÝN KURAL: Asker çalýþýyorsa (IsWorking) onu GÖRMEZDEN GEL.
            // Onu sadece DefensePoint (Kule) scripti bize geri verebilir.
            if (soldier != null && !soldier.IsRecruited && !soldier.IsWorking)
            {
                AddSoldierToSquad(soldier);
            }
        }
    }

    public void AddSoldierToSquad(SoldierAI newSoldier)
    {
        // Zaten listedeyse iþlem yapma
        if (currentSquad.Contains(newSoldier)) return;

        // Hedefi belirle (Ya FollowPoint ya da son asker)
        Transform target = (currentSquad.Count == 0) ? followPoint : currentSquad[currentSquad.Count - 1].transform;

        // ÖNEMLÝ DEÐÝÞÝKLÝK:
        // Önce askere emri ver, eðer asker "Tamam geliyorum (true)" derse listeye ekle.
        // Eðer "Meþgulüm (false)" derse listeye ekleme.
        if (newSoldier.OnRecruit(target))
        {
            currentSquad.Add(newSoldier);
        }
    }

    public SoldierAI GiveMemberToBuilding()
    {
        if (currentSquad.Count == 0) return null;

        SoldierAI soldier = currentSquad[currentSquad.Count - 1];
        currentSquad.RemoveAt(currentSquad.Count - 1);
        return soldier;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public bool IsSquadFull()
    {
        return currentSquad.Count >= maxSquadSize;
    }

}