using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Eðer DOTween yoksa, alttaki Notu oku!

public class StackManager : MonoBehaviour
{
    public Transform stackContainer; // Sýrtýmýzdaki boþ obje
    public float yOffset = 0.3f;     // Objeler arasý dikey boþluk
    public int maxCapacity = 10;     // Çanta limiti

    // Sýrtýmýzdaki objeleri tutan liste
    public List<GameObject> collectedItems = new List<GameObject>();

    // Eþya Ekleme Fonksiyonu
    public void AddItem(GameObject item)
    {
        if (collectedItems.Count >= maxCapacity)
        {
            // Kapasite doluysa alma (veya uyarý ver)
            return;
        }

        // 1. Fizik özelliklerini kapat (artýk bir obje deðil, görsel bir yük)
        item.GetComponent<Collider>().enabled = false;

        // 2. Sýrt çantasýna taþý (Parent yap)
        item.transform.SetParent(stackContainer);

        // 3. Hedef pozisyonu hesapla (Üst üste dizme matematiði)
        Vector3 targetPos = new Vector3(0, collectedItems.Count * yOffset, 0);

        // 4. Animasyonlu taþýma (Burada yerel pozisyon kullanýyoruz)
        // Eðer DOTween yoksa: item.transform.localPosition = targetPos; yazabilirsin.
        // Ama varsa bu çok daha havalý olur:
        item.transform.DOLocalJump(targetPos, 0.5f, 1, 0.3f);

        // 5. Rotasyonu düzelt
        item.transform.localRotation = Quaternion.identity;

        // 6. Listeye ekle
        collectedItems.Add(item);
    }

    // Eþya Çýkarma Fonksiyonu (DropZone çaðýracak)
    public GameObject RemoveItem()
    {
        if (collectedItems.Count > 0)
        {
            // Listenin son elemanýný (en üsttekini) al
            GameObject itemToRemove = collectedItems[collectedItems.Count - 1];

            // Listeden sil
            collectedItems.RemoveAt(collectedItems.Count - 1);

            // Objeyi Player'dan kopar (Unparent)
            itemToRemove.transform.SetParent(null);

            return itemToRemove; // Objeyi DropZone'a veriyoruz
        }
        return null; // Çanta boþ
    }
}