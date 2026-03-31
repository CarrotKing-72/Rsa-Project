using UnityEngine;
using System.Collections;

public class PathTile : MonoBehaviour
{
    public enum TileState { Locked, Seeded, Grown }

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite lockedSprite;
    public Sprite seededSprite;
    public Sprite grownSprite;

    [Header("Effects")]
    public ParticleSystem growParticles;

    [Header("Animation")]
    public float transitionDelay = 0.3f;

    private TileState _currentState = TileState.Locked;
    private BoxCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        SetVisual(TileState.Locked);
    }

    // Called by PathManager to animate this tile growing
    public IEnumerator AnimateGrow(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Stage 1: Seeds appear
        SetVisual(TileState.Seeded);
        yield return new WaitForSeconds(0.8f);

        // Stage 2: Fully grown -- open the path
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
                SetCollider(true);   // Block the player
                break;
            case TileState.Seeded:
                spriteRenderer.sprite = seededSprite;
                SetCollider(true);   // Still blocked while growing
                break;
            case TileState.Grown:
                spriteRenderer.sprite = grownSprite;
                SetCollider(false);  // Path is open
                break;
        }
    }

    private void SetCollider(bool enabled)
    {
        if (_collider != null)
            _collider.enabled = enabled;
    }

    public TileState CurrentState => _currentState;
}
