using System.Collections;
using UnityEngine;
using DG.Tweening; // DOTween (Sallanma efekti için)

public class ResourceSource : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject resourcePrefab; // Düþecek odun (Senin Collectable prefabýn)
    public float uretimHizi = 1.0f;   // Kaç saniyede bir odun atsýn?
    public int maxRezerv = 5;         // Bu aðaçta toplam kaç odun var?

    private bool oyuncuYaninda = false;
    private bool uretiyor = false;

    // Oyuncu alana girdi
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Player tag'i önemli!
        {
            oyuncuYaninda = true;
            if (!uretiyor && maxRezerv > 0)
            {
                StartCoroutine(UretimDongusu());
            }
        }
    }

    // Oyuncu alandan çýktý
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYaninda = false;
            // Coroutine otomatik durur çünkü while döngüsünde kontrol ediyoruz
        }
    }

    IEnumerator UretimDongusu()
    {
        uretiyor = true;

        while (oyuncuYaninda && maxRezerv > 0)
        {
            // 1. Aðacý salla (Görsel Efekt) - DOTween
            transform.DOShakeScale(0.2f, 0.1f);

            // 2. Odunu yarat (Aðacýn biraz yukarýsýnda)
            Vector3 spawnPos = transform.position + Vector3.up * 2f;

            // Rastgele saða sola fýrlasýn ki doðal dursun
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            GameObject yeniOdun = Instantiate(resourcePrefab, spawnPos + randomOffset, Quaternion.identity);

            if (yeniOdun.TryGetComponent(out Rigidbody rb))
            {
                // Yerçekimi açýk mý emin ol
                rb.useGravity = true;
                rb.isKinematic = false;

                // Rastgele bir yöne fýrlat (Havai fiþek gibi etrafa saçýlsýn)
                // Vector3.up * 3f -> Yukarý zýplasýn
                // Random.onUnitSphere * 2f -> Saða sola rastgele gitsin
                Vector3 forceDirection = (Vector3.up * 4f) + (Random.onUnitSphere * 2f);

                rb.AddForce(forceDirection, ForceMode.Impulse); // Impulse = Ani Darbe

                // Havada dönsün (Görsel þov)
                rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            }

            // 3. Rezervi düþür
            maxRezerv--;

            // 4. Bekle
            yield return new WaitForSeconds(uretimHizi);
        }

        // Rezerv bittiyse aðacý yok et veya kurut (Þimdilik yok edelim)
        if (maxRezerv <= 0)
        {
            // Aðaç küçülerek yok olsun
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => Destroy(gameObject));
        }

        uretiyor = false;
    }
}