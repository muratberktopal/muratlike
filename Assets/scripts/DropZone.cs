using System.Collections;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    public float dropSpeed = 0.1f; // Boþaltma hýzý

    private void OnTriggerEnter(Collider other)
    {
        // Oyuncudaki StackManager'ý bul
        if (other.TryGetComponent(out StackManager manager))
        {
            // Boþaltma iþlemini baþlat
            StartCoroutine(UnloadStack(manager));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Oyuncu alandan çýkarsa durdur
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
        }
    }

    IEnumerator UnloadStack(StackManager manager)
    {
        // Oyuncunun sýrtýnda eþya olduðu sürece döngüye gir
        while (manager.ItemCount > 0)
        {
            // 1. Oyuncudan eþyayý iste
            Collectable item = manager.RemoveFromStack();

            if (item != null)
            {
                // 2. Eþyayý bu binanýn merkezine taþý (Animasyonlu gibi görünsün)
                item.transform.position = this.transform.position;

                // 3. Eþyayý yok et (Veya depoya ekle)
                Destroy(item.gameObject, 0.5f);
            }

            // 4. Biraz bekle (Pýtýr pýtýr atma efekti için)
            yield return new WaitForSeconds(dropSpeed);
        }
    }
}