using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private enum HealthSystem { regenHealth, healthPoints };
    private float timeSinceLastDamaged;
    private bool hit;
    private float health;
    private Animator animator;

    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private bool resetHealth;

    [Header("Health Regen")]
    [SerializeField] private float maxPlayerHealth = 100;
    [SerializeField] private float timeTillHealthregenSec;
    [SerializeField] private float healthRegenPerSecond;

    [Header("Health Points")]
    [SerializeField] private int totalHitPoints;


    private void Start()
    {
        health = maxPlayerHealth;
    }
    private void Update()
    {
       // Debug.Log("Health: " + health);
        if(resetHealth)
            health = maxPlayerHealth;

        if(health < maxPlayerHealth)
        {
            if(healthSystem == HealthSystem.regenHealth) 
                RegenHealth();
            else if(healthSystem == HealthSystem.healthPoints)
                HealthPoints();
        }

    }

    private void RegenHealth()
    {
        if(hit)
        {
            timeSinceLastDamaged = Time.time;
            hit = false;
        }

        float damageTime = Time.time - timeSinceLastDamaged;

        if(damageTime >= timeTillHealthregenSec)
        {
            health += healthRegenPerSecond * Time.deltaTime;
        }

        if (health > maxPlayerHealth)
            health = maxPlayerHealth;
    }

    private void HealthPoints()
    {
        

    }

    public void Hit(float damage)
    {
        hit = true;
        health -= damage;
        animator.SetTrigger("Hit");
    }




}
