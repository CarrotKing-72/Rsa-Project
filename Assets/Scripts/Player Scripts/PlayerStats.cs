using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHealth = 100f;
    public float health = 100f;
    public float temperature = 37f; // Normal body temp
    public float minTemperature = 30f;
    public float maxTemperature = 45f;
    private float heatTimer = 0f;
    [Header("UI References")]
    public Slider healthSlider;
    public Slider tempSlider;

    [Header("Death Settings")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float deathAnimationDelay = 1.25f;

    private bool deathHandled = false;

    public event Action<float> OnHealthChanged;
    public event Action<float> OnTemperatureChanged;

    private void Start()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            if (playerAnimator == null)
            {
                playerAnimator = GetComponentInChildren<Animator>();
            }
        }

        // Subscribe UI update methods to events
        OnHealthChanged += UpdateHealthUI;
        OnTemperatureChanged += UpdateTemperatureUI;

        // Initialize sliders at start
        UpdateHealthUI(health);
        UpdateTemperatureUI(temperature);
    }

    public void ChangeHealth(float amount)
    {
        if (deathHandled)
        {
            return;
        }

        health = Mathf.Clamp(health + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(health);

        if (health <= 0)
        {
            HandleDeath();
        }



    }

    public void ChangeTemperature(float amount)
    {
        temperature = Mathf.Clamp(temperature + amount, minTemperature, maxTemperature);
        OnTemperatureChanged?.Invoke(temperature);
    }
    public void ResetStats()
    {
        deathHandled = false;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsDead", false);
        }

        // Reset Health
        health = maxHealth;
        OnHealthChanged?.Invoke(health);

        // Reset Temperature to normal (37 degrees)
        temperature = 37f;
        OnTemperatureChanged?.Invoke(temperature);

        // CRITICAL: Reset the exponential heat timer so damage starts at base level again
        heatTimer = 0f;

        Debug.Log("Player Stats Reset for New Level");
    }
    private void HandleDeath()
    {
        if (deathHandled)
        {
            return;
        }

        deathHandled = true;
        Debug.Log("Player Died! Returning to Hub...");

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsDead", true);
        }

        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        yield return new WaitForSeconds(deathAnimationDelay);

        SeedInventory inventory = GetComponent<SeedInventory>();
        if (inventory != null)
        {
            inventory.ResetSeeds();
        }

        SceneManager.LoadScene("Hub");
    }
    
    void Update()
    {
        // If temperature is too high, lose health
        if (temperature > 41f)
        {
            heatTimer += Time.deltaTime;
            // Damage starts at 2 and increases by 4 every second you stay hot
            float damagePerSecond = 2f + (heatTimer * 1.5f);
            ChangeHealth(-Time.deltaTime * damagePerSecond);
        }
        else
        {
            heatTimer = 0f; // Reset danger when you reach a tree
        }
    }



    // UI update methods
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
   