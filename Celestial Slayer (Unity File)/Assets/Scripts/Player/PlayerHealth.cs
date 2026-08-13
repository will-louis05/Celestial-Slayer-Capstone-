using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private enum HealthSystem { regenHealth, healthPoints };
    [Header("Health Stats")]
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private float maxPlayerHealth;
    public static float health;
    [SerializeField] float timeTillHealthregenSec;
    [SerializeField] float healthRegenPerSecond;

    void Die()
    {

    }


}
