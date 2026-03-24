using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Economy")]
    public int totalSeedsInBank = 0;
    
    [Header("Path Progress")]
    public int totalPathSections = 5;    // How many sections to unlock the park
    public int sectionsUnlocked = 0;
    public int seedsPerSection = 3;      // Cost to unlock each section

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveSeeds(int amount)
    {
        totalSeedsInBank += amount;
        Debug.Log("Total Seeds Banked: " + totalSeedsInBank);
    }

    // Returns true if purchase succeeded
    public bool TryUnlockNextSection()
    {
        if (sectionsUnlocked >= totalPathSections) return false;
        if (totalSeedsInBank < seedsPerSection) return false;

        totalSeedsInBank -= seedsPerSection;
        sectionsUnlocked++;
        Debug.Log($"Section {sectionsUnlocked} unlocked! Seeds remaining: {totalSeedsInBank}");
        return true;
    }

    public bool IsPathComplete() => sectionsUnlocked >= totalPathSections;
}