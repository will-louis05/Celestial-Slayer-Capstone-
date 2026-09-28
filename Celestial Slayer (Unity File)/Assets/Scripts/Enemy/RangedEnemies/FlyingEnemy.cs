using UnityEngine;


public class FlyingEnemy : RangedEnemy
{

    [SerializeField] private GameObject areaOfEffectAttack;

    [Header("AOE Values")]
    [SerializeField] private float damageTicTime;
    [SerializeField] private float timeTillDestory;

    private void Update()
    {
        if (!speared)
        {
            FindPlayer();
            transform.LookAt(player);
        }
    }

    protected override void Fire()
    {
        GameObject shotFired = Instantiate(shot, ((0.1f * transform.forward) + transform.position), Quaternion.identity);
        Collider enemyCollider = GetComponentInChildren<Collider>();
        shotFired.GetComponent<FlyerShot>().ShotSpawn(player.position, projectileSpeed, damage, damageTicTime, areaOfEffectAttack, timeTillDestory, enemyCollider);
    }
}
