using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected bool speared;
    protected Animator animator;

    [SerializeField] protected float enemyMass;
    [SerializeField] protected float forceRequiredToSpear;
    [SerializeField] protected GameObject[] joints;
    public EnemySpawner spawner;

    [SerializeField] private float regainSpeed;
    [SerializeField] protected float spearSpeedDecreaseRatio;
    public float damage;

    private bool dead = false;

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

    public abstract bool EnemySpeared(Rigidbody spearRigidbody, Transform limbhit, float spearSpeed, Collision collision);

    public void EnemyStuck()
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
