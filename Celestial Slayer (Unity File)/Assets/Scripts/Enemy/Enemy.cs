using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private BehaviorGraphAgent behaviorGraph;
    private bool speared;
    [SerializeField] private float enemyMass;
    private Rigidbody spearRb;
    public EnemySpawner spawner;
    private NavMeshAgent navMesh;
    private Animator animator;
    [SerializeField] private GameObject[] joints;
    public EnemyAttack attackScrpt;


    [SerializeField] private float regainSpeed;
    [SerializeField] private float spearSpeedDecreaseRatio;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    public float damage;

    [Header("SFX")]
    [SerializeField] private AudioSource deathSFX;

    void Start()
    {
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        navMesh = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("CanMove", !speared);

        skinnedMeshRenderer.material.color = Color.green;
        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void Update()
    {
        

    }

    public bool EnemySpeared(Rigidbody spearRigidbody, Transform limbhit, float spearSpeed, Collision collision)
    {
        float inveseForce = enemyMass * spearSpeedDecreaseRatio;

        float postCollisionSpeed = spearSpeed - inveseForce;

        if (postCollisionSpeed < 0)
        {
            skinnedMeshRenderer.material.color = Color.blue;
            return true;
        }

        spearRb = spearRigidbody;
        speared = true;

        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
        animator.enabled = false;

        navMesh.enabled = false;
        behaviorGraph.enabled = false;

        skinnedMeshRenderer.material.color = Color.yellow;
        int spearIgnoreLayer = LayerMask.NameToLayer("SpearIgnore");
        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = false;
            joint.layer = spearIgnoreLayer;
        }

        FixedJoint limbFixedJoint = limbhit.AddComponent<FixedJoint>();
        limbFixedJoint.connectedBody = spearRb.transform.GetComponent<Rigidbody>();

        Debug.Log(postCollisionSpeed);
        spearRb.linearVelocity = postCollisionSpeed * spearRb.transform.forward;

        return false;
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
        skinnedMeshRenderer.material.color = Color.black;
    }

    private void Killed()
    {
        //Death SFX
        deathSFX.pitch = Random.Range(1.3f, 1.5f);
        deathSFX.Play();

        //Make Them SpearableAgain
        int spearIgnoreLayer = LayerMask.NameToLayer("SpearIgnore");
        foreach (var joint in joints)
        {
            joint.layer = spearIgnoreLayer;
        }

        skinnedMeshRenderer.material.color = Color.red;
        spawner.currentEnemyCount--;
        Destroy(this);
        Destroy(attackScrpt);
    }
}
