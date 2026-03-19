using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SeedInventory : MonoBehaviour
{
    public int seedCount = 0;
    public TMP_Text seedText; // Assign a UI Text object here in the Inspector

    private void Start()
    {
        UpdateUI();
    }

    public void AddSeed(int amount)
    {
        seedCount += amount;
        UpdateUI();
    }

    public void ResetSeeds()
    {
        seedCount = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (seedText != null)
        {
            seedText.text = "Seeds: " + seedCount.ToString();
        }
    }
}