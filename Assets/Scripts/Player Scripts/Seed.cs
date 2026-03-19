using UnityEngine;

public class Seed : MonoBehaviour
{
    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector2.Distance(transform.position, player.transform.position) < 0.1f)
        {
            // Give the seed to the player's inventory
            SeedInventory inventory = player.GetComponent<SeedInventory>();
            if (inventory != null)
            {
                inventory.AddSeed(1);
            }

            Debug.Log("Seed Picked Up!");
            Destroy(gameObject);
        }
    }
}