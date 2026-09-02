using UnityEngine;

public class ExplosiveSpear: Spear
{
    [SerializeField] private float radius;
    [SerializeField] private float force;


    private void OnCollisionEnter(Collision collision)
    {
        Explosion();
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
                Debug.Log(enemyhit);
                enemyhit.GetComponent<Enemy>().ExplosionHit();
            }
            if (objRb != null)
            {
                objRb.isKinematic = false;
                objRb.AddExplosionForce(force, transform.position, radius);
            }
        }
    }   
}
