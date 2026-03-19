using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public Slider healthSlider;
    public Slider temperatureSlider;
    public Text healthText;
    public Text temperatureText;

    [Obsolete]
    private void Start()
    {
        if (!playerStats)
            playerStats = FindObjectOfType<PlayerStats>();

        // Initialize UI
        UpdateHealthUI(playerStats.health);
        UpdateTemperatureUI(playerStats.temperature);

        // Subscribe to events (make sure PlayerStats has these events)
        playerStats.OnHealthChanged += UpdateHealthUI;
        playerStats.OnTemperatureChanged += UpdateTemperatureUI;
    }

    private void UpdateHealthUI(float health)
    {
        if (healthSlider) healthSlider.value = health;
        if (healthText) healthText.text = $"Health: {health:0}";
    }

    private void UpdateTemperatureUI(float temp)
    {
        if (temperatureSlider) temperatureSlider.value = temp;
        if (temperatureText) temperatureText.text = $"Temp: {temp:0.0}°C";
    }
}

