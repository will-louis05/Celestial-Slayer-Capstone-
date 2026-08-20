using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private BehaviorGraphAgent behaviorGraph;
    private bool speared;
    private Rigidbody spearRb;
    public EnemySpawner spawner;
    private NavMeshAgent navMesh;
    private float enemyMass;

    [SerializeField] private float regainSpeed;
    public float damage;
    [SerializeField] private MeshRenderer meshRender;

    [Header("SFX")]
    [SerializeField] private AudioSource deathSFX;

    void Start()
    {
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        navMesh = GetComponent<NavMeshAgent>();
        spearRb.mass = enemyMass;
    }

    void Update()
    {
        if (speared)
        {
            if (spearRb.linearVelocity.magnitude < regainSpeed)
            {
                RegainControl();
            }
        }
    }

    public void EnemySpeared(Rigidbody spearRigidbody)
    {
        meshRender.material.color = Color.yellow;
        spearRb = spearRigidbody;
        speared = true;
        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
        navMesh.enabled = false;
        behaviorGraph.enabled = false;
    }

    public void EnemyStuck()
    {
        Killed();
    }

    private void RegainControl()
    {
        meshRender.material.color = Color.blue;
        speared = false;
        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
    }

    private void Killed()
    {
        //Death SFX
        deathSFX.pitch = Random.Range(1.3f, 1.5f);
        deathSFX.Play();

        //Enable gravity on death
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        spawner.currentEnemyCount--;
        meshRender.material.color = Color.red;
        Destroy(this);
    }
}
