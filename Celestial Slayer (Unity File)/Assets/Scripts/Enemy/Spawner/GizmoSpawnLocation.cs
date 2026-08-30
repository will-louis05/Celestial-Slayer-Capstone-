using UnityEngine;

public class GizmoSpawnLocations : MonoBehaviour
{
    public enum EnemyType { basic, small, big, flying, random}
    public EnemyType enemyType;
    public int enemyTypeInt;
    private void OnDrawGizmos()
    {
        switch(enemyType)
        {
            case EnemyType.basic:
                Gizmos.color = Color.blue;
                enemyTypeInt = 0;
                break;
            case EnemyType.small:
                Gizmos.color = Color.green;
                enemyTypeInt = 1;
                break;
            case EnemyType.big:
                Gizmos.color = Color.orange;
                enemyTypeInt = 2;
                break;
            case EnemyType.flying:
                Gizmos.color = Color.red;
                enemyTypeInt = 3;
                break;
            case EnemyType.random:
                Gizmos.color = Color.black;
                enemyTypeInt = 4;
                break;

        }
        Gizmos.DrawSphere(transform.position, 1);
    }
}
