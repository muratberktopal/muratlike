using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // <--- Bunu eklemezsen çalýþmaz!

public class StackManager : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform stackContainer; // Sýrt noktasý
    public float yOffset = 0.3f;     // Dizilme aralýðý
    public int maxCapacity = 10;     // Kapasite
    public float jumpDuration = 0.5f;// Sahneye uçuþ süresi

    // Liste
    private List<Collectable> collectedList = new List<Collectable>();

    // DropZone eriþimi için sayý bilgisi
    public int ItemCount => collectedList.Count;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Collectable item))
        {
            if (item.IsCollected) return;

            if (collectedList.Count < maxCapacity)
            {
                // Fiziðini kapatýp çantaya alýyoruz
                AddToStack(item);
            }
        }
    }

    // --- TOPLAMA (Çarpýþma) ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Collectable item))
        {
            if (item.IsCollected) return;

            if (collectedList.Count < maxCapacity)
            {
                AddToStack(item);
            }
        }
    }

    // --- EKLEME (Animasyonlu) ---
    public void AddToStack(Collectable item)
    {
        item.Collect();
        collectedList.Add(item);

        // Oyuncunun çocuðu yap
        item.transform.SetParent(stackContainer);

        // Hedef pozisyonu hesapla (Lokal)
        Vector3 targetPos = new Vector3(0, (collectedList.Count - 1) * yOffset, 0);

        // DOTween Büyüsü: Zýplayarak yerine git
        // (Hedef, Zýplama Gücü, Zýplama Sayýsý, Süre)
        item.transform.DOLocalJump(targetPos, 2f, 1, jumpDuration)
            .OnComplete(() =>
            {
                // Vardýðýnda rotasyonu düzelt (Yamuk durmasýn)
                item.transform.localRotation = Quaternion.identity;
            });
    }

    // --- ÇIKARMA ---
    public Collectable RemoveFromStack()
    {
        if (collectedList.Count == 0) return null;

        int lastIndex = collectedList.Count - 1;
        Collectable item = collectedList[lastIndex];

        collectedList.RemoveAt(lastIndex);

        // Eðer havada uçarken çýkarýlýrsa animasyonu durdur (Bug olmasýn)
        item.transform.DOKill();

        item.transform.SetParent(null);

        return item;
    }
}