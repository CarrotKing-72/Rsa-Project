using UnityEngine;
using TMPro;

public class HubUISync : MonoBehaviour
{
    public TMP_Text bankedSeedsText;

    private void Start()
    {
        UpdateBankDisplay();
    }

    public void UpdateBankDisplay()
    {
        if (GameManager.Instance != null && bankedSeedsText != null)
        {
            // Pull the persistent data from the GameManager
            bankedSeedsText.text = "Banked Seeds: " + GameManager.Instance.totalSeedsInBank;
        }
    }
}