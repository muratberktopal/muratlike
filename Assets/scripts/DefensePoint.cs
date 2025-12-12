using UnityEngine;
using System.Collections.Generic;

public class DefensePoint : MonoBehaviour
{
    [Header("Settings")]
    public Transform[] defenseSlots;

    // Slot Takibi
    private SoldierAI[] _slotOccupants;

    void Start()
    {
        _slotOccupants = new SoldierAI[defenseSlots.Length];
    }

    // Sadece alana giriş anında çalışır
    private void OnTriggerEnter(Collider other)
    {
        // SquadManager'ı alıyoruz
        SquadManager playerSquad = other.GetComponent<SquadManager>();

        if (playerSquad != null)
        {
            // Karar Anı: İçeride asker var mı?
            if (HasAnySoldier())
            {
                // VAR -> Hepsini geri ver (Retrieve)
                ReturnAllSoldiers(playerSquad);
            }
            else
            {
                // YOK -> Doldurabildiğin kadar doldur (Deploy)
                DeployAllSoldiers(playerSquad);
            }
        }
    }

    // --- DURUM KONTROLÜ ---
    private bool HasAnySoldier()
    {
        for (int i = 0; i < _slotOccupants.Length; i++)
        {
            // Eğer slot doluysa ve asker oradaysa (silinmemişse)
            if (_slotOccupants[i] != null) return true;
        }
        return false;
    }

    // --- HEPSİNİ GERİ VER (RETRIEVE) ---
    private void ReturnAllSoldiers(SquadManager playerSquad)
    {
        for (int i = 0; i < _slotOccupants.Length; i++)
        {
            SoldierAI soldier = _slotOccupants[i];

            // Asker var mı?
            if (soldier != null)
            {
                // Askeri, SquadManager'a geri döndür.
                // SquadManager kendi içinde doluluk kontrolünü yapacak.
                playerSquad.ReturnMemberToSquad(soldier);

                _slotOccupants[i] = null; // Slotu boşalt
            }
        }
    }

    // --- HEPSİNİ DOLDUR (DEPLOY) ---
    private void DeployAllSoldiers(SquadManager playerSquad)
    {
        for (int i = 0; i < _slotOccupants.Length; i++)
        {
            // Eğer bu slot boşsa VE SquadManager'da verilebilecek asker varsa
            if (_slotOccupants[i] == null && playerSquad.CanGiveMember())
            {
                // Oyuncudan asker iste
                SoldierAI soldier = playerSquad.GiveMemberToBuilding();

                // Eğer SquadManager null döndürürse (asker bittiyse) döngüyü kır.
                if (soldier == null) break;

                // Askeri yerleştir ve slotu doldur
                soldier.OnDeploy(defenseSlots[i]);
                _slotOccupants[i] = soldier;
            }
        }
    }
}