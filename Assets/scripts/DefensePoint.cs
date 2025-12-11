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
        // Sırayı bozmadan baştan sona (0, 1, 2...) geri veriyoruz
        for (int i = 0; i < _slotOccupants.Length; i++)
        {
            SoldierAI soldier = _slotOccupants[i];

            // Asker var mı ve oyuncuda yer var mı?
            if (soldier != null && !playerSquad.IsSquadFull())
            {
                soldier.LeaveWork(); // İşten çıkar
                playerSquad.AddSoldierToSquad(soldier); // Takıma ekle
                _slotOccupants[i] = null; // Slotu boşalt
            }
        }
    }

    // --- HEPSİNİ DOLDUR (DEPLOY) ---
    private void DeployAllSoldiers(SquadManager playerSquad)
    {
        for (int i = 0; i < _slotOccupants.Length; i++)
        {
            // Eğer bu slot boşsa
            if (_slotOccupants[i] == null)
            {
                // Oyuncudan asker iste
                SoldierAI soldier = playerSquad.GiveMemberToBuilding();

                // Oyuncunun askeri bittiyse döngüyü kır
                if (soldier == null) break;

                // Askeri yerleştir
                soldier.OnDeploy(defenseSlots[i]);
                _slotOccupants[i] = soldier;
            }
        }
    }
}