using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSquare : MonoBehaviour
{
    private LevelGenerator levelGen;

    private void Start()
    {
        levelGen = FindFirstObjectByType<LevelGenerator>();
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector2.Distance(transform.position, player.transform.position) < 0.1f)
        {
            // 1. Find the player's current run seeds
            SeedInventory inventory = player.GetComponent<SeedInventory>();
            if (inventory != null)
            {
                // 2. Save them to the persistent Manager
                GameManager.Instance.SaveSeeds(inventory.seedCount);
            }

            // 3. Switch to the Hub scene
            Debug.Log("Returning to Hub...");
            SceneManager.LoadScene("Hub");
        }
    }
}