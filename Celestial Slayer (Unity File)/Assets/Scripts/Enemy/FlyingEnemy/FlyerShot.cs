using UnityEngine;

public class FlyerShot : MonoBehaviour
{
    private Vector3 playerPos;
    public float speed;
    private bool ballSpawned;
    private bool ballhit;
    private GameObject areaOfEffectAttack;
    public float damgeOverTime;
    public float damgeTicTime;
    [SerializeField] private GameObject areaOfEffectObj;


    public void ShotSpawn(Vector3 playerPosition, float ballSpeed, float damge, float ticTime)
    {
        playerPos = playerPosition;
        speed = ballSpeed;
        transform.LookAt(playerPos);
        ballSpawned = true;
        damgeOverTime = damge;
        damgeTicTime = ticTime;
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
        areaScrpt.damgeOverTime = damgeOverTime;
        areaScrpt.damgeTicTime = damgeTicTime;
    }
}
