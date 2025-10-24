using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the upgrade system for the clicker game.
/// Upgrades unlock at specific click milestones and provide click multipliers.
/// </summary>
public class UpgradeSystem : MonoBehaviour
{
    [Header("Upgrade Settings")]
    public int[] upgradeMilestones = { 100, 1000, 10000, 100000, 1000000 };
    public int[] upgradeCosts = { 100, 100, 1000, 10000, 100000 };
    public int[] clickMultipliers = { 10, 100, 1000, 10000, 100000 };
    
    [Header("UI References")]
    public GameObject[] upgradeButtons;
    public TextMeshProUGUI[] upgradeCostTexts;
    public TextMeshProUGUI[] upgradeDescriptionTexts;
    
    // Track which upgrades have been purchased
    private bool[] upgradesPurchased;
    private int totalClickMultiplier = 1; // Base multiplier
    
    void Start()
    {
        upgradesPurchased = new bool[upgradeMilestones.Length];
        UpdateUpgradeUI();
    }
    
    void Update()
    {
        UpdateUpgradeAvailability();
    }
    
    /// <summary>
    /// Updates which upgrade buttons should be visible and available
    /// </summary>
    private void UpdateUpgradeAvailability()
    {
        for (int i = 0; i < upgradeMilestones.Length; i++)
        {
            if (upgradeButtons[i] != null)
            {
                // Show upgrade if we've reached the milestone and haven't purchased it
                bool shouldShow = Clickable.Clicks >= upgradeMilestones[i] && !upgradesPurchased[i];
                upgradeButtons[i].SetActive(shouldShow);
            }
        }
    }
    
    /// <summary>
    /// Called when an upgrade button is clicked
    /// </summary>
    /// <param name="upgradeIndex">Index of the upgrade to purchase</param>
    public void PurchaseUpgrade(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgradeMilestones.Length)
            return;
            
        // Check if we can afford this upgrade
        if (Clickable.Clicks >= upgradeCosts[upgradeIndex] && !upgradesPurchased[upgradeIndex])
        {
            // Deduct the cost
            Clickable.Clicks -= upgradeCosts[upgradeIndex];
            
            // Mark as purchased
            upgradesPurchased[upgradeIndex] = true;
            
            // Add to total multiplier
            totalClickMultiplier += clickMultipliers[upgradeIndex];
            
            // Hide the button
            if (upgradeButtons[upgradeIndex] != null)
                upgradeButtons[upgradeIndex].SetActive(false);
                
            Debug.Log($"Purchased upgrade {upgradeIndex + 1}! Total multiplier: {totalClickMultiplier}");
        }
    }
    
    /// <summary>
    /// Gets the total click multiplier from all purchased upgrades
    /// </summary>
    public int GetTotalClickMultiplier()
    {
        return totalClickMultiplier;
    }
    
    /// <summary>
    /// Updates the UI text for upgrade costs and descriptions
    /// </summary>
    private void UpdateUpgradeUI()
    {
        for (int i = 0; i < upgradeMilestones.Length; i++)
        {
            // Add bounds checking to prevent index out of range errors
            if (upgradeCostTexts != null && i < upgradeCostTexts.Length && upgradeCostTexts[i] != null)
            {
                upgradeCostTexts[i].text = $"Cost: {upgradeCosts[i]} clicks";
            }
            
            if (upgradeDescriptionTexts != null && i < upgradeDescriptionTexts.Length && upgradeDescriptionTexts[i] != null)
            {
                upgradeDescriptionTexts[i].text = $"+{clickMultipliers[i]} clicks per click";
            }
        }
    }
}
