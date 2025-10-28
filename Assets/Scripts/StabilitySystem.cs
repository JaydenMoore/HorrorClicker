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
    public float minDropAmount = 15f; // Minimum 15% drop
    public float maxDropAmount = 45f; // Maximum 45% drop
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
    public Sprite[] necrosisteenSprites; // Special sprites for necrosisteen stage progression (0-4)
    public Sprite[] pupilsfalloffSprites; // Special sprites for pupils fall off stage progression (0-4)
    public TransitionAnimationSystem transitionSystem; // Reference to transition animation system
    
    // Current stability value
    private float currentStability;
    private int lastMilestoneReached = 0;
    private int currentSpriteIndex = 0;
    private bool isGameOver = false;
    private string specialStageType = ""; // Track which special stage we're in ("necrosisteen", "pupilsfalloff", or "")
    private int specialStageIndex = 0; // Track progression within special stage (0-4)
    private bool hasInitialized = false; // Prevent sprite changes during initialization
    private bool hasEggBroken = false; // Track if egg has been broken
    private bool hasReachedFirstMilestone = false; // Track if we've ever reached 100 clicks
    
    // Wavering animation
    private bool isWavering = false;
    private Vector3 originalBarPosition;
    private Vector3 originalSpritePosition;
    private float waverIntensity = 5f; // Increased for more dramatic effect
    private float waverSpeed = 15f; // Faster wobbling
    private float wobbleRange = 3f; // How much the stability value wobbles (reduced for subtle effect)
    
    // Sprite bobble animation
    private float spriteBobbleIntensity = 0.05f; // Subtle bobble effect
    private float spriteBobbleSpeed = 3f; // Slower than stability bar
    
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
            originalSpritePosition = mainSpriteRenderer.transform.position;
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
        if (!hasEggBroken || !hasReachedFirstMilestone)
        {
            if (currentStability != initialStability)
            {
                currentStability = initialStability;
                UpdateStabilityUI();
            }
        }
        
        // Track if we've ever reached the first milestone
        if (Clickable.Clicks >= 100 && !hasReachedFirstMilestone)
        {
            hasReachedFirstMilestone = true;
        }
        
        UpdateWaveringAnimation();
        UpdateSpriteBobble();
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
    /// Called by UpgradeSystem when an upgrade is purchased
    /// </summary>
    public void TriggerUpgradeDrop()
    {
        // Only drop stability if egg has broken and first milestone reached
        if (hasEggBroken && hasReachedFirstMilestone && !isGameOver)
        {
            // Apply the drop probability (80% chance to drop)
            if (Random.Range(0f, 1f) <= dropProbability)
            {
                DropStability();
            }
        }
    }
    
    /// <summary>
    /// Triggers a special branching stage that plays an animation sequence
    /// </summary>
    private void TriggerSpecialStage(string stageType)
    {
        specialStageType = stageType;
        specialStageIndex = 0;
        
        // Start the special stage animation sequence
        StartCoroutine(PlaySpecialStageAnimation(stageType));
    }
    
    /// <summary>
    /// Plays the special stage animation sequence (like egg hatching)
    /// </summary>
    private IEnumerator PlaySpecialStageAnimation(string stageType)
    {
        Sprite[] animationSprites = null;
        
        // Get the appropriate sprite array
        if (stageType == "necrosisteen" && necrosisteenSprites != null)
        {
            animationSprites = necrosisteenSprites;
        }
        else if (stageType == "pupilsfalloff" && pupilsfalloffSprites != null)
        {
            animationSprites = pupilsfalloffSprites;
        }
        
        if (animationSprites == null || animationSprites.Length == 0)
        {
            yield break;
        }
        
        // Play through each sprite in the animation
        for (int i = 0; i < animationSprites.Length; i++)
        {
            if (mainSpriteRenderer != null && animationSprites[i] != null)
            {
                mainSpriteRenderer.sprite = animationSprites[i];
                specialStageIndex = i;
            }
            
            // Wait before showing next sprite (0.5 seconds like egg animation)
            yield return new WaitForSeconds(0.5f);
        }
        
        // After animation completes, check if we should trigger game over
        if (specialStageIndex >= animationSprites.Length - 1)
        {
            // Reached the end of special stage progression - trigger game over
            TriggerGameOver();
        }
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
        if (isGameOver || !hasInitialized || !hasEggBroken || !hasReachedFirstMilestone) return;
        
        int newSpriteIndex = GetSpriteIndexForStability(currentStability);
        
        Debug.Log($"CheckForSpriteChange: currentStability={currentStability}, newSpriteIndex={newSpriteIndex}, currentSpriteIndex={currentSpriteIndex}");
        
        // Check for special branching stages before normal sprite change
        if (string.IsNullOrEmpty(specialStageType))
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
        else
        {
            // We're in a special stage - don't allow normal sprite changes
            return;
        }
        
        // Add bounds checking to prevent index out of range errors
        if (newSpriteIndex != currentSpriteIndex && 
            newSpriteIndex >= 0 && 
            newSpriteIndex < stabilitySprites.Length && 
            stabilitySprites != null && 
            stabilitySprites.Length > 0)
        {
            currentSpriteIndex = newSpriteIndex;
            
            Debug.Log($"Changing sprite to index {newSpriteIndex}");
            
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
                Debug.Log($"Set sprite directly to {stabilitySprites[currentSpriteIndex].name}");
            }
        }
    }
    
    /// <summary>
    /// Determines which sprite index to use based on current stability
    /// </summary>
    private int GetSpriteIndexForStability(float stability)
    {
        // Check thresholds from lowest to highest to find the correct sprite
        // Thresholds: [80, 60, 40, 20, 10]
        // Sprite indices: 0=baby, 1=toddler, 2=child, 3=teen, 4=adult, 5=elderly
        
        if (stability < spriteThresholds[4]) return 5; // Below 10% - elderly
        if (stability < spriteThresholds[3]) return 4; // Below 20% - adult
        if (stability < spriteThresholds[2]) return 3; // Below 40% - teen
        if (stability < spriteThresholds[1]) return 2; // Below 60% - child
        if (stability < spriteThresholds[0]) return 1; // Below 80% - toddler
        
        return 0; // 80% or above - baby
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
    /// Handles the subtle bobble animation for the main sprite
    /// </summary>
    private void UpdateSpriteBobble()
    {
        if (mainSpriteRenderer != null && !isGameOver)
        {
            // Create a gentle bobbing motion using sine and cosine waves
            float bobbleX = Mathf.Sin(Time.time * spriteBobbleSpeed) * spriteBobbleIntensity;
            float bobbleY = Mathf.Cos(Time.time * spriteBobbleSpeed * 0.8f) * spriteBobbleIntensity * 0.6f;
            
            // Apply the bobble offset to the original position
            mainSpriteRenderer.transform.position = originalSpritePosition + new Vector3(bobbleX, bobbleY, 0);
        }
        else if (isGameOver && mainSpriteRenderer != null)
        {
            // Reset to original position when game is over
            mainSpriteRenderer.transform.position = originalSpritePosition;
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
        specialStageType = "";
        specialStageIndex = 0;
        hasInitialized = false;
        hasEggBroken = false; // Reset egg state
        hasReachedFirstMilestone = false; // Reset milestone tracker
        
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
