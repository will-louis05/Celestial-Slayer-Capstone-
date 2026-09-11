using System.Threading;
using UnityEngine;

public class AreaOfEffectAttack : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public float damgeOverTime;
    public float damgeTicTime;
    private float timeSinceLastDamged;
    public float timeTillDestory;

    private void Start()
    {
        timeSinceLastDamged = Time.time;
        Invoke(nameof(DestoryArea), timeTillDestory);
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
                playerHealth.Hit(damgeOverTime);
                timeSinceLastDamged = Time.time;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        timeSinceLastDamged = Time.time;
    }

    private void DestoryArea()
    {
        Destroy(gameObject);
    }
}
