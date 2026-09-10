using System.Threading;
using UnityEngine;

public class AreaOfEffectAttack : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public float damgeOverTime;
    public float damgeTicTime;
    private float timeSinceLastDamged;
    public float timeTillDestory;
    private float aliveTime;

    private void Start()
    {
        timeSinceLastDamged = Time.time;
    }

    private void Update()
    {
        aliveTime += Time.deltaTime;
        if(timeTillDestory < aliveTime)
            Destroy(gameObject);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.transform.CompareTag("Player"))
        {
            Debug.Log("PlayerAttack");
            if (playerHealth == null)
            { 
                playerHealth = collider.transform.GetComponent<PlayerHealth>();
            }
            if (damgeTicTime < (Time.time - timeSinceLastDamged))
            {
                Debug.Log("dmg");
                playerHealth.Hit(damgeOverTime);
                timeSinceLastDamged = Time.time;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        timeSinceLastDamged = Time.time;
    }
}
