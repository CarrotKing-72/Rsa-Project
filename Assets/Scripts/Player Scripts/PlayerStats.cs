using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    
    [Header("Player Stats")]
    public float maxHealth      = 100f;
    public float health         = 100f;
    public float temperature    = 37f;   // Normal body temp in Celsius
    public float minTemperature = 30f;
    public float maxTemperature = 45f;

    
    [Header("Heat Damage Settings")]
    [Tooltip("Temperature must exceed this value before damage starts.")]
    public float damageTempThreshold = 40f;

    [Tooltip("Flat damage per second the moment you enter the danger zone.")]
    public float baseDamagePerSecond = 8f;

    [Tooltip("Extra damage added per second of continuous heat exposure.")]
    public float heatRampPerSecond   = 4f;

    [Tooltip("Maximum damage per second so it never becomes instant death.")]
    public float maxDamagePerSecond  = 25f;

    [Tooltip("Temperature must drop THIS far below the threshold before damage stops. Prevents flickering.")]
    public float cooldownTempBuffer  = 0.5f;

    
    [Header("UI References")]
    public UnityEngine.UI.Slider healthSlider;
    public UnityEngine.UI.Slider tempSlider;

    
    [Header("Death Settings")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float deathAnimationDelay = 1.25f;

    
    public event Action<float> OnHealthChanged;
    public event Action<float> OnTemperatureChanged;

    
    private float _heatTimer         = 0f;
    private bool  _isTakingHeatDamage = false;
    private bool  _deathHandled      = false;

    
    private void Start()
    {
        
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();

        
        OnHealthChanged      += UpdateHealthUI;
        OnTemperatureChanged += UpdateTemperatureUI;

        
        UpdateHealthUI(health);
        UpdateTemperatureUI(temperature);
    }

    private void Update()
    {
        HandleHeatDamage();
    }

    
    public void ChangeHealth(float amount)
    {
        if (_deathHandled) return;

        health = Mathf.Clamp(health + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(health);

        if (health <= 0)
            HandleDeath();
    }

    
    public void ChangeTemperature(float amount)
    {
        temperature = Mathf.Clamp(temperature + amount, minTemperature, maxTemperature);
        OnTemperatureChanged?.Invoke(temperature);
    }

    
    public void ResetStats()
    {
        _deathHandled       = false;
        _heatTimer          = 0f;
        _isTakingHeatDamage = false;

        if (playerAnimator != null)
            playerAnimator.SetBool("IsDead", false);

        health = maxHealth;
        OnHealthChanged?.Invoke(health);

        temperature = 37f;
        OnTemperatureChanged?.Invoke(temperature);

        Debug.Log("[PlayerStats] Stats reset.");
    }

    
    private void HandleHeatDamage()
    {
        

        if (_isTakingHeatDamage)
        {
           
            if (temperature < damageTempThreshold - cooldownTempBuffer)
            {
                _isTakingHeatDamage = false;
                _heatTimer          = 0f;
                Debug.Log("[PlayerStats] Cooled down. Heat damage stopped.");
            }
        }
        else
        {
            
            if (temperature > damageTempThreshold)
            {
                _isTakingHeatDamage = true;
                Debug.Log("[PlayerStats] Overheating! Heat damage started.");
            }
        }

        // --- Apply damage if we are in the damage state ---

        if (!_isTakingHeatDamage) return;

        _heatTimer += Time.deltaTime;

        // Damage ramps up the longer you stay hot, but is capped so it stays fair
        float damagePerSecond = Mathf.Min(
            baseDamagePerSecond + (_heatTimer * heatRampPerSecond),
            maxDamagePerSecond
        );

        ChangeHealth(-Time.deltaTime * damagePerSecond);
    }

    
    private void HandleDeath()
    {
        if (_deathHandled) return;
        _deathHandled = true;

        Debug.Log("[PlayerStats] Player died. Returning to Hub...");

        if (playerAnimator != null)
            playerAnimator.SetBool("IsDead", true);

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathAnimationDelay);

        // Drop seeds on death so runs feel punishing but recoverable
        SeedInventory inventory = GetComponent<SeedInventory>();
        if (inventory != null)
            inventory.ResetSeeds();

        SceneManager.LoadScene("Hub");
    }

    
    private void UpdateHealthUI(float currentHealth)
    {
        if (healthSlider)
            healthSlider.value = currentHealth / maxHealth;
    }

    private void UpdateTemperatureUI(float currentTemp)
    {
        if (tempSlider)
            tempSlider.value = (currentTemp - minTemperature) / (maxTemperature - minTemperature);
    }
}