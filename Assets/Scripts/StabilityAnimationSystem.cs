using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Alternative stability system that uses animations instead of static sprites
/// </summary>
public class StabilityAnimationSystem : MonoBehaviour
{
    [Header("Stability Settings")]
    public float initialStability = 100f;
    public float dropAmount = 40f; // 40% drop each time
    
    [Header("Animation References")]
    public Animator stabilityAnimator;
    public string[] animationTriggers = {
        "Stability100",    // 100% stability
        "Stability80",     // Below 80%
        "Stability60",     // Below 60%
        "Stability40",     // Below 40%
        "Stability20"      // Below 20%
    };
    
    [Header("UI References")]
    public UnityEngine.UI.Slider stabilityBar;
    public TMPro.TextMeshProUGUI stabilityText;
    public UnityEngine.UI.Image stabilityBarFill;
    
    // Current stability value
    private float currentStability;
    private int lastMilestoneReached = 0;
    private int currentAnimationIndex = 0;
    
    // Wavering animation
    private bool isWavering = false;
    private Vector3 originalBarPosition;
    private float waverIntensity = 5f;
    private float waverSpeed = 15f;
    private float wobbleRange = 3f;
    
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
        
        if (currentMilestone > lastMilestoneReached)
        {
            lastMilestoneReached = currentMilestone;
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
        currentStability = Mathf.Max(0f, currentStability);
        
        if (currentStability < initialStability)
        {
            isWavering = true;
        }
        
        CheckForAnimationChange();
        UpdateStabilityUI();
        
        Debug.Log($"Stability dropped to {currentStability}%");
    }
    
    /// <summary>
    /// Checks if we need to change the animation based on stability thresholds
    /// </summary>
    private void CheckForAnimationChange()
    {
        int newAnimationIndex = GetAnimationIndexForStability(currentStability);
        
        if (newAnimationIndex != currentAnimationIndex && 
            newAnimationIndex >= 0 && 
            newAnimationIndex < animationTriggers.Length && 
            stabilityAnimator != null)
        {
            currentAnimationIndex = newAnimationIndex;
            stabilityAnimator.SetTrigger(animationTriggers[currentAnimationIndex]);
            Debug.Log($"Animation changed to {animationTriggers[currentAnimationIndex]} (stability: {currentStability}%)");
        }
    }
    
    /// <summary>
    /// Determines which animation to trigger based on current stability
    /// </summary>
    private int GetAnimationIndexForStability(float stability)
    {
        if (stability < 20f) return 4;  // Stability20
        if (stability < 40f) return 3;  // Stability40
        if (stability < 60f) return 2;  // Stability60
        if (stability < 80f) return 1;  // Stability80
        return 0; // Stability100
    }
    
    /// <summary>
    /// Updates the stability bar and text UI
    /// </summary>
    private void UpdateStabilityUI()
    {
        if (stabilityBar != null)
        {
            stabilityBar.value = currentStability;
        }
        
        if (stabilityText != null)
        {
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
            float wobbleValue = Mathf.Sin(Time.time * waverSpeed) * wobbleRange;
            float wobbledValue = currentStability + wobbleValue;
            wobbledValue = Mathf.Clamp(wobbledValue, 0f, initialStability);
            stabilityBar.value = wobbledValue;
            
            float waverX = Mathf.Sin(Time.time * waverSpeed * 0.5f) * waverIntensity;
            float waverY = Mathf.Cos(Time.time * waverSpeed * 0.3f) * waverIntensity * 0.5f;
            stabilityBar.transform.position = originalBarPosition + new Vector3(waverX, waverY, 0);
            
            if (stabilityBarFill != null)
            {
                Color barColor = Color.Lerp(Color.red, Color.green, currentStability / initialStability);
                stabilityBarFill.color = barColor;
            }
        }
    }
    
    public float GetCurrentStability()
    {
        return currentStability;
    }
}
