using UnityEngine;
using System.Collections;

public class PathManager : MonoBehaviour
{
    [Header("Path Tiles (assign in order, base → park)")]
    public PathTile[] pathTiles;

    [Header("Park Gate")]
    public GameObject parkGateObject;  // A locked gate sprite/object

    private void Start()
    {
        RestoreProgress();
    }

    // Silently restores visual state on scene load — no animation
    private void RestoreProgress()
    {
        int unlocked = GameManager.Instance.sectionsUnlocked;

        for (int i = 0; i < pathTiles.Length; i++)
        {
            if (i < unlocked)
                pathTiles[i].RestoreState(PathTile.TileState.Grown);
            else
                pathTiles[i].RestoreState(PathTile.TileState.Locked);
        }

        UpdateParkGate();
    }

    // Called by PlantingStation when player successfully spends seeds
    public void UnlockNextSection()
    {
        int nextIndex = GameManager.Instance.sectionsUnlocked - 1;

        if (nextIndex >= 0 && nextIndex < pathTiles.Length)
        {
            // Stagger tiles if a section has multiple tiles
            StartCoroutine(AnimateSectionUnlock(nextIndex));
        }

        UpdateParkGate();
    }

    private IEnumerator AnimateSectionUnlock(int tileIndex)
    {
        yield return pathTiles[tileIndex].AnimateGrow(0f);
    }

    private void UpdateParkGate()
    {
        if (parkGateObject != null)
            parkGateObject.SetActive(!GameManager.Instance.IsPathComplete());
    }
}