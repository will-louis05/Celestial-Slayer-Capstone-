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
    private Collider self;
    [SerializeField] private GameObject areaOfEffectObj;


    public void ShotSpawn(Vector3 playerPosition, float ballSpeed, float damage, float ticTime, GameObject areaOfEfftect, float timeTillDes, Collider shooter)
    {
        playerPos = playerPosition;
        speed = ballSpeed;
        transform.LookAt(playerPos);
        ballSpawned = true;
        areaOfEffectAttack = areaOfEfftect;
        damageOverTime = damage;
        damageTicTime = ticTime;
        timeTillDestory = timeTillDes;
        self = shooter;
    }

    void Update()
    {
        if (ballSpawned || !ballhit)
        {
            transform.position += transform.forward * speed;
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision != self)
        {
            ObjHit();
        }
    }

    private void ObjHit()
    {
        GameObject areaOfEffect = Instantiate(areaOfEffectAttack, transform.position, Quaternion.identity);
        Debug.Log(areaOfEffect);    
        AreaOfEffectAttack areaScrpt = areaOfEffect.GetComponent<AreaOfEffectAttack>();
        areaScrpt.damgeOverTime = damageOverTime;
        areaScrpt.damgeTicTime = damageTicTime;
        areaScrpt.timeTillDestory = timeTillDestory;
        Destroy(gameObject);
    }
}
