using UnityEngine;
using UnityEngine.AI; // <--- NavMesh için bunu eklemek ZORUNLU

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Sahnedeki "TownHall" etiketli binayý bul
        GameObject bina = GameObject.FindGameObjectWithTag("TownHall");

        if (bina != null)
        {
            target = bina.transform;
        }
    }

    void Update()
    {
        // Hedef varsa yürü
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}