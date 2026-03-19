using UnityEngine;
using UnityEngine.UI;

public class HeatVignette : MonoBehaviour
{
    [Header("References")]
    public Image vignetteImage;
    public PlayerStats playerStats;

    [Header("Settings")]
    public float dangerThreshold = 41f;
    public float maxAlpha = 0.6f;
    public float pulseSpeed = 2f;

    private void Update()
    {
        if (playerStats == null || vignetteImage == null) return;

        if (playerStats.temperature >= dangerThreshold)
        {
            // Normalise how deep into danger zone we are (41 to 45)
            float dangerPercent = (playerStats.temperature - dangerThreshold) /
                                  (playerStats.maxTemperature - dangerThreshold);

            // Pulse the alpha using a sine wave — creates a heartbeat feel
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            float alpha = Mathf.Lerp(0.1f, maxAlpha, dangerPercent) * pulse;

            vignetteImage.color = new Color(1f, 0f, 0f, alpha);
        }
        else
        {
            // Fade out smoothly when safe
            Color c = vignetteImage.color;
            c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime);
            vignetteImage.color = c;
        }
    }
}

