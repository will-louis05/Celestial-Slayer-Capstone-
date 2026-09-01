using UnityEngine;

public class GizmoSpawnLocations : MonoBehaviour
{
    public enum EnemyType { basic, small, brute, flying, random}
    public EnemyType enemyType;
    private void OnDrawGizmos()
    {
        switch(enemyType)
        {
            case EnemyType.basic:
                Gizmos.color = Color.blue;
                break;
            case EnemyType.small:
                Gizmos.color = Color.green;
                break;
            case EnemyType.brute:
                Gizmos.color = Color.orange;
                break;
            case EnemyType.flying:
                Gizmos.color = Color.red;
                break;
            case EnemyType.random:
                Gizmos.color = Color.black;
                break;

        }
        Gizmos.DrawSphere(transform.position, 1);
    }
}
