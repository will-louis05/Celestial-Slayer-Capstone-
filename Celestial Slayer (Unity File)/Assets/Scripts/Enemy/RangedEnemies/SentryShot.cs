using UnityEngine;

public class SentryShot : MonoBehaviour
{
    private float shotSpeed;
    private bool fired;
    private Rigidbody rb;
    private float damage;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 playerPos, float projectileSpeed, float damageDelt)
    {
        shotSpeed = projectileSpeed;
        damage = damageDelt;
        transform.LookAt(playerPos);
        fired = true;
    }

    private void Update()
    {
        if (fired)
        {
            rb.MovePosition(transform.position + transform.forward * shotSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.transform.GetComponent<PlayerHealth>().Hit(damage, false);
            Destroy(gameObject);
        }
        else if(!collider.CompareTag("Enemy"))
            Destroy(gameObject);
    }
}
