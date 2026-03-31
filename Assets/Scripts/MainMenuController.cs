using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject subMenuPanel;
    public GameObject settingsPanel;

    [Header("Settings UI")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string KEY_MUSIC = "musicVolume";
    private const string KEY_SFX   = "sfxVolume";

    private void Start()
    {
        // Always boot to main panel
        ShowPanel(mainPanel);

        // Restore saved audio prefs
        if (musicSlider) musicSlider.value = PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
        if (sfxSlider)   sfxSlider.value   = PlayerPrefs.GetFloat(KEY_SFX, 1f);
    }

    // ---- Main Panel ----

    public void OnPlayPressed()
    {
        StartCoroutine(FadeToPanel(mainPanel, subMenuPanel));
    }

    public void OnSettingsPressed()
    {
        StartCoroutine(FadeToPanel(mainPanel, settingsPanel));
    }

    public void OnQuitPressed()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    // ---- Sub Menu Panel ----

    public void OnContinuePressed()
    {
        // GameManager already loaded save in Awake -- just go to Hub
        SceneManager.LoadScene("Hub");
    }

    public void OnNewGamePressed()
    {
        // Wipe save data then start fresh
        if (GameManager.Instance != null)
            GameManager.Instance.ResetProgress();

        SceneManager.LoadScene("Hub");
    }

    public void OnSubMenuBackPressed()
    {
        StartCoroutine(FadeToPanel(subMenuPanel, mainPanel));
    }

    // ---- Settings Panel ----

    public void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(KEY_MUSIC, value);
        PlayerPrefs.Save();
        // Wire to AudioMixer later: mixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
    }

    public void OnSfxVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(KEY_SFX, value);
        PlayerPrefs.Save();
    }

    public void OnSettingsBackPressed()
    {
        StartCoroutine(FadeToPanel(settingsPanel, mainPanel));
    }

    // ---- Panel Transitions ----

    private IEnumerator FadeToPanel(GameObject from, GameObject to)
    {
        yield return StartCoroutine(FadeCanvasGroup(from, 1f, 0f, 0.2f));
        from.SetActive(false);
        to.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(to, 0f, 1f, 0.2f));
    }

    private IEnumerator FadeCanvasGroup(GameObject panel, float startAlpha, float endAlpha, float duration)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed  += Time.deltaTime;
            cg.alpha  = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    private void ShowPanel(GameObject panel)
    {
        if (mainPanel)     mainPanel.SetActive(panel == mainPanel);
        if (subMenuPanel)  subMenuPanel.SetActive(panel == subMenuPanel);
        if (settingsPanel) settingsPanel.SetActive(panel == settingsPanel);
    }
}
