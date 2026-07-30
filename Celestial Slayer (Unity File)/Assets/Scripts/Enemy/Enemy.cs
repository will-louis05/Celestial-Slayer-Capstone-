using Unity.Behavior;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private BehaviorGraphAgent behaviorGraph;
    private bool speared;
    private Rigidbody spearRb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
    }

    // Update is called once per frame
    void Update()
    {


        if (speared && spearRb.linearVelocity.magnitude < 10)
        {
            
        }
    }

    public void EnemySpeared(out bool pierce, out bool stuck, Rigidbody spearRb)
    {
        speared = true;
        pierce = true;
        stuck = false;

        Killed();

        //behaviorGraph.SetVariableValue("CanMove", speared);
    }

    public void EnemyStuck()
    {
        behaviorGraph.SetVariableValue("CanMove", false);
        Destroy(this);
    }

    private void SpearBreak()
    {

    }

    private void Killed()
    {
        EnemySpawner enemySpawner = transform.parent.gameObject.GetComponent<EnemySpawner>();
        enemySpawner.currentEnemyCount--;
        Destroy(gameObject);

    }
}
