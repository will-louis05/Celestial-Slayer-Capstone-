using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private enum HealthSystem { regenHealth, healthPoints };
    private float timeSinceLastDamaged;
    private float health;
    private Animator animator;

    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private float mercyTime;

    [Header("Health Regen")]
    [SerializeField] private float maxPlayerHealth = 100;
    [SerializeField] private float timeTillHealthregenSec;
    [SerializeField] private float healthRegenPerSecond;

    [Header("Health Points")]
    [SerializeField] private int totalHitPoints;
    private Image healthBarFill;

    [Header("Dev Tools")]
    [SerializeField] private bool infiniteHealth;
    [SerializeField] private bool resetHealth;


    private void Start()
    {
        health = maxPlayerHealth;
        animator = GetComponent<Animator>();

        healthBarFill = GameObject.Find("BarFront").GetComponent<Image>();
        healthBarFill.fillAmount = Mathf.Clamp01(health / maxPlayerHealth);
    }

    private void Update()
    {
        // Debug.Log("Health: " + health);
        if (resetHealth)
        {
            health = maxPlayerHealth;
            resetHealth = false;

            healthBarFill.fillAmount = Mathf.Clamp01(health / maxPlayerHealth);
        }

        if (infiniteHealth)
        {
            health = maxPlayerHealth;

            healthBarFill.fillAmount = Mathf.Clamp01(health / maxPlayerHealth);
        }

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
        float damageTime = Time.time - timeSinceLastDamaged;

        if(damageTime >= timeTillHealthregenSec)
        {
            health += healthRegenPerSecond * Time.deltaTime;
        }

        if (health > maxPlayerHealth)
            health = maxPlayerHealth;

        healthBarFill.fillAmount = Mathf.Clamp01(health / maxPlayerHealth);
    }

    private void HealthPoints()
    {
        
    }

    public void Hit(float damage, bool isTicEffect)
    {
        if ((Time.time - timeSinceLastDamaged) < mercyTime && !isTicEffect)
            return;
        
        health -= damage;
        animator.SetTrigger("Hit");

        healthBarFill.fillAmount = Mathf.Clamp01(health / maxPlayerHealth);

        timeSinceLastDamaged = Time.time;
    }

    private void Die()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
