using UnityEngine;

public class ExplosiveSpear: Spear
{
    [SerializeField] private float radius;
    [SerializeField] private float force;
    [SerializeField] private GameObject explosionParticle;
    private int killedEnemyCount;


    private void OnCollisionEnter(Collision collision)
    {
        Explosion();
        Instantiate(explosionParticle, collision.contacts[0].point, Quaternion.identity);
        Destroy(gameObject);
    }

    private void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        Transform enemyhit = null;
        foreach (Collider collider in colliders)
        {
            Rigidbody objRb = collider.GetComponent<Rigidbody>();
            if (collider.CompareTag("Enemy"))
            {
                enemyhit = collider.transform.root;
                Enemy enemyHitScrp = enemyhit.GetComponent<Enemy>();
                if (enemyHitScrp != null)
                {
                    killedEnemyCount++;
                    enemyHitScrp.ExplosionHit();
                    AmmoManager ammoScrpt = GameObject.Find("Player").GetComponent<AmmoManager>();
                    if (killedEnemyCount == ammoScrpt.killsNeedForRefund)
                        GameObject.Find("Player").GetComponent<AmmoManager>().RefundSpear();
                    }
            }
            if (objRb != null)
            {
                objRb.isKinematic = false;
                objRb.AddExplosionForce(force, transform.position, radius);
            }
        }
    }   
}
