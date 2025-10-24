using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages transition animations between different game states
/// </summary>
public class TransitionAnimationSystem : MonoBehaviour
{
    [Header("Animation Settings")]
    public Sprite[] animation0; // First animation (egg sequence)
    public Sprite[] animation1; // Second animation (100 clicks)
    public Sprite[] animation2; // Third animation (1,000 clicks)
    public Sprite[] animation3; // Fourth animation (10,000 clicks)
    public Sprite[] animation4; // Fifth animation (100,000 clicks)
    public float panelDuration = 0.5f; // Time between each panel
    public bool playOnFirstClick = true;
    
    [Header("References")]
    public SpriteRenderer mainSpriteRenderer;
    public StabilitySystem stabilitySystem;
    
    [Header("Game State Sprites")]
    public Sprite[] gameStateSprites; // Your manually added sprites for different stability levels
    
    // Animation state
    private bool hasPlayedFirstAnimation = false;
    private bool isPlayingTransition = false;
    private int currentGameState = 0; // 0 = initial, 1 = after first animation, 2 = after 100 clicks, etc.
    
    void Start()
    {
        // Start with the first panel of the first transition animation
        if (animation0 != null && animation0.Length > 0 && mainSpriteRenderer != null)
        {
            mainSpriteRenderer.sprite = animation0[0];
        }
        
        // Set initial game state to 0 (before first click)
        currentGameState = 0;
    }
    
    void Update()
    {
        // Check if we should play the first transition animation
        if (playOnFirstClick && !hasPlayedFirstAnimation && Clickable.Clicks >= 1)
        {
            StartCoroutine(PlayTransitionAnimation(0, 1)); // Play first animation (index 0) for game state 1
            hasPlayedFirstAnimation = true;
            currentGameState = 1; // Update current game state
        }
        
        // Check for stability-based transitions (only after first click)
        if (stabilitySystem != null && Clickable.Clicks > 0)
        {
            int newGameState = GetGameStateFromStability(stabilitySystem.GetCurrentStability());
            if (newGameState != currentGameState && newGameState > currentGameState)
            {
                // Use different animation for each game state (game state 2 uses animation 1, etc.)
                int animationIndex = Mathf.Max(0, newGameState - 1);
                StartCoroutine(PlayTransitionAnimation(animationIndex, newGameState));
                currentGameState = newGameState;
            }
        }
    }
    
    /// <summary>
    /// Plays the transition animation between panels
    /// </summary>
    private IEnumerator PlayTransitionAnimation(int animationIndex, int targetState)
    {
        if (isPlayingTransition)
            yield break;
            
        // Get the appropriate animation array
        Sprite[] currentAnimation = GetAnimationArray(animationIndex);
        if (currentAnimation == null || currentAnimation.Length == 0)
            yield break;
            
        isPlayingTransition = true;
        
        // Play the animation for the specified animation index (variable length)
        for (int i = 0; i < currentAnimation.Length; i++)
        {
            if (mainSpriteRenderer != null && currentAnimation[i] != null)
            {
                mainSpriteRenderer.sprite = currentAnimation[i];
            }
            
            yield return new WaitForSeconds(panelDuration);
        }
        
        // After animation completes, set the appropriate game state sprite
        SetGameStateSprite(targetState);
        
        isPlayingTransition = false;
    }
    
    /// <summary>
    /// Gets the animation array based on the index
    /// </summary>
    private Sprite[] GetAnimationArray(int index)
    {
        switch (index)
        {
            case 0: return animation0;
            case 1: return animation1;
            case 2: return animation2;
            case 3: return animation3;
            case 4: return animation4;
            default: return null;
        }
    }
    
    /// <summary>
    /// Sets the sprite based on the current game state
    /// </summary>
    private void SetGameStateSprite(int gameState)
    {
        if (mainSpriteRenderer == null)
            return;
            
        // Use StabilitySystem sprites if available, otherwise use our own
        if (stabilitySystem != null && stabilitySystem.stabilitySprites != null)
        {
            int spriteIndex = GetSpriteIndexForGameState(gameState);
            if (spriteIndex >= 0 && spriteIndex < stabilitySystem.stabilitySprites.Length)
            {
                mainSpriteRenderer.sprite = stabilitySystem.stabilitySprites[spriteIndex];
                Debug.Log($"Set StabilitySystem sprite to index {spriteIndex} for game state {gameState}");
            }
        }
        else if (gameStateSprites != null)
        {
            int spriteIndex = GetSpriteIndexForGameState(gameState);
            if (spriteIndex >= 0 && spriteIndex < gameStateSprites.Length)
            {
                mainSpriteRenderer.sprite = gameStateSprites[spriteIndex];
                Debug.Log($"Set game state sprite to index {spriteIndex} for game state {gameState}");
            }
        }
    }
    
    /// <summary>
    /// Determines which game state we're in based on stability
    /// </summary>
    private int GetGameStateFromStability(float stability)
    {
        if (stability < 20f) return 5;  // 6th state
        if (stability < 40f) return 4;  // 5th state
        if (stability < 60f) return 3;  // 4th state
        if (stability < 80f) return 2;  // 3rd state
        return 1; // 2nd state (after first animation)
    }
    
    /// <summary>
    /// Maps game states to sprite indices
    /// </summary>
    private int GetSpriteIndexForGameState(int gameState)
    {
        // Game state 1 = after first animation (use sprite 0)
        // Game state 2 = after 100 clicks (use sprite 1)
        // Game state 3 = after 1000 clicks (use sprite 2)
        // etc.
        return gameState - 1;
    }
    
    /// <summary>
    /// Manually trigger a transition animation
    /// </summary>
    public void TriggerTransition(int targetState)
    {
        if (!isPlayingTransition)
        {
            int animationIndex = Mathf.Max(0, targetState - 1);
            StartCoroutine(PlayTransitionAnimation(animationIndex, targetState));
        }
    }
    
    /// <summary>
    /// Set the panel duration for transition animations
    /// </summary>
    public void SetPanelDuration(float duration)
    {
        panelDuration = duration;
    }
}
