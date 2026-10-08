using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected bool speared;
    protected Animator animator;

    [SerializeField] protected GameObject[] joints;
    public EnemySpawner spawner;

    [Header("Enemy Attributes")]
    [SerializeField] protected float enemyMass;
    [SerializeField] protected float forceRequiredToSpear;
    [SerializeField] private float regainSpeed;
    [SerializeField] protected float spearSpeedDecreaseRatio;
    public float damage;

    public bool dead = false;
    //[SerializeField] private bool enemyDisable;
    //private bool inEnabled;
    //[SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("SFX")]
    [SerializeField] private AudioSource enemySFX;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();

        ////DEBUG TOOL
        //skinnedMeshRenderer.material.color = Color.green;
        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = true;
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

    public virtual bool EnemySpeared(Rigidbody spearRigidbody, Transform limbhit, float spearSpeed, Collision collision)
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

        speared = true;
        animator.enabled = false;

        ////DEBUG TOOL
        //skinnedMeshRenderer.material.color = Color.yellow;
        int spearIgnoreLayer = LayerMask.NameToLayer("SpearIgnore");
        foreach (var joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = false;
            joint.layer = spearIgnoreLayer;
        }

        FixedJoint limbFixedJoint = limbhit.gameObject.AddComponent<FixedJoint>();
        limbFixedJoint.connectedBody = spearRigidbody.transform.GetComponent<Rigidbody>();

        spearRigidbody.linearVelocity = postCollisionSpeed * spearRigidbody.transform.forward;

        //Bugfix apply velocity to both spear and enemy
        //Vector3 velocity = postCollisionSpeed * spearRb.transform.forward;
        //spearRb.linearVelocity = velocity;
        //enemyRb.isKinematic = false;
        //enemyRb.linearVelocity = velocity;

        Invoke(nameof(EnemyStuck), 60f);

        return false;
    }

    public  void EnemyStuck()
    {
        Killed();
    }


    protected virtual void Killed()
    {
        //Make Them SpearableAgain
        foreach (var joint in joints)
        {
            joint.layer = 0;
        }
        ////Debug TOOL
        //skinnedMeshRenderer.material.color = Color.red;

        //Prevent double kill
        if (!dead)
        {
            spawner.currentEnemyCount--;
            dead = true;
        }

        Destroy(animator);
        Destroy(this);
    }
}
