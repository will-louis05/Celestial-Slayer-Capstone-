using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : Enemy
{
    protected BehaviorGraphAgent behaviorGraph;
    protected NavMeshAgent navMesh;
    [SerializeField] protected float runSpeed;
    public EnemyAttack attackScrpt;
    private Rigidbody spearRb;


    protected override void Start()
    {
        base.Start();

        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        navMesh = GetComponent<NavMeshAgent>();

        behaviorGraph.BlackboardReference.SetVariableValue("Speed", runSpeed);

        animator.SetBool("CanMove", !speared);
    }

    public override bool EnemySpeared(Rigidbody spearRigidbody, Transform limbhit, float spearSpeed, Collision collision)
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

        //enemyDisable = false;
    }

    protected override void Killed()
    {
        Destroy(attackScrpt);
        Destroy(navMesh);
        Destroy(behaviorGraph);
        base.Killed();
 
    }
}
