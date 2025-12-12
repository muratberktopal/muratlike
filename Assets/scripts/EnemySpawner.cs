using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // Kimi doðurayým?
    public Transform spawnPoint;    // Nerede doðurayým?
    public float saniye = 3f;       // Kaç saniyede bir?

    void Start()
    {
        StartCoroutine(Dogur());
    }

    IEnumerator Dogur()
    {
        while (true) // Sonsuza kadar döngü
        {
            // Düþmaný yarat
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            // Bekle
            yield return new WaitForSeconds(saniye);
        }
    }
}