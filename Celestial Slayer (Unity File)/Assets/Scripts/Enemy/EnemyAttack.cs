using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Enemy enemyParent;
    private float damage;

    private void Start()
    {
        damage = enemyParent.damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.name);
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            playerHealth.Hit(damage);
        }
    }
}
