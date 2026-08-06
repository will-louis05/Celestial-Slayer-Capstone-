using Unity.Behavior;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private BehaviorGraphAgent behaviorGraph;
    private bool speared;
    private Rigidbody spearRb;
    private EnemySpawner spawner;

    [SerializeField] private float regainSpeed;

    void Start()
    {
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        //spawner = transform.parent.gameObject.GetComponent<EnemySpawner>();
    }

    void Update()
    {
        if (speared)
        {
            Debug.Log(spearRb.linearVelocity.magnitude);
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
        behaviorGraph.SetVariableValue("CanMove", !speared);
    }

    public void EnemyStuck()
    {
        Killed();
    }

    private void RegainControl()
    {
        speared = false;
        behaviorGraph.SetVariableValue("CanMove", !speared);
    }

    private void Killed()
    {
        //spawner.currentEnemyCount--;
        Debug.Log("enemyKilled");
        Destroy(this);

    }
}
