using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Map Dimensions")]
    public int width = 10;
    public int height = 9;

    [Header("Generation Settings")]
    [Range(0, 1)] public float treeRatio = 0.15f;
    public Transform playerTransform;

    [Header("Prefabs")]
    public GameObject concretePrefab;
    public GameObject treePrefab;
    public GameObject seedPrefab;
    public GameObject endTilePrefab;
    public GameObject borderPrefab; 

    private Vector2Int startPos;
    private Vector2Int endPos;

    // Reference to GridManager needed to set safe zones
    private GridManager gridManager;

    [Header("Seed Settings")]
    [SerializeField] private int minSeedsPerLevel = 2;
    [SerializeField] private int maxSeedsPerLevel = 3;
    [SerializeField] private float minDistanceBetweenSeeds = 3.5f;
    [Range(0f, 1f)] [SerializeField] private float seedEdgeBias = 0.45f;

    private List<Vector2Int> seedPositions = new List<Vector2Int>();


    private void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        foreach (Transform child in transform) { Destroy(child.gameObject); }

        // Reset player heat/stats if needed here
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                if (x == -1 || x == width || y == -1 || y == height)
                {
                    Vector3 borderPos = transform.position + new Vector3(x, y, 0);
                    Instantiate(borderPrefab, borderPos, Quaternion.identity, transform);
                }
            }
        }


        if (playerTransform != null)
        {
            PlayerStats stats = playerTransform.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.ResetStats();
            }

            Vector3 relativePos = playerTransform.position - transform.position;
            startPos = new Vector2Int(Mathf.RoundToInt(relativePos.x), Mathf.RoundToInt(relativePos.y));
        }

        DetermineSpecialPositions();

        HashSet<Vector2Int> path = CreateWindingPath();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Vector3 worldPos = new Vector3(transform.position.x + x, transform.position.y + y, 0);

                // END TILE: Spawns on a safe tile (tree base)
                if (pos == endPos)
                {
                    Instantiate(treePrefab, worldPos, Quaternion.identity, transform);
                    Instantiate(endTilePrefab, worldPos + new Vector3(0, 0, -0.1f), Quaternion.identity, transform);
                    continue;
                }

                // SEED: Spawns on concrete to maintain difficulty
                if (seedPositions.Contains(pos))
                {
                    Instantiate(concretePrefab, worldPos, Quaternion.identity, transform);
                    Instantiate(seedPrefab, worldPos + new Vector3(0, 0, -0.1f), Quaternion.identity, transform);
                    continue;
                }

                if (path.Contains(pos))
                {
                    GameObject p = (Random.value < 0.4f) ? treePrefab : concretePrefab;
                    Instantiate(p, worldPos, Quaternion.identity, transform);
                }
                else
                {
                    GameObject p = (Random.value < treeRatio) ? treePrefab : concretePrefab;
                    Instantiate(p, worldPos, Quaternion.identity, transform);
                }
            }
        }
    }

    private void DetermineSpecialPositions()
    {
        int endX, endY;
        if (startPos.x < width / 2) endX = Random.Range(width / 2, width - 1);
        else endX = Random.Range(0, width / 2);

        endY = (Random.value > 0.5f) ? 0 : height - 1;
        endPos = new Vector2Int(endX, endY);

        seedPositions.Clear();

        int minCount = Mathf.Max(1, minSeedsPerLevel);
        int maxCount = Mathf.Max(minCount, maxSeedsPerLevel);
        int targetSeedCount = Random.Range(minCount, maxCount + 1);
        float spacing = Mathf.Max(0f, minDistanceBetweenSeeds);

        // Try strict spacing first, then relax to guarantee placement on small maps.
        TryAddSeedPositions(targetSeedCount, 8f, 8f, spacing);
        if (seedPositions.Count < targetSeedCount) TryAddSeedPositions(targetSeedCount, 6f, 6f, spacing);
        if (seedPositions.Count < targetSeedCount) TryAddSeedPositions(targetSeedCount, 4f, 4f, spacing * 0.8f);
        if (seedPositions.Count < targetSeedCount) TryAddSeedPositions(targetSeedCount, 2f, 2f, spacing * 0.6f);
        if (seedPositions.Count < targetSeedCount) TryAddSeedPositions(targetSeedCount, 0f, 0f, spacing * 0.4f);

        if (seedPositions.Count == 0)
        {
            Debug.LogWarning("LevelGenerator: No valid seed positions found.");
        }
    }

    private void TryAddSeedPositions(int targetSeedCount, float minDistanceFromStart, float minDistanceFromEnd, float minSeedSpacing)
    {
        int attempts = 0;
        while (seedPositions.Count < targetSeedCount && attempts < 500)
        {
            attempts++;
            int rx = Random.Range(0, width);
            int ry = Random.Range(0, height);

            // Edge bias controls how often seeds are forced onto borders.
            if (Random.value < seedEdgeBias)
            {
                if (Random.value > 0.5f) rx = (Random.value > 0.5f) ? 0 : width - 1;
                else ry = (Random.value > 0.5f) ? 0 : height - 1;
            }

            Vector2Int candidate = new Vector2Int(rx, ry);

            bool tooCloseToStart = Vector2Int.Distance(candidate, startPos) <= minDistanceFromStart;
            bool tooCloseToEnd = Vector2Int.Distance(candidate, endPos) <= minDistanceFromEnd;
            bool duplicate = seedPositions.Contains(candidate);
            bool onEnd = candidate == endPos;
            bool tooCloseToOtherSeed = false;

            foreach (Vector2Int existingSeed in seedPositions)
            {
                if (Vector2Int.Distance(candidate, existingSeed) <= minSeedSpacing)
                {
                    tooCloseToOtherSeed = true;
                    break;
                }
            }

            if (!tooCloseToStart && !tooCloseToEnd && !duplicate && !onEnd && !tooCloseToOtherSeed)
            {
                seedPositions.Add(candidate);
            }
        }
    }

    private HashSet<Vector2Int> CreateWindingPath()
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        Vector2Int current = startPos;

        foreach (Vector2Int seed in seedPositions)
        {
            current = WalkPath(current, seed, path);
        }

        WalkPath(current, endPos, path);
        return path;
    }

    private Vector2Int WalkPath(Vector2Int from, Vector2Int to, HashSet<Vector2Int> path)
    {
        Vector2Int curr = from;
        int safety = 0;
        while (curr != to && safety < 500)
        {
            safety++;
            if (Random.value > 0.5f && curr.x != to.x)
                curr.x += (int)Mathf.Sign(to.x - curr.x);
            else if (curr.y != to.y)
                curr.y += (int)Mathf.Sign(to.y - curr.y);

            path.Add(curr);
        }
        return curr;
    }
}