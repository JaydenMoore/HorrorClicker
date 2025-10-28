using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClickTracker : MonoBehaviour
{

    TextMeshProUGUI _trackingText;
    private UpgradeSystem upgradeSystem;

    /// <summary>
    /// Awake is run before the Start() method, usually used for
    /// initializing values.
    /// </summary>
    private void Awake()
    {
        _trackingText = GetComponent<TextMeshProUGUI>();
    }
    
    void Start()
    {
        upgradeSystem = FindObjectOfType<UpgradeSystem>();
    }

    public void Update()
    {
        // See how many clicks the player has, stored in the static
        // variable `Clickable.Clicks`.
        string displayText = "Observations: " + Clickable.Clicks;
        
        // Add click multiplier info if upgrades are available
        if (upgradeSystem != null && upgradeSystem.GetTotalClickMultiplier() > 1)
        {
            displayText += "\nResearching Efficiency: x" + upgradeSystem.GetTotalClickMultiplier();
        }
        
        _trackingText.text = displayText;
    }

}
