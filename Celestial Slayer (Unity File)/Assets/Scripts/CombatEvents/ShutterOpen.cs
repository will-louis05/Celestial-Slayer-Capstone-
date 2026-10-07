using UnityEngine;

public class ShutterOpen : MonoBehaviour, ICombatEvent
{
    [SerializeField] private float shutterOpenTimeSecs;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject[] shutters;
    private bool shutterOpening;
    public void PostCombatEvent()
    {
        shutterOpening = true;
    }

    private void Update()
    {
        if (shutterOpening)
        {
            foreach (GameObject s in shutters)
            {
                float yScale = s.transform.localScale.y - (shutterOpenTimeSecs * Time.deltaTime);
                s.transform.localScale = new Vector3(s.transform.localScale.x, yScale, s.transform.localScale.z);
                if (yScale <= 0)
                {
                    if (spawner != null)
                    {
                        spawner.spawnEnemies = true;
                    }
                    Destroy(shutters[0]);  
                    Destroy(shutters[1]);
                    Destroy(gameObject);
                }
            }
        }
    }
    
    public void TriggerEvent()
    {
        Debug.Log("shutterTrigger");
        shutterOpening = true;
    }
}
