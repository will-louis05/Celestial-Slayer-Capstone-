using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [SerializeField] private float fireRate;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private GameObject shot;
    [SerializeField] private GameObject areaOfEffectAttack;
    private Transform player;
    private float timeSinceLastFired;
    [Header("Damge Values")]
    [SerializeField] private float damageDelt;
    [SerializeField] private float damageTicTime;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float timeTillDestory;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }


    private void Update()
    {
        Vector3 directionFromPlayer = player.position - transform.position;
        if (distanceFromPlayer > directionFromPlayer.magnitude)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionFromPlayer, out hit))
            {
                Debug.DrawRay(transform.position, directionFromPlayer);
                if (hit.collider.CompareTag("Player"))
                {

                    if (fireRate < (Time.time - timeSinceLastFired))
                    {
                        Fire(hit.point);
                        timeSinceLastFired = Time.time;
                    }
                }
            }
        }
    }

    private void Fire(Vector3 playerLocation)
    {
        GameObject shotFired = Instantiate(shot, ((0.1f * transform.forward) + transform.position), Quaternion.identity);
        shotFired.GetComponent<FlyerShot>().ShotSpawn(playerLocation, projectileSpeed, damageDelt, damageTicTime, areaOfEffectAttack, timeTillDestory);
    }
}
