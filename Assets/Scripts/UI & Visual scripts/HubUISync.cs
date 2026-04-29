using UnityEngine;
using TMPro;

public class HubUISync : MonoBehaviour
{
    public TMP_Text bankedSeedsText;

    private void OnEnable()
    {
        // Subscribe whenever this object becomes active
        if (GameManager.Instance != null)
            GameManager.Instance.OnBankUpdated += UpdateBankDisplay;

        UpdateBankDisplay(); // also read current value immediately
    }

    private void OnDisable()
    {
        // Always unsubscribe to avoid memory leaks
        if (GameManager.Instance != null)
            GameManager.Instance.OnBankUpdated -= UpdateBankDisplay;
    }

    public void UpdateBankDisplay()
    {
        if (GameManager.Instance != null && bankedSeedsText != null)
            bankedSeedsText.text = "Banked Seeds: " + GameManager.Instance.totalSeedsInBank;
    }
}