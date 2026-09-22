using UnityEngine;

public class RangedEnemy : Enemy
{
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
}
