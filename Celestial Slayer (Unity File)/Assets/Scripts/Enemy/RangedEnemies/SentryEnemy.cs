using UnityEngine;

public class SentryEnemy : RangedEnemy
{
    private int bulletsSpawnCounter;
    [SerializeField] private GameObject bullet;
    private GameObject[] bulletsSpawn;

    protected override void Fire(Vector3 playerPos)
    {
        
    }

    private void SpawnBullets()
    {
        if (bulletsSpawnCounter < 3)
        {
            Instantiate(bullet, transform.position + Vector3.up * 2, Quaternion.identity);
        }

    }
}
