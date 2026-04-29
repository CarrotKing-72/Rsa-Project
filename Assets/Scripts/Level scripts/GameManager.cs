using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Economy")]
    public int totalSeedsInBank = 0;

    [Header("Path Progress")]
    public int totalPathSections = 5;
    public int sectionsUnlocked = 0;
    public int seedsPerSection = 3;

    private const string KEY_SEEDS    = "totalSeedsInBank";
    private const string KEY_SECTIONS = "sectionsUnlocked";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public event Action OnBankUpdated;
    public void SaveSeeds(int amount)
    {
        totalSeedsInBank += amount;
        SaveProgress();
        Debug.Log("Total Seeds Banked: " + totalSeedsInBank);
    }

    public bool TryUnlockNextSection()
    {
        if (sectionsUnlocked >= totalPathSections) return false;
        if (totalSeedsInBank < seedsPerSection) return false;

        totalSeedsInBank -= seedsPerSection;
        sectionsUnlocked++;
        SaveProgress();
        Debug.Log($"Section {sectionsUnlocked} unlocked! Seeds remaining: {totalSeedsInBank}");
        return true;
    }

    public bool IsPathComplete() => sectionsUnlocked >= totalPathSections;

    // Wipes all save data -- called by New Game button
     public void StartNewGame()
    {
       
        totalSeedsInBank = 0;
        sectionsUnlocked = 0;

       
        PlayerPrefs.DeleteKey(KEY_SEEDS);
        PlayerPrefs.DeleteKey(KEY_SECTIONS);
        PlayerPrefs.Save();

        
        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        if (stats != null)
            stats.ResetStats();

        
        OnBankUpdated?.Invoke();

        Debug.Log("[GameManager] New game started. All progress wiped.");

        
        SceneManager.LoadScene("Hub");
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(KEY_SEEDS, totalSeedsInBank);
        PlayerPrefs.SetInt(KEY_SECTIONS, sectionsUnlocked);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        totalSeedsInBank = PlayerPrefs.GetInt(KEY_SEEDS, 0);
        sectionsUnlocked = PlayerPrefs.GetInt(KEY_SECTIONS, 0);
        Debug.Log($"Progress loaded -- Seeds: {totalSeedsInBank}, Sections: {sectionsUnlocked}");
    }
}
