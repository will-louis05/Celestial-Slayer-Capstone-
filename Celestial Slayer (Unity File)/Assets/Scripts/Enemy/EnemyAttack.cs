using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Enemy enemyParent;
    private float damage;

    private void Start()
    {
        Transform parent = transform.root;
        enemyParent = parent.GetComponent<Enemy>();
        damage = enemyParent.damage;
        enemyParent.attackScrpt = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.Hit(damage);
        }
    }
}
