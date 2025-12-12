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
        if (IsSquadFull()) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, recruitLayer);
        foreach (var hit in hits)
        {
            SoldierAI soldier = hit.GetComponent<SoldierAI>();

            // KURAL: Asker çalýþýyorsa onu görmezden gel.
            if (soldier != null && !soldier.IsRecruited && !soldier.IsWorking)
            {
                TryAddRecruit(soldier);
            }
        }
    }

    // Takýma yeni bir askeri eklemeyi dener
    private void TryAddRecruit(SoldierAI newSoldier)
    {
        // Zaten listedeyse iþlem yapma
        if (currentSquad.Contains(newSoldier)) return;

        // Hedefi belirle (Ya FollowPoint ya da son asker)
        Transform target = (currentSquad.Count == 0) ? followPoint : currentSquad[currentSquad.Count - 1].transform;

        // Askere emri ver. Baþarýlý olursa (true dönerse) listeye ekle.
        if (newSoldier.OnRecruit(target))
        {
            currentSquad.Add(newSoldier);
            // DEBUG: Debug.Log($"Asker takýma eklendi: {newSoldier.name}");
        }
    }


    // --- DIÞ KULLANIM METOTLARI (DefensePoint için) ---

    // Dýþarýdan bir asker (örneðin bir binadan) takýma geri döner
    public void ReturnMemberToSquad(SoldierAI returnedSoldier)
    {
        if (returnedSoldier == null || currentSquad.Contains(returnedSoldier) || IsSquadFull()) return;

        returnedSoldier.LeaveWork(); // Askerin durumunu Idle'a çek
        TryAddRecruit(returnedSoldier); // Takip zincirine girmeyi dene
    }

    // Takýmdan en son askeri bir binaya vermek için ayýrýr.
    public SoldierAI GiveMemberToBuilding()
    {
        if (currentSquad.Count == 0) return null;

        // Listenin sonundaki askeri al
        SoldierAI soldier = currentSquad[currentSquad.Count - 1];

        // ÖNEMLÝ: Askere 'LeaveWork' emri VERME! Bu emri sadece bina (DefensePoint) verecek,
        // askerini aldýktan sonra onu kendi slotuna yerleþtirirken.

        currentSquad.RemoveAt(currentSquad.Count - 1); // Listeden çýkar
        return soldier;
    }

    // Takýmýn dolu olup olmadýðýný kontrol et
    public bool IsSquadFull()
    {
        return currentSquad.Count >= maxSquadSize;
    }

    // Takýmda asker olup olmadýðýný kontrol et
    public bool CanGiveMember()
    {
        return currentSquad.Count > 0;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}