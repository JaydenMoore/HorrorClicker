using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main game manager that coordinates all game systems.
/// Handles initialization and communication between different game components.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("System References")]
    public UpgradeSystem upgradeSystem;
    public StabilitySystem stabilitySystem;
    public ClickTracker clickTracker;
    
    [Header("Game Settings")]
    public bool debugMode = false;
    
    void Start()
    {
        // Initialize all systems
        InitializeSystems();
        
        if (debugMode)
        {
            Debug.Log("GameManager initialized with all systems");
        }
    }
    
    /// <summary>
    /// Initializes and connects all game systems
    /// </summary>
    private void InitializeSystems()
    {
        // Find systems if not assigned in inspector
        if (upgradeSystem == null)
            upgradeSystem = FindObjectOfType<UpgradeSystem>();
            
        if (stabilitySystem == null)
            stabilitySystem = FindObjectOfType<StabilitySystem>();
            
        if (clickTracker == null)
            clickTracker = FindObjectOfType<ClickTracker>();
        
        // Verify all systems are found
        if (upgradeSystem == null)
            Debug.LogWarning("UpgradeSystem not found! Upgrade features will not work.");
            
        if (stabilitySystem == null)
            Debug.LogWarning("StabilitySystem not found! Stability features will not work.");
            
        if (clickTracker == null)
            Debug.LogWarning("ClickTracker not found! Click display will not work.");
    }
    
    /// <summary>
    /// Gets the current game state for debugging or save/load
    /// </summary>
    public GameState GetGameState()
    {
        return new GameState
        {
            totalClicks = Clickable.Clicks,
            stability = stabilitySystem != null ? stabilitySystem.GetCurrentStability() : 100f,
            currentSpriteIndex = stabilitySystem != null ? stabilitySystem.GetCurrentSpriteIndex() : 0
        };
    }
    
    /// <summary>
    /// Resets the game to initial state (for testing or new game)
    /// </summary>
    public void ResetGame()
    {
        Clickable.Clicks = 0;
        
        if (stabilitySystem != null)
        {
            // Reset stability system
            stabilitySystem.GetType().GetField("currentStability", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(stabilitySystem, 100f);
        }
        
        Debug.Log("Game reset to initial state");
    }
}

/// <summary>
/// Data structure to hold game state information
/// </summary>
[System.Serializable]
public class GameState
{
    public int totalClicks;
    public float stability;
    public int currentSpriteIndex;
}
