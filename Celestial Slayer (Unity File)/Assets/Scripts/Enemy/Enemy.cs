using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private BehaviorGraphAgent behaviorGraph;
    private bool speared;
    [SerializeField] private float enemyMass;
    [SerializeField] private float forceRequiredToSpear;
    [SerializeField] private float runSpeed;
    private Rigidbody enemyRb;
    private Rigidbody spearRb;
    public EnemySpawner spawner;
    private NavMeshAgent navMesh;
    private Animator animator;
    [SerializeField] private GameObject[] joints;
    public EnemyAttack attackScrpt;

    [SerializeField] private float regainSpeed;
    [SerializeField] private float spearSpeedDecreaseRatio;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    public bool isBigEnemy;
    public float damage;

    private FixedJoint jointOnSpear;
    [SerializeField] private bool enemyDisable;
    private bool inEnabled;

    [Header("SFX")]
    [SerializeField] private AudioSource enemySFX;

    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        navMesh = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("CanMove", !speared);

        behaviorGraph.BlackboardReference.SetVariableValue("Speed", runSpeed);
        ////DEBUG TOOL
        //skinnedMeshRenderer.material.color = Color.green;
        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void Update()
    {
        if (enemyDisable)
        {
            RegainControl();
        }
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
        float inveseForce = enemyMass * spearSpeedDecreaseRatio;

        float postCollisionSpeed = spearSpeed - inveseForce;

        //DEBUG TOOL
        //Debug.Log("Post colSpeed = " + postCollisionSpeed);
        if (postCollisionSpeed < forceRequiredToSpear)
        {
            //skinnedMeshRenderer.material.color = Color.blue;
            return true;
        }

        spearRb = spearRigidbody;
        speared = true;

        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
        animator.enabled = false;

        navMesh.enabled = false;
        behaviorGraph.enabled = false;

        ////DEBUG TOOL
        //skinnedMeshRenderer.material.color = Color.yellow;
        int spearIgnoreLayer = LayerMask.NameToLayer("SpearIgnore");
        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = false;
            joint.layer = spearIgnoreLayer;
        }

        FixedJoint limbFixedJoint = limbhit.gameObject.AddComponent<FixedJoint>();
        limbFixedJoint.connectedBody = spearRb.transform.GetComponent<Rigidbody>();

        spearRb.linearVelocity = postCollisionSpeed * spearRb.transform.forward;

        //Bugfix apply velocity to both spear and enemy
        //Vector3 velocity = postCollisionSpeed * spearRb.transform.forward;
        //spearRb.linearVelocity = velocity;
        //enemyRb.isKinematic = false;
        //enemyRb.linearVelocity = velocity;

        attackScrpt.enabled = false;

        return false;
    }

    public void EnemyStuck()
    {
        Killed();
    }

    private void RegainControl()
    {
        //attackScrpt.enabled = true;

        //jointOnSpear = null;
        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = true;
            joint.layer = 0;
        }

        speared = false;


        animator.enabled = true;
        navMesh.enabled = true;
        behaviorGraph.enabled = true;

        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
        animator.SetBool("CanMove", !speared);
        ////DEBUG TOOL
        //skinnedMeshRenderer.material.color = Color.black;
    }

    private void DisableEnemy()
    {
        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
        animator.enabled = false;

        navMesh.enabled = false;
        behaviorGraph.enabled = false;

        ////DEBUG TOOL
        //skinnedMeshRenderer.material.color = Color.yellow;
        int spearIgnoreLayer = LayerMask.NameToLayer("SpearIgnore");
        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = false;
            joint.layer = spearIgnoreLayer;
        }

        enemyDisable = false;
    }

    private void Killed()
    {
        //Make Them SpearableAgain
        foreach (var joint in joints)
        {
            joint.layer = 0;
        }
        ////Debug TOOL
        //skinnedMeshRenderer.material.color = Color.red;
        spawner.currentEnemyCount--;
        Destroy(navMesh);
        Destroy(behaviorGraph);
        Destroy(animator);
        Destroy(attackScrpt);
        Destroy(this);
    }
}
