using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public LayerMask obstacleLayer;

    [Header("References")]
    public GridManager gridManager;
    public PlayerStats playerStats;

    private Animator animator;
    private Vector3 targetPos;
    private Vector2Int currentGridPos;
    private bool isMoving;

    // For your Animator parameters
    public bool LeftPress;
    public bool RightPress;
    public bool UpPress;
    public bool DownPress;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (!gridManager)
            gridManager = FindFirstObjectByType<GridManager>();
        if (!playerStats)
            playerStats = GetComponent<PlayerStats>();

        currentGridPos = Vector2Int.RoundToInt(transform.position);
        targetPos = new Vector3(currentGridPos.x, currentGridPos.y, 0);
        transform.position = targetPos;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            transform.position = targetPos;
            isMoving = false;
            HandleMovementInput();
        }

        UpdateAnimationParams();
        ApplyTileEffectPerSecond();
    }

    private void UpdateAnimationParams()
    {
        // Keep these for your walking logic if needed
        LeftPress = Input.GetKey(KeyCode.A);
        RightPress = Input.GetKey(KeyCode.D);
        UpPress = Input.GetKey(KeyCode.W);
        DownPress = Input.GetKey(KeyCode.S);

        if (animator)
        {
            animator.SetBool("LeftPress", LeftPress);
            animator.SetBool("RightPress", RightPress);
            animator.SetBool("UpPress", UpPress);
            animator.SetBool("DownPress", DownPress);
            animator.SetBool("isMoving", isMoving);
        }
    }

    private void HandleMovementInput()
    {
        if (isMoving) return;

        Vector2Int moveDir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) moveDir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) moveDir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) moveDir = Vector2Int.right;

        if (moveDir != Vector2Int.zero)
        {
            Vector3 potentialTarget = targetPos + (Vector3)(Vector2)moveDir;
            Collider2D hit = Physics2D.OverlapBox(potentialTarget, new Vector2(0.8f, 0.8f), 0, obstacleLayer);

            if (hit == null)
            {
                currentGridPos += moveDir;
                targetPos = potentialTarget;
                isMoving = true;

                // NEW: Update direction floats for the Idle Blend Tree
                if (animator)
                {
                    animator.SetFloat("LastMoveX", moveDir.x);
                    animator.SetFloat("LastMoveY", moveDir.y);
                }

                HandleTileOnStep(currentGridPos);
            }
            else
            {
                Debug.Log("Blocked by: " + hit.name);
            }
        }
    }

    private void ApplyTileEffectPerSecond()
    {
        if (gridManager == null) return;
        TileType currentTile = gridManager.GetTileTypeAtPosition(currentGridPos);
        float tempEffect = gridManager.GetTemperatureEffect(currentTile);
        playerStats.ChangeTemperature(tempEffect * Time.deltaTime);
    }

    private void HandleTileOnStep(Vector2Int gridPos)
    {
        if (gridManager == null) return;
        TileType tileType = gridManager.GetTileTypeAtPosition(gridPos);
    }
}