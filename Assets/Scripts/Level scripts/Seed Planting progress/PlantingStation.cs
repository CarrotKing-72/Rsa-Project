using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlantingStation : MonoBehaviour
{
    [Header("References")]
    public PathManager pathManager;

    [Header("Prompt UI (Planter/Canvas)")]
    public GameObject promptPanel;          // The panel with background image
    public TMP_Text promptTitleText;        // "[ E ]  Plant"
    public TMP_Text promptCostText;         // "🌱 Cost: X seeds"

    [Header("Seed Bank UI (HUDCanvas)")]
    public TMP_Text seedBankText;           // "🌱 12 Seeds"

    [Header("Settings")]
    public float interactRadius = 1.2f;

    // Colors
    private static readonly Color _normalColor  = new Color(0.66f, 0.84f, 0.63f); // soft green
    private static readonly Color _errorColor   = new Color(0.95f, 0.35f, 0.35f); // warm red
    private static readonly Color _titleColor   = Color.white;

    private Transform _player;
    private bool _playerInRange = false;
    private bool _isFlashing = false;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        RefreshSeedBank();
        SetPromptVisible(false);
    }

    private void Update()
    {
        if (_player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);
        bool wasInRange = _playerInRange;
        _playerInRange = dist <= interactRadius;

        // Animate in/out when range changes
        if (_playerInRange != wasInRange)
        {
            bool show = _playerInRange && !GameManager.Instance.IsPathComplete();
            if (show) StartCoroutine(AnimatePromptIn());
            else SetPromptVisible(false);
        }

        if (_playerInRange && Input.GetKeyDown(KeyCode.E))
            TryPlant();
    }

    private void TryPlant()
    {
        bool success = GameManager.Instance.TryUnlockNextSection();

        if (success)
        {
            pathManager.UnlockNextSection();
            RefreshSeedBank();
            RefreshPromptText();

            if (GameManager.Instance.IsPathComplete())
            {
                SetPromptVisible(false);
                Debug.Log("Park path complete!");
            }
        }
        else
        {
            if (!_isFlashing)
            {
                StopAllCoroutines();
                StartCoroutine(FlashNotEnoughSeeds());
            }
        }
    }

    // Slide + fade the prompt panel in from slightly below
    private IEnumerator AnimatePromptIn()
    {
        if (promptPanel == null) yield break;

        promptPanel.SetActive(true);
        RefreshPromptText();

        CanvasGroup cg = promptPanel.GetComponent<CanvasGroup>();
        RectTransform rt = promptPanel.GetComponent<RectTransform>();

        if (cg == null) cg = promptPanel.AddComponent<CanvasGroup>();

        Vector2 startPos = rt.anchoredPosition + Vector2.down * 10f;
        Vector2 endPos   = rt.anchoredPosition;

        float duration = 0.15f;
        float elapsed  = 0f;
        cg.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            cg.alpha = t;
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        cg.alpha = 1f;
        rt.anchoredPosition = endPos;
    }

    // Flash red with shake on not enough seeds
    private IEnumerator FlashNotEnoughSeeds()
    {
        _isFlashing = true;

        if (promptTitleText) promptTitleText.text = "Not enough seeds!";
        if (promptCostText)  promptCostText.text  = "";

        // Shake the panel
        RectTransform rt = promptPanel != null ? promptPanel.GetComponent<RectTransform>() : null;
        Vector2 originalPos = rt != null ? rt.anchoredPosition : Vector2.zero;

        float shakeDuration = 0.3f;
        float elapsed = 0f;
        float shakeMagnitude = 4f;

        SetTextColor(_errorColor, _errorColor);

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            if (rt != null)
            {
                float x = originalPos.x + Random.Range(-shakeMagnitude, shakeMagnitude);
                rt.anchoredPosition = new Vector2(x, originalPos.y);
            }
            yield return null;
        }

        if (rt != null) rt.anchoredPosition = originalPos;

        yield return new WaitForSeconds(0.8f);

        SetTextColor(_titleColor, _normalColor);
        RefreshPromptText();

        _isFlashing = false;
    }

    private void RefreshPromptText()
    {
        if (GameManager.Instance.IsPathComplete()) return;
        int cost = GameManager.Instance.seedsPerSection;

        if (promptTitleText) 
        {
            promptTitleText.text  = "[ E ]  Plant";
            promptTitleText.color = _titleColor;
        }
        if (promptCostText)  
        {
            promptCostText.text  = $"Cost: {cost} seeds";
            promptCostText.color = _normalColor;
        }
    }

    private void RefreshSeedBank()
    {
        if (seedBankText != null)
            seedBankText.text = $"{GameManager.Instance.totalSeedsInBank} Seeds";
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptPanel != null) promptPanel.SetActive(visible);
    }

    private void SetTextColor(Color titleCol, Color costCol)
    {
        if (promptTitleText) promptTitleText.color = titleCol;
        if (promptCostText)  promptCostText.color  = costCol;
    }
}