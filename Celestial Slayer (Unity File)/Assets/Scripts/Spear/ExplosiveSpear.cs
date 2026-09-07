using UnityEngine;

public class ExplosiveSpear: Spear
{
    [SerializeField] private float radius;
    [SerializeField] private float force;
    [SerializeField] private GameObject explosionParticle;


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
            if (collider.CompareTag("Enemy") && collider.transform.root != enemyhit)
            {
                enemyhit = collider.transform.root;
                Enemy enemyHitScrp = enemyhit.GetComponent<Enemy>();
                if(enemyHitScrp != null)
                    enemyHitScrp.ExplosionHit();
            }
            if (objRb != null)
            {
                objRb.isKinematic = false;
                objRb.AddExplosionForce(force, transform.position, radius);
            }
        }
    }   
}
