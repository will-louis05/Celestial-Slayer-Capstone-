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
    private float timer = 0.01f;

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
        if (speared)
        {
            //if (spearRb.linearVelocity.magnitude < regainSpeed)
            //{
            //    RegainControl();
            //}
        }
    }

    public bool EnemySpeared(Rigidbody spearRigidbody, Transform limbhit, float spearSpeed)
    {
        Debug.Log("spearSpeed = " + spearSpeed);
        float inveseForce = enemyMass * spearSpeedDecreaseRatio;
        //BUGFIX if hit immediately auto set speed
        if (Time.deltaTime < timer)
            spearSpeed = 90f;
        Debug.Log("spearSpeed postFix = " + spearSpeed);

        float postCollisionSpeed = spearSpeed - inveseForce;

        if (postCollisionSpeed < 0)
        {
            Debug.Log("SpearTooSlow");
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

        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = false;
        }

        FixedJoint spearFixedJoint = spearRigidbody.transform.AddComponent<FixedJoint>();
        spearFixedJoint.connectedBody = limbhit.GetComponent<Rigidbody>();

        spearRb.linearVelocity = postCollisionSpeed * transform.forward;

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

        //Enable gravity on death
        animator.enabled = false;
        skinnedMeshRenderer.material.color = Color.red;
        spawner.currentEnemyCount--;
        Debug.Log("enemyKilled");
        Destroy(this);
        Destroy(attackScrpt);
    }
}
