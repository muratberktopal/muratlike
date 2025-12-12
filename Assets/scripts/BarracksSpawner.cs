using System.Collections;
using UnityEngine;
using DG.Tweening; // Görsel güzellik için DOTween (ResourceSource'da kullandýðýn için ekledim)

public class BarracksSpawner : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private GameObject soldierPrefab;   // Üretilecek Asker
    [SerializeField] private Transform spawnPoint;       // Askerin çýkacaðý nokta
    [SerializeField] private Transform inputPoint;       // Odunun gideceði huni/kapý noktasý

    [Header("Maliyet")]
    [SerializeField] private int costPerUnit = 3;        // 1 Asker kaç odun?
    [SerializeField] private float paymentSpeed = 0.2f;  // Odunlarý ne hýzla alsýn?

    private int _currentPaidAmount = 0; // Þu ana kadar ödenen miktar
    private Coroutine _productionCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        // Oyuncu alana girdi mi?
        if (other.TryGetComponent(out StackManager manager))
        {
            if (_productionCoroutine == null)
                _productionCoroutine = StartCoroutine(ProcessPayment(manager));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Oyuncu alandan çýktýysa durdur
        if (other.CompareTag("Player"))
        {
            if (_productionCoroutine != null)
            {
                StopCoroutine(_productionCoroutine);
                _productionCoroutine = null;
            }
        }
    }

    private IEnumerator ProcessPayment(StackManager manager)
    {
        // Oyuncunun sýrtýnda eþya olduðu sürece ve oyuncu alanda kaldýðý sürece
        while (manager.ItemCount > 0)
        {
            // 1. Odunu oyuncudan al
            Collectable item = manager.RemoveFromStack();

            if (item != null)
            {
                // 2. Odunu kýþlanýn giriþine taþý (Görsel efekt)
                item.transform.DOJump(inputPoint.position, 1.5f, 1, 0.3f)
                    .OnComplete(() =>
                    {
                        // Odun hedefe varýnca yok et
                        Destroy(item.gameObject);

                        // Ödemeyi kaydet
                        AddProgress(manager.transform);
                    });

                // Bir sonraki odunu almadan önce bekle
                yield return new WaitForSeconds(paymentSpeed);
            }
            else
            {
                // Eþya null geldiyse döngüyü kýr (Güvenlik)
                yield break;
            }
        }

        _productionCoroutine = null;
    }

    private void AddProgress(Transform playerTransform)
    {
        _currentPaidAmount++;
        Debug.Log($"Kýþla: {_currentPaidAmount}/{costPerUnit} odun ödendi.");

        // Yeterli odun birikti mi?
        if (_currentPaidAmount >= costPerUnit)
        {
            SpawnSoldier(playerTransform);
            _currentPaidAmount = 0; // Sayacý sýfýrla
        }
    }

    private void SpawnSoldier(Transform leaderToFollow)
    {
        if (soldierPrefab == null) return;

        // 1. Askeri yarat
        GameObject newSoldier = Instantiate(soldierPrefab, spawnPoint.position, Quaternion.identity);

        // 2. Askeri otomatik recruit et (Takýma kat)
        // IRecruitable arayüzünü kullandýðýn için sisteminle %100 uyumlu olur.
        if (newSoldier.TryGetComponent(out IRecruitable recruit))
        {
            // Oyuncuyu takip etmesini söyle
            bool joined = recruit.OnRecruit(leaderToFollow);

            if (joined)
            {
                // Opsiyonel: Asker çýktý efekti (Ses, partikül vs.)
                // newSoldier.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
            }
        }

        // Eðer SquadManager'ýn özel bir "AddMember" fonksiyonu varsa ve IRecruitable bunu kapsamýyorsa,
        // burada leaderToFollow üzerinden SquadManager'a eriþip ekleme yapabilirsin.
    }
}