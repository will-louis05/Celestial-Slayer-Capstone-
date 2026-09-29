using UnityEngine;

public class GizmoSpawnLocations : MonoBehaviour
{
    public enum EnemyType { basic, brute, flying, sentry,random }
    public EnemyType enemyType;
    public float spawnTime;
    private void OnDrawGizmos()
    {
        switch(enemyType)
        {
            case EnemyType.basic:
                Gizmos.color = Color.blue;
                break;
            case EnemyType.sentry:
                Gizmos.color = Color.green;
                break;
            case EnemyType.brute:
                Gizmos.color = Color.orange;
                break;
            case EnemyType.flying:
                Gizmos.color = Color.purple;
                break;
            case EnemyType.random:
                Gizmos.color = Color.black;
                break;

        }
        Gizmos.DrawSphere(transform.position, 1);
    }
}
