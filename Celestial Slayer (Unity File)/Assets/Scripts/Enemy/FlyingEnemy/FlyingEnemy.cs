using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;


public class FlyingEnemy : MonoBehaviour, IEnemyInterface
{
    [SerializeField] private float fireRate;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private GameObject shot;
    [SerializeField] private GameObject areaOfEffectAttack;
    [SerializeField] private GameObject[] joints;
    [SerializeField] private float enemyMass;
    [SerializeField] private float spearSpeedDecreaseRatio;

    private Rigidbody enemyRb;
    private Transform player;
    private float timeSinceLastFired;
    private bool speared;
    public EnemySpawner spawner;

    [Header("Damge Values")]
    [SerializeField] private float damageDelt;
    [SerializeField] private float damageTicTime;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float timeTillDestory;

    private void Start()
    {
        player = GameObject.Find("Player").transform;

        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = true;
        }
    }


    private void Update()
    {
        if (!speared)
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
            transform.LookAt(player.position);
        }
    }

    private void Fire(Vector3 playerLocation)
    {
        GameObject shotFired = Instantiate(shot, ((0.1f * transform.forward) + transform.position), Quaternion.identity);
        Collider enemyCollider = GetComponentInChildren<Collider>();
        shotFired.GetComponent<FlyerShot>().ShotSpawn(playerLocation, projectileSpeed, damageDelt, damageTicTime, areaOfEffectAttack, timeTillDestory, this.GetComponent<Collider>());
    }

    public void ExplosionHit()
    {
        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = false;
        }
        Killed();
    }

    public bool EnemySpeared(Rigidbody spearRigidbody, Transform limbhit, float spearSpeed, Collision collision)
    {

        //float inveseForce = enemyMass * spearSpeedDecreaseRatio;

        float postCollisionSpeed = spearSpeed;

        speared = true;

        transform.GetComponent<Collider>().enabled = false;
        transform.SetParent(spearRigidbody.transform);

        spearRigidbody.linearVelocity = postCollisionSpeed * spearRigidbody.transform.forward;
        spawner.currentEnemyCount--;
        Destroy(gameObject);
        return false;
    }

    public void Killed()
    {
        //Make Them SpearableAgain
        int spearIgnoreLayer = LayerMask.NameToLayer("SpearIgnore");
        foreach (var joint in joints)
        {
            joint.layer = spearIgnoreLayer;
        }
        ////Debug TOOL
        //skinnedMeshRenderer.material.color = Color.red;
        //spawner.currentEnemyCount--;
        Destroy(this);
    }
}
