using UnityEngine;

public class SentryEnemy : RangedEnemy
{
    private int bulletsSpawnCounter;
    private GameObject[] bulletsSpawned;
    private bool inBulletSpawn;
    [SerializeField] private float bulletSpawnRate;
    [SerializeField] private int burstCount;

    [Header("Sentry Parts")]
    [SerializeField] private GameObject sentryHeart;
    [SerializeField] private GameObject stars;



    protected override void Start()
    {
        base.Start();
        bulletsSpawned = new GameObject[burstCount];
    }

    protected override void Fire()
    {
        if (!inBulletSpawn)
            SpawnBullets();
    }

    private void SpawnBullets()
    {
        if (!speared)
        {
            if (bulletsSpawnCounter < burstCount)
            {
                inBulletSpawn = true;

                Vector3 bulletSpawnLoc = transform.position + (Vector3.up * 2) + (transform.forward * (bulletsSpawnCounter - 1));
                bulletsSpawned[bulletsSpawnCounter] = Instantiate(shot, bulletSpawnLoc, Quaternion.identity);
                bulletsSpawnCounter++;

                Invoke(nameof(SpawnBullets), bulletSpawnRate);
            }
            else
            {
                ShootBullets();
            }
        }
    }

    private void ShootBullets()
    {
        if (!speared)
        {
            if (bulletsSpawnCounter != 0)
            {
                bulletsSpawned[bulletsSpawnCounter - 1].GetComponent<SentryShot>().Shoot(player.position, projectileSpeed, damage);
                bulletsSpawned[bulletsSpawnCounter - 1] = null;
                bulletsSpawnCounter--;
                Invoke(nameof(ShootBullets), fireRate);
            }
            else if (bulletsSpawnCounter == 0)
            {
                inBulletSpawn = false;
            }
        }


    }

    private void Update()
    {
        FindPlayer();


        if (inBulletSpawn)
        {
            foreach (var bullet in bulletsSpawned)
            {
                if (bullet != null)
                    bullet.transform.LookAt(player);
            }
        }
        
        stars.transform.LookAt(player.position);
    }

    public override bool EnemySpeared(Rigidbody spearRigidbody, Transform limbhit, float spearSpeed, Collision collision)
    {
        speared = true;

        foreach (GameObject joint in joints)
        {
            if (joint == sentryHeart)
            {
                joint.transform.SetParent(spearRigidbody.transform);
            }
            else
            {
                joint.GetComponent<Collider>().enabled = true;
                joint.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        foreach (GameObject bullet in bulletsSpawned)
        {
            if (bullet != null)
            {
                bullet.GetComponent<Collider>().isTrigger = false;
                var bulletRbbullet = bullet.GetComponent<Rigidbody>();
                bulletRbbullet.isKinematic = false;
                bulletRbbullet.useGravity = true;
                Destroy(bullet.GetComponent<SentryShot>());
            }
        }

        spearRigidbody.linearVelocity = spearSpeed * spearRigidbody.transform.forward;
        Killed();
        return false;
    }

    protected override void Killed()
    {
        base.Killed();
        Destroy(stars);
    }
}
