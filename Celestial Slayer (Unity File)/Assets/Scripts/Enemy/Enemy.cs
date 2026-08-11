using Unity.Behavior;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private BehaviorGraphAgent behaviorGraph;
    private bool speared;
    private Rigidbody spearRb;
    private EnemySpawner spawner;

    [SerializeField] private float regainSpeed;
    [SerializeField] private MeshRenderer meshRender;

    void Start()
    {
        behaviorGraph = GetComponent<BehaviorGraphAgent>();
        spawner = transform.parent.gameObject.GetComponent<EnemySpawner>();
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
        meshRender.material.color = Color.yellow;
        spearRb = spearRigidbody;
        speared = true;
        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
    }

    public void EnemyStuck()
    {
        Killed();
    }

    private void RegainControl()
    {
        meshRender.material.color = Color.blue;
        speared = false;
        behaviorGraph.BlackboardReference.SetVariableValue("CanMove", !speared);
    }

    private void Killed()
    {
        spawner.currentEnemyCount--;
        meshRender.material.color = Color.red;
        Destroy(this);
    }
}
