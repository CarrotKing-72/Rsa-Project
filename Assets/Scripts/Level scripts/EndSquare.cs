using UnityEngine;
using UnityEngine.SceneManagement;

// Requires a Collider2D set to "Is Trigger" on this GameObject
[RequireComponent(typeof(Collider2D))]
public class EndSquare : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SeedInventory inventory = other.GetComponent<SeedInventory>();
        if (inventory != null)
            GameManager.Instance.SaveSeeds(inventory.seedCount);

        Debug.Log("Returning to Hub...");
        SceneManager.LoadScene("Hub");
    }
}