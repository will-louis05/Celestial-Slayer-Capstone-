using Unity.Behavior;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private BehaviorGraphAgent behaviorGraph;
    private bool speared;
    private Rigidbody spearRb;
    private EnemySpawner spawner;

    void Start()
    {
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        spawner = transform.parent.gameObject.GetComponent<EnemySpawner>();
    }

    void Update()
    {
        if (speared && spearRb.linearVelocity.magnitude < 10)
        {
            RegainControl();
        }
    }

    public void EnemySpeared(out bool pierce, Rigidbody spearRb)
    {
        speared = true;
        pierce = true;

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
        spawner.currentEnemyCount--;
        Destroy(gameObject);

    }
}
