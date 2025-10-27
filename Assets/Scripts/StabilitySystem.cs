using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the stability system for the clicker game.
/// Stability decreases with probability at click milestones and affects visual elements.
/// </summary>
public class StabilitySystem : MonoBehaviour
{
    [Header("Stability Settings")]
    public float initialStability = 100f;
    public float minDropAmount = 20f; // Minimum 20% drop
    public float maxDropAmount = 60f; // Maximum 60% drop
    public float dropProbability = 0.8f; // 80% chance to drop at each milestone
    
    [Header("Sprite Change Thresholds")]
    public float[] spriteThresholds = { 80f, 60f, 40f, 20f, 10f }; // Below these values, change sprite (5 thresholds for 6 sprites)
    
    [Header("UI References")]
    public Slider stabilityBar;
    public TextMeshProUGUI stabilityText;
    public Image stabilityBarFill;
    
    [Header("Sprite References")]
    public SpriteRenderer mainSpriteRenderer;
    public Sprite eggSprite; // Initial egg sprite that breaks on first click
    public Sprite[] stabilitySprites; // Placeholder sprites for different stability levels
    public Sprite gameOverSprite; // Sprite to show when stability reaches 0
    public Sprite necrosisteenSprite; // Special sprite for necrosisteen stage
    public Sprite pupilsfalloffSprite; // Special sprite for pupils fall off stage
    public TransitionAnimationSystem transitionSystem; // Reference to transition animation system
    
    // Current stability value
    private float currentStability;
    private int lastMilestoneReached = 0;
    private int currentSpriteIndex = 0;
    private bool isGameOver = false;
    private bool hasTriggeredSpecialStage = false; // Prevent multiple special stage triggers
    private bool hasInitialized = false; // Prevent sprite changes during initialization
    private bool hasEggBroken = false; // Track if egg has been broken
    
    // Wavering animation
    private bool isWavering = false;
    private Vector3 originalBarPosition;
    private float waverIntensity = 5f; // Increased for more dramatic effect
    private float waverSpeed = 15f; // Faster wobbling
    private float wobbleRange = 3f; // How much the stability value wobbles (reduced for subtle effect)
    
    void Start()
    {
        currentStability = initialStability;
        
        if (stabilityBar != null)
        {
            stabilityBar.maxValue = initialStability;
            stabilityBar.value = currentStability;
            originalBarPosition = stabilityBar.transform.position;
        }
        
        // Scale down the main sprite renderer
        if (mainSpriteRenderer != null)
        {
            mainSpriteRenderer.transform.localScale = new Vector3(0.7f, 0.7f, 1f); // Scale down to 70%
        }
        
        // Set initial sprite to EGG (not baby!)
        currentSpriteIndex = -1; // Special index for egg
        if (mainSpriteRenderer != null && eggSprite != null)
        {
            mainSpriteRenderer.sprite = eggSprite;
        }
        else
        {
            Debug.LogError("Failed to set egg sprite - missing components or egg sprite!");
        }
        
        UpdateStabilityUI();
        
        // Mark as initialized after first frame
        hasInitialized = true;
    }
    
    void Update()
    {
        // Force stability to 100% until egg is broken and first milestone reached
        if (!hasEggBroken || Clickable.Clicks < 100)
        {
            if (currentStability != initialStability)
            {
                currentStability = initialStability;
                UpdateStabilityUI();
            }
        }
        
        CheckForMilestoneDrops();
        UpdateWaveringAnimation();
    }
    
    void LateUpdate()
    {
        // Ensure egg sprite is set after all other Start() methods
        if (!hasEggBroken && mainSpriteRenderer != null && eggSprite != null)
        {
            if (mainSpriteRenderer.sprite != eggSprite)
            {
                mainSpriteRenderer.sprite = eggSprite;
            }
        }
    }
    
    /// <summary>
    /// Checks if we've reached a new click milestone and drops stability probabilistically
    /// </summary>
    private void CheckForMilestoneDrops()
    {
        // Don't drop stability if game is over or egg hasn't broken
        if (isGameOver || !hasEggBroken) return;
        
        int currentMilestone = GetCurrentMilestone(Clickable.Clicks);
        
        // If we've reached a new milestone
        if (currentMilestone > lastMilestoneReached)
        {
            lastMilestoneReached = currentMilestone;
            
            // Drop stability probabilistically
            if (Random.Range(0f, 1f) <= dropProbability)
            {
                DropStability();
            }
        }
    }
    
    /// <summary>
    /// Determines which milestone tier we're currently at
    /// </summary>
    private int GetCurrentMilestone(int clicks)
    {
        if (clicks >= 1000000) return 6;
        if (clicks >= 100000) return 5;
        if (clicks >= 10000) return 4;
        if (clicks >= 1000) return 3;
        if (clicks >= 100) return 2;
        return 1;
    }
    
    /// <summary>
    /// Drops stability by a random amount between minDropAmount and maxDropAmount
    /// </summary>
    private void DropStability()
    {
        // Calculate random drop amount between min and max
        float dropAmount = Random.Range(minDropAmount, maxDropAmount);
        currentStability -= dropAmount;
        currentStability = Mathf.Max(0f, currentStability); // Don't go below 0
        
        // Check for game over
        if (currentStability <= 0f && !isGameOver)
        {
            TriggerGameOver();
            return;
        }
        
        // Start wavering if below 100%
        if (currentStability < initialStability)
        {
            isWavering = true;
        }
        
        // Check for sprite changes
        CheckForSpriteChange();
        
        UpdateStabilityUI();
    }
    
    /// <summary>
    /// Triggers a special branching stage that leads to game over
    /// </summary>
    private void TriggerSpecialStage(string stageType)
    {
        hasTriggeredSpecialStage = true;
        isGameOver = true;
        
        // Set the appropriate special sprite
        if (mainSpriteRenderer != null)
        {
            if (stageType == "necrosisteen" && necrosisteenSprite != null)
            {
                mainSpriteRenderer.sprite = necrosisteenSprite;
            }
            else if (stageType == "pupilsfalloff" && pupilsfalloffSprite != null)
            {
                mainSpriteRenderer.sprite = pupilsfalloffSprite;
            }
            else
            {
                // Fallback to regular game over sprite
                if (gameOverSprite != null)
                {
                    mainSpriteRenderer.sprite = gameOverSprite;
                }
            }
        }
        
        // Stop wavering animation
        isWavering = false;
        
        // You could add additional special stage effects here
        // For example: play special sound effects, trigger special animations, etc.
    }
    
    /// <summary>
    /// Triggers the game over state
    /// </summary>
    private void TriggerGameOver()
    {
        isGameOver = true;
        
        // Set game over sprite
        if (mainSpriteRenderer != null && gameOverSprite != null)
        {
            mainSpriteRenderer.sprite = gameOverSprite;
        }
        
        // Stop wavering animation
        isWavering = false;
        
        // You could add additional game over effects here
        // For example: disable clicking, show game over UI, etc.
    }
    
    /// <summary>
    /// Breaks the egg on first click and triggers egg animation
    /// </summary>
    public void BreakEgg()
    {
        if (!hasEggBroken)
        {
            hasEggBroken = true;
            
            // Trigger the egg animation through TransitionAnimationSystem
            if (transitionSystem != null)
            {
                transitionSystem.TriggerEggAnimation();
            }
            else
            {
                // Fallback: directly transition to baby sprite
                currentSpriteIndex = 0;
                if (mainSpriteRenderer != null && stabilitySprites != null && stabilitySprites.Length > 0 && stabilitySprites[0] != null)
                {
                    mainSpriteRenderer.sprite = stabilitySprites[0];
                }
            }
        }
    }
    
    /// <summary>
    /// Checks if the egg has been broken
    /// </summary>
    public bool HasEggBroken()
    {
        return hasEggBroken;
    }
    
    /// <summary>
    /// Sets the current sprite index (used by TransitionAnimationSystem)
    /// </summary>
    public void SetSpriteIndex(int index)
    {
        currentSpriteIndex = index;
    }
    
    /// <summary>
    /// Checks if we need to change the sprite based on stability thresholds
    /// </summary>
    private void CheckForSpriteChange()
    {
        // Don't change sprites if game is over, not initialized, egg hasn't broken, or haven't reached first milestone
        if (isGameOver || !hasInitialized || !hasEggBroken || Clickable.Clicks < 100) return;
        
        int newSpriteIndex = GetSpriteIndexForStability(currentStability);
        
        // Check for special branching stages before normal sprite change
        if (!hasTriggeredSpecialStage)
        {
            // Check for necrosisteen stage (teen stage - sprite index 3: goodendingteen)
            if (newSpriteIndex == 3 && Random.Range(0f, 1f) <= 0.5f)
            {
                TriggerSpecialStage("necrosisteen");
                return;
            }
            
            // Check for pupils fall off stage (toddler stage - sprite index 2: goodendingtoddler)
            if (newSpriteIndex == 2 && Random.Range(0f, 1f) <= 0.5f)
            {
                TriggerSpecialStage("pupilsfalloff");
                return;
            }
        }
        
        // Add bounds checking to prevent index out of range errors
        if (newSpriteIndex != currentSpriteIndex && 
            newSpriteIndex >= 0 && 
            newSpriteIndex < stabilitySprites.Length && 
            stabilitySprites != null && 
            stabilitySprites.Length > 0)
        {
        currentSpriteIndex = newSpriteIndex;
            
            // Use transition system if available, otherwise use direct sprite change
            if (transitionSystem != null)
            {
                // Only trigger transition if we're past the initial state (after first click)
                if (Clickable.Clicks > 0)
                {
                    transitionSystem.TriggerTransition(newSpriteIndex);
                }
            }
            else if (mainSpriteRenderer != null && stabilitySprites[currentSpriteIndex] != null)
            {
                mainSpriteRenderer.sprite = stabilitySprites[currentSpriteIndex];
            }
        }
    }
    
    /// <summary>
    /// Determines which sprite index to use based on current stability
    /// </summary>
    private int GetSpriteIndexForStability(float stability)
    {
        for (int i = 0; i < spriteThresholds.Length; i++)
        {
            if (stability < spriteThresholds[i])
            {
                return i + 1; // Return index 1, 2, 3, 4, 5 for thresholds
            }
        }
        return 0; // Default sprite (full stability)
    }
    
    /// <summary>
    /// Updates the stability bar and text UI
    /// </summary>
    private void UpdateStabilityUI()
    {
        if (stabilityBar != null)
        {
            // Set the actual stability value (no wobbling on the value itself)
            stabilityBar.value = currentStability;
        }
        
        if (stabilityText != null)
        {
            // Show the actual stability value (no wobbling on the text)
            stabilityText.text = $"Stability: {currentStability:F1}%";
        }
    }
    
    /// <summary>
    /// Handles the wavering animation for the stability bar
    /// </summary>
    private void UpdateWaveringAnimation()
    {
        if (isWavering && stabilityBar != null && !isGameOver)
        {
            // Create wobbling effect on the slider fill by changing the value back and forth
            float wobbleValue = Mathf.Sin(Time.time * waverSpeed) * wobbleRange;
            float wobbledValue = currentStability + wobbleValue;
            
            // Clamp the wobbled value to stay within reasonable bounds
            wobbledValue = Mathf.Clamp(wobbledValue, 0f, initialStability);
            stabilityBar.value = wobbledValue;
            
            // Also move the bar position slightly for extra effect
            float waverX = Mathf.Sin(Time.time * waverSpeed * 0.5f) * waverIntensity;
            float waverY = Mathf.Cos(Time.time * waverSpeed * 0.3f) * waverIntensity * 0.5f;
            stabilityBar.transform.position = originalBarPosition + new Vector3(waverX, waverY, 0);
            
            // Change bar color based on stability
            if (stabilityBarFill != null)
            {
                Color barColor = Color.Lerp(Color.red, Color.green, currentStability / initialStability);
                stabilityBarFill.color = barColor;
            }
        }
        else if (isGameOver && stabilityBar != null)
        {
            // Set stability bar to 0 and make it red when game is over
            stabilityBar.value = 0f;
            if (stabilityBarFill != null)
            {
                stabilityBarFill.color = Color.red;
            }
        }
    }
    
    /// <summary>
    /// Gets the current stability percentage
    /// </summary>
    public float GetCurrentStability()
    {
        return currentStability;
    }
    
    /// <summary>
    /// Gets the current sprite index
    /// </summary>
    public int GetCurrentSpriteIndex()
    {
        return currentSpriteIndex;
    }
    
    /// <summary>
    /// Checks if the game is in game over state
    /// </summary>
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    /// <summary>
    /// Resets the stability system (for restarting the game)
    /// </summary>
    public void ResetStability()
    {
        currentStability = initialStability;
        lastMilestoneReached = 0;
        currentSpriteIndex = -1; // Reset to egg state
        isGameOver = false;
        isWavering = false;
        hasTriggeredSpecialStage = false;
        hasInitialized = false;
        hasEggBroken = false; // Reset egg state
        
        // Reset stability bar position
        if (stabilityBar != null)
        {
            stabilityBar.value = currentStability;
            stabilityBar.transform.position = originalBarPosition;
        }
        
        // Reset to egg sprite
        if (mainSpriteRenderer != null && eggSprite != null)
        {
            mainSpriteRenderer.sprite = eggSprite;
        }
        
        UpdateStabilityUI();
        hasInitialized = true;
    }
}
