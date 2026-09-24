using UnityEngine;

public class ExplosiveSpear: Spear
{
    [SerializeField] private float radius;
    [SerializeField] private float radiusRatio;
    [SerializeField] private float force;
    [SerializeField] private GameObject explosionParticle;
    private GameObject explosion;
    [SerializeField] private GameObject debugSphere;
    private int killedEnemyCount;
    private bool drawGizmo;


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
        GameObject sphere = Instantiate(debugSphere, transform.position, Quaternion.identity);
        var sphereSize = radius * 2;
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
    }

    private void DestoryExplosionVFX()
    {
        Destroy(explosion);
    }
}
