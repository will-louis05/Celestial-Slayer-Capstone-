using UnityEngine;
using UnityEngine.Rendering;

public class EnableObjectTrigger : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private bool EnableSpawnerOverObject;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!EnableSpawnerOverObject)
            {
                objectToEnable.SetActive(true);
            }
            else
            {
                enemySpawner.spawnEnemies = true;
            }
            Destroy(gameObject);
        }
    }
}
