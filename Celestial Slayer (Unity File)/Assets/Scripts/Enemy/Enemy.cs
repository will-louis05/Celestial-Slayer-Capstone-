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
    private Animator animator;
    private float enemyMass;

    [SerializeField] private float regainSpeed;
    public float damage;

    [Header("SFX")]
    [SerializeField] private AudioSource deathSFX;

    void Start()
    {
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        navMesh = GetComponent<NavMeshAgent>();
        spearRb = gameObject.GetComponent<Rigidbody>();
        spearRb.mass = enemyMass;
        animator = GetComponent<Animator>();
        animator.SetBool("CanMove", !speared);
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
        spearRb = spearRigidbody;
        speared = true;
        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
        animator.SetBool("CanMove", !speared);
        navMesh.enabled = false;
        behaviorGraph.enabled = false;
    }

    public void EnemyStuck()
    {
        Killed();
    }

    private void RegainControl()
    {
        speared = false;
        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
        animator.SetBool("CanMove", !speared);
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
        Destroy(this);
    }
}
