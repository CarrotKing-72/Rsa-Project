using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Seed : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SeedInventory inventory = other.GetComponent<SeedInventory>();
        if (inventory != null)
            inventory.AddSeed(1);

        Debug.Log("Seed Picked Up!");
        Destroy(gameObject);
    }
}