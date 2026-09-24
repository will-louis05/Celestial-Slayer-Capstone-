using UnityEngine;

public abstract class RangedEnemy : Enemy
{
    [SerializeField] protected float distanceFromPlayer;
    [SerializeField] protected GameObject shot;

    protected Transform player;
    private float timeSinceLastFired;

    [Header("Damge Values")]
    [SerializeField] protected float fireRate;
    [SerializeField] protected float damageDelt;
    [SerializeField] protected float projectileSpeed;

    protected override void Start()
    {
        player = GameObject.Find("Player").transform;
        base.Start();
    }

    public override bool EnemySpeared(Rigidbody spearRigidbody, Transform limbhit, float spearSpeed, Collision collision)
    {
        Killed();
        spearRigidbody.linearVelocity = spearSpeed * spearRigidbody.transform.forward;
        return false;
    }

    protected override void Killed()
    {
        spawner.currentEnemyCount--;
        Destroy(gameObject);
    }

    protected void FindPlayer()
    {
        Vector3 directionFromPlayer = player.position - transform.position;
        if (distanceFromPlayer > directionFromPlayer.magnitude)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionFromPlayer, out hit))
            {
                Debug.DrawRay(transform.position, directionFromPlayer);
                if (hit.collider.CompareTag("Player"))
                {

                    if (fireRate < (Time.time - timeSinceLastFired))
                    {
                        Fire(hit.point);
                        timeSinceLastFired = Time.time;
                    }
                }
            }
        }
    }

    protected abstract void Fire(Vector3 playerPos);
}
