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
            
            transform.LookAt(player.position);
        }
    }

    protected override void Fire(Vector3 playerLocation)
    {
        GameObject shotFired = Instantiate(shot, ((0.1f * transform.forward) + transform.position), Quaternion.identity);
        Collider enemyCollider = GetComponentInChildren<Collider>();
        shotFired.GetComponent<FlyerShot>().ShotSpawn(playerLocation, projectileSpeed, damageDelt, damageTicTime, areaOfEffectAttack, timeTillDestory, enemyCollider);
    }
}
