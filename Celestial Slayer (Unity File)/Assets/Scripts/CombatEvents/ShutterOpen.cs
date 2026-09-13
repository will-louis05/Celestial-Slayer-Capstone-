using UnityEngine;

public class ShutterOpen : MonoBehaviour, ICombatEvent
{
    [SerializeField] private float shutterOpenTimeSecs;
    [SerializeField] private EnemySpawner spawner;
    private bool shutterOpening;
    public void PostCombatEvent()
    {
        shutterOpening = true;
    }

    private void Update()
    {
        if (shutterOpening)
        { 
            float yScale = transform.localScale.y - (shutterOpenTimeSecs * Time.deltaTime);
            transform.localScale = new Vector3(transform.localScale.x, yScale, transform.localScale.z);
            if (yScale <= 0)
            {
                spawner.spawnEnemies = true;
                Destroy(gameObject);
            }
        }
    }
}
