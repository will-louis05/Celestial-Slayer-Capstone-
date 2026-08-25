using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private enum HealthSystem { regenHealth, healthPoints };
    private float timeSinceLastDamaged;
    private bool hit;
    private float health;
    private Animator animator;

    [SerializeField] private HealthSystem healthSystem;

    [Header("Health Regen")]
    [SerializeField] private float maxPlayerHealth = 100;
    [SerializeField] private float timeTillHealthregenSec;
    [SerializeField] private float healthRegenPerSecond;

    [Header("Health Points")]
    [SerializeField] private int totalHitPoints;

    [Header("Dev Tools")]
    private Transform greenScalePoint;
    [SerializeField] private bool infiniteHealth;
    [SerializeField] private bool resetHealth;


    private void Start()
    {
        health = maxPlayerHealth;
        animator = GetComponent<Animator>();
        greenScalePoint = GameObject.Find("Scalepoint").transform;
    }
    private void Update()
    {
        // Debug.Log("Health: " + health);
        if (resetHealth)
        {
            health = maxPlayerHealth;
            resetHealth = false;
        }


        if(infiniteHealth)
            health = maxPlayerHealth;

        if(health < maxPlayerHealth)
        {
            if(healthSystem == HealthSystem.regenHealth) 
                RegenHealth();
            else if(healthSystem == HealthSystem.healthPoints)
                HealthPoints();
        }

        if (health <= 0)
            Die();

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

        greenScalePoint.localScale = new Vector3(health / 100f, 1, 1);
    }

    private void HealthPoints()
    {
        

    }

    public void Hit(float damage)
    {
        hit = true;
        health -= damage;
        animator.SetTrigger("Hit");
        greenScalePoint.localScale = new Vector3 (health / 100f, 1, 1);
    }

    private void Die()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }



}
