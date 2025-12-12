using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private GameObject enemyPrefab;    // Üretilecek düþman
    [SerializeField] private float spawnInterval = 2f;  // Kaç saniyede bir?

    [Header("Spawn Noktalarý")]
    [SerializeField] private List<Transform> spawnPoints; // Esnek Liste (Array yerine List daha iyidir)

    private void Start()
    {
        // --- GÜVENLÝK KONTROLÜ ---
        // Eðer listeyi boþ unuttuysan oyun hata vermesin, seni uyarsýn.
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("HATA: EnemySpawner scriptine Spawn Noktasý atamayý unuttun!");
            return; // Kodu burada durdur
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("HATA: Enemy Prefab'ý atanmamýþ!");
            return;
        }

        // Her þey tamsa üretime baþla
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true) // Oyun bitene kadar döngü
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        // 1. Rastgele bir index seç
        int randomIndex = Random.Range(0, spawnPoints.Count);

        // 2. O noktayý al
        Transform selectedPoint = spawnPoints[randomIndex];

        // 3. Düþmaný yarat (Rotasyon önemli deðilse Quaternion.identity)
        Instantiate(enemyPrefab, selectedPoint.position, Quaternion.identity);
    }
}