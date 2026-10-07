using UnityEngine;

public class ExplosiveSpear: Spear
{
    [SerializeField] private float radius;
    [SerializeField] private float radiusRatio;
    [SerializeField] private float force;
    [SerializeField] private GameObject explosionParticle;
    private GameObject explosion;
    private int killedEnemyCount;

    private void OnCollisionEnter(Collision collision)
    {
        Explosion();
        explosion = Instantiate(explosionParticle, collision.contacts[0].point, Quaternion.identity);
        Invoke(nameof(DestoryExplosionVFX), 2f);
        Destroy(gameObject);
    }

    private void Explosion()
    {
        radius = throwSpeed*radiusRatio;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        Transform enemyhit = null;
        foreach (Collider collider in colliders)
        {
            Rigidbody objRb = collider.GetComponent<Rigidbody>();
            if (collider.CompareTag("Enemy"))
            {
                enemyhit = collider.transform.root;
                Enemy enemyHitScrp = enemyhit.GetComponent<Enemy>();
                if (enemyHitScrp != null && !enemyHitScrp.dead)
                { 
                    if(enemyHitScrp is MeleeEnemy mEnemy)
                    {
                        if(mEnemy.isBrute)
                            continue;
                    }
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

        //Update kill counter
        if (killedEnemyCount > 0)
            UIManager.instance.AddKills(killedEnemyCount);
    }

    private void DestoryExplosionVFX()
    {
        Destroy(explosion);
    }
}
