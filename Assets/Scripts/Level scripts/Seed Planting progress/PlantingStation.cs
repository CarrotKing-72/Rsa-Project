using UnityEngine;
using TMPro;

public class PlantingStation : MonoBehaviour
{
    [Header("References")]
    public PathManager pathManager;
    public TMP_Text promptText;     // "Press E to plant (X seeds)"
    public TMP_Text seedBankText;   // Shows current banked seeds

    [Header("Settings")]
    public float interactRadius = 1.2f;

    private Transform _player;
    private bool _playerInRange = false;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateUI();
        SetPromptVisible(false);
    }

    private void Update()
    {
        if (_player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);
        _playerInRange = dist <= interactRadius;

        SetPromptVisible(_playerInRange && !GameManager.Instance.IsPathComplete());

        if (_playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryPlant();
        }
    }

    private void TryPlant()
    {
        bool success = GameManager.Instance.TryUnlockNextSection();

        if (success)
        {
            pathManager.UnlockNextSection();
            UpdateUI();

            if (GameManager.Instance.IsPathComplete())
            {
                SetPromptVisible(false);
                Debug.Log("Park path complete!");
            }
        }
        else
        {
            // Flash the prompt to signal not enough seeds
            StopAllCoroutines();
            StartCoroutine(FlashNotEnoughSeeds());
        }
    }

    private System.Collections.IEnumerator FlashNotEnoughSeeds()
    {
        promptText.text = "Not enough seeds!";
        promptText.color = Color.red;
        yield return new WaitForSeconds(1.2f);
        promptText.color = Color.white;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (seedBankText != null)
            seedBankText.text = "Seeds: " + GameManager.Instance.totalSeedsInBank;

        if (promptText != null && !GameManager.Instance.IsPathComplete())
        {
            promptText.text = $"[E] Plant  ({GameManager.Instance.seedsPerSection} seeds)";
        }
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptText != null) promptText.gameObject.SetActive(visible);
    }
}

