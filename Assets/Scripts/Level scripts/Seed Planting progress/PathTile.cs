using UnityEngine;
using System.Collections;

public class PathTile : MonoBehaviour
{
    public enum TileState { Locked, Seeded, Grown }

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite lockedSprite;    // Dirt / cracked concrete
    public Sprite seededSprite;    // Dirt with seeds on it
    public Sprite grownSprite;     // Grass / tree tile

    [Header("Effects")]
    public ParticleSystem growParticles;  // Green puff — assign in Inspector
    
    [Header("Animation")]
    public float transitionDelay = 0.3f; // Stagger between tiles

    private TileState _currentState = TileState.Locked;

    private void Awake()
    {
        SetVisual(TileState.Locked);
    }

    // Called by PathManager to animate this tile growing
    public IEnumerator AnimateGrow(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Stage 1: Seeds appear
        SetVisual(TileState.Seeded);
        yield return new WaitForSeconds(0.8f);

        // Stage 2: Grows green
        SetVisual(TileState.Grown);

        if (growParticles != null)
            growParticles.Play();
    }

    // Called on scene load to restore state instantly (no animation)
    public void RestoreState(TileState state)
    {
        _currentState = state;
        SetVisual(state);
    }

    private void SetVisual(TileState state)
    {
        _currentState = state;
        switch (state)
        {
            case TileState.Locked:
                spriteRenderer.sprite = lockedSprite;
                break;
            case TileState.Seeded:
                spriteRenderer.sprite = seededSprite;
                break;
            case TileState.Grown:
                spriteRenderer.sprite = grownSprite;
                break;
        }
    }

    public TileState CurrentState => _currentState;
}