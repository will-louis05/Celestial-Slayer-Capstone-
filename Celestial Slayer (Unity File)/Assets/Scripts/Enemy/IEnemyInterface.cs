using UnityEngine;

public interface IEnemyInterface
{
    public void ExplosionHit();

    public bool EnemySpeared(Rigidbody spearRigidbody, Transform limbhit, float spearSpeed, Collision collision);

    public void EnemyStuck()
    {
        Killed();
    }

    public void Killed();
}
