using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int totalSeedsInBank = 0;

    private void Awake()
    {
        // Singleton pattern: ensures only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // This keeps the object alive between scenes
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
}