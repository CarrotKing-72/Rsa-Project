using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuPanel;

    public GameObject settingsPanel;

    [Header("Settings UI")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string KEY_MUSIC = "musicVolume";
    private const string KEY_SFX   = "sfxVolume";
    
    private bool _isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    // Called by Escape key or a pause button on screen
    public void TogglePause()
    {
        _isPaused = !_isPaused;
        pauseMenuPanel.SetActive(_isPaused);

        // timeScale 0 freezes everything -- Update, physics, animations all stop
        // your UI buttons still work because they run off real time not game time
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    // Wire to your Resume button
    public void Resume()
    {
        _isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Wire to your Quit to Hub button
    public void QuitToHub()
    {
        // ALWAYS reset timeScale before loading a new scene
        // if you forget this, the Hub will load frozen and nothing will move
        Time.timeScale = 1f;
        SceneManager.LoadScene("Hub");
    }

    // Wire to your Quit to Main Menu button if you want one
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    
    public void OnSettingsPressed()
    {
        StartCoroutine(FadeToPanel(pauseMenuPanel, settingsPanel));
    }
    
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
        StartCoroutine(FadeToPanel(settingsPanel, pauseMenuPanel));
    }
    // Called when this object is destroyed (scene unloads) -- safety net
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

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
            elapsed  += Time.unscaledDeltaTime;
            cg.alpha  = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    private void ShowPanel(GameObject panel)
    {
        if (pauseMenuPanel)     pauseMenuPanel.SetActive(panel == pauseMenuPanel);
        
        if (settingsPanel) settingsPanel.SetActive(panel == settingsPanel);
    }


}