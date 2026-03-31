using UnityEngine;

public class HubSpawnPoint : MonoBehaviour
{
    [SerializeField] private Vector3 hubSpawnPosition = new Vector3(-10.5f, 0.5f, 0f);

    private void Start()
    {
        // Set both the visual position AND the movement target
        // so PlayerController.Update() doesn't drag the player back to the grid line
        transform.position = hubSpawnPosition;

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.targetPos = hubSpawnPosition;
            pc.currentGridPos = new Vector2Int(-11, 0);
        }
    }
}