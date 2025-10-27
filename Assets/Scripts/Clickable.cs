using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{

    // This variable is marked as `static`, which means it is
    // accessible by any script, anywhere! They can just write
    // Clickable.Clicks to read/write to this value.
    //
    // See an example in `ClickTracker.cs`, which is on a UI
    // object titled `Tracker Text (TMP)`.
    public static int Clicks = 0;

    // Reference to the upgrade system to get click multipliers
    private UpgradeSystem upgradeSystem;

    void Start()
    {
        // Find the upgrade system in the scene
        upgradeSystem = FindObjectOfType<UpgradeSystem>();
    }

    /// <summary>
    /// This function is called when the mouse button clicks
    /// on this object.
    /// </summary>
    private void OnMouseDown()
    {
        // Check if game is over (prevent clicking)
        StabilitySystem stabilitySystem = FindObjectOfType<StabilitySystem>();
        if (stabilitySystem != null && stabilitySystem.IsGameOver())
        {
            return; // Don't allow clicking when game is over
        }
        
        // Check if this is the first click (egg breaking)
        if (Clicks == 0 && stabilitySystem != null)
        {
            stabilitySystem.BreakEgg();
        }
        
        int clickValue = 1; // Base click value
        
        // Apply upgrade multipliers if upgrade system exists
        if (upgradeSystem != null)
        {
            clickValue = upgradeSystem.GetTotalClickMultiplier();
        }
        
        Clicks += clickValue;  // add points based on upgrades
    }

}
