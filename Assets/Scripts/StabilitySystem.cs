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
    public float dropAmount = 40f; // 40% drop each time
    // Removed dropProbability - now drops every time at milestones
    
    [Header("Sprite Change Thresholds")]
    public float[] spriteThresholds = { 80f, 60f, 40f, 20f }; // Below these values, change sprite
    
    [Header("UI References")]
    public Slider stabilityBar;
    public TextMeshProUGUI stabilityText;
    public Image stabilityBarFill;
    
    [Header("Sprite References")]
    public SpriteRenderer mainSpriteRenderer;
    public Sprite[] stabilitySprites; // Placeholder sprites for different stability levels
    public TransitionAnimationSystem transitionSystem; // Reference to transition animation system
    
    // Current stability value
    private float currentStability;
    private int lastMilestoneReached = 0;
    private int currentSpriteIndex = 0;
    
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
        UpdateStabilityUI();
    }
    
    void Update()
    {
        CheckForMilestoneDrops();
        UpdateWaveringAnimation();
    }
    
    /// <summary>
    /// Checks if we've reached a new click milestone and drops stability
    /// </summary>
    private void CheckForMilestoneDrops()
    {
        int currentMilestone = GetCurrentMilestone(Clickable.Clicks);
        
        // If we've reached a new milestone
        if (currentMilestone > lastMilestoneReached)
        {
            lastMilestoneReached = currentMilestone;
            
            // Drop stability EVERY TIME we reach a milestone
            DropStability();
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
    /// Drops stability by the specified amount
    /// </summary>
    private void DropStability()
    {
        currentStability -= dropAmount;
        currentStability = Mathf.Max(0f, currentStability); // Don't go below 0
        
        // Start wavering if below 100%
        if (currentStability < initialStability)
        {
            isWavering = true;
        }
        
        // Check for sprite changes
        CheckForSpriteChange();
        
        UpdateStabilityUI();
        
        Debug.Log($"Stability dropped to {currentStability}%");
    }
    
    /// <summary>
    /// Checks if we need to change the sprite based on stability thresholds
    /// </summary>
    private void CheckForSpriteChange()
    {
        int newSpriteIndex = GetSpriteIndexForStability(currentStability);
        
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
                    Debug.Log($"Triggered transition animation for sprite index {currentSpriteIndex} (stability: {currentStability}%)");
                }
            }
            else if (mainSpriteRenderer != null && stabilitySprites[currentSpriteIndex] != null)
            {
                mainSpriteRenderer.sprite = stabilitySprites[currentSpriteIndex];
                Debug.Log($"Sprite changed to index {currentSpriteIndex} (stability: {currentStability}%)");
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
                return i + 1; // Return index 1, 2, 3, 4 for thresholds
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
        if (isWavering && stabilityBar != null)
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
}
