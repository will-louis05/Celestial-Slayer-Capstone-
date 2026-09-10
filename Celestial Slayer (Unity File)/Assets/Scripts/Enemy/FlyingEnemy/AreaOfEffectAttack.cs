using System.Threading;
using UnityEngine;

public class AreaOfEffectAttack : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public float damgeOverTime;
    public float damgeTicTime;
    private float timeSinceLastDamged;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.CompareTag("Player"))
        {
            if (playerHealth != null)
            { 
                playerHealth = collider.transform.GetComponent<PlayerHealth>();
            }
            if (damgeTicTime > (Time.time - timeSinceLastDamged))
            {
                playerHealth.Hit(damgeOverTime);
                timeSinceLastDamged = Time.time;
            }
        }
    }
}
