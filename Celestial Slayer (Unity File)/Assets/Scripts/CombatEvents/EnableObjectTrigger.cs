using UnityEngine;
using UnityEngine.Rendering;

public class EnableObjectTrigger : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private bool EnableSpawnerOverObject;
    [SerializeField] private GameObject interfaceObject;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!EnableSpawnerOverObject && objectToEnable != null)
            {
                objectToEnable.SetActive(true);
            }
            else if(enemySpawner != null)
            {
                enemySpawner.spawnEnemies = true;
            }
            else if(interfaceObject != null)
            {
                interfaceObject.GetComponent<ICombatEvent>().TriggerEvent();
            }
            Destroy(gameObject);
        }
    }
}
