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
    private Rigidbody rb;
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
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (ballSpawned || !ballhit)
        {
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        }
            
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("HitObj");
        if (collider != self)
        {
            ObjHit();
        }
    }

    private void ObjHit()
    {
        GameObject areaOfEffect = Instantiate(areaOfEffectAttack, transform.position, Quaternion.identity);
        AreaOfEffectAttack areaScrpt = areaOfEffect.GetComponent<AreaOfEffectAttack>();
        areaScrpt.damgeOverTime = damageOverTime;
        areaScrpt.damgeTicTime = damageTicTime;
        areaScrpt.timeTillDestory = timeTillDestory;
        Destroy(gameObject);
    }
}
