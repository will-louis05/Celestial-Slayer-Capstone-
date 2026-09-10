using UnityEngine;

public class FlyerShot : MonoBehaviour
{
    private Vector3 playerPos;
    private float speed;
    private bool ballSpawned;
    private bool ballhit;
    private GameObject areaOfEffectAttack;
    private float damageOverTime;
    private float damageTicTime;
    private float timeTillDestory;
    [SerializeField] private GameObject areaOfEffectObj;


    public void ShotSpawn(Vector3 playerPosition, float ballSpeed, float damage, float ticTime, GameObject areaOfEfftect, float timeTillDes)
    {
        playerPos = playerPosition;
        speed = ballSpeed;
        transform.LookAt(playerPos);
        ballSpawned = true;
        areaOfEffectAttack = areaOfEfftect;
        damageOverTime = damage;
        damageTicTime = ticTime;
        timeTillDestory = timeTillDes;
    }

    void Update()
    {
        if (ballSpawned || !ballhit)
        {
            transform.position += transform.forward * speed;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        ObjHit(collision);
    }

    private void ObjHit(Collision collision)
    {
        GameObject areaOfEffect = Instantiate(areaOfEffectAttack, collision.contacts[0].point, Quaternion.identity);
        AreaOfEffectAttack areaScrpt = areaOfEffect.GetComponent<AreaOfEffectAttack>();
        areaScrpt.damgeOverTime = damageOverTime;
        areaScrpt.damgeTicTime = damageTicTime;
        areaScrpt.timeTillDestory = timeTillDestory;
        Destroy(gameObject);
    }
}
