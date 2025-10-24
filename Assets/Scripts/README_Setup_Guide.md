# HorrorClicker Upgrade & Stability System Setup Guide

## Overview
This guide explains how to set up the new upgrade and stability systems in your HorrorClicker Unity project.

## New Scripts Added
1. **UpgradeSystem.cs** - Manages upgrade buttons and click multipliers
2. **StabilitySystem.cs** - Handles stability counter with drops and sprite changes
3. **GameManager.cs** - Coordinates all game systems
4. **PlaceholderSpriteGenerator.cs** - Creates placeholder sprites for stability levels

## Modified Scripts
1. **Clickable.cs** - Now integrates with upgrade multipliers
2. **ClickTracker.cs** - Shows click multiplier information

## Setup Instructions

### 1. Upgrade System Setup

#### In your Game Scene (02_Game.unity):

1. **Create Upgrade UI Canvas:**
   - Create a new Canvas for upgrades (or use existing UI Canvas)
   - Add a Panel for upgrade buttons

2. **Create Upgrade Buttons:**
   - Create 5 Button GameObjects as children of the upgrade panel
   - Name them: "Upgrade1", "Upgrade2", "Upgrade3", "Upgrade4", "Upgrade5"
   - Add TextMeshPro components to each button for cost and description

3. **Setup UpgradeSystem:**
   - Create an empty GameObject named "UpgradeSystem"
   - Add the UpgradeSystem script to it
   - In the inspector, assign:
     - `upgradeButtons[]` - Drag your 5 upgrade buttons
     - `upgradeCostTexts[]` - Drag the TextMeshPro components showing costs
     - `upgradeDescriptionTexts[]` - Drag the TextMeshPro components showing descriptions

4. **Configure Button Events:**
   - For each upgrade button, add an OnClick event
   - Call `UpgradeSystem.PurchaseUpgrade(0)` for button 1, `PurchaseUpgrade(1)` for button 2, etc.

### 2. Stability System Setup

#### Create Stability UI:

1. **Create Stability Bar:**
   - Create a Canvas (or use existing UI Canvas)
   - Add a Slider component for the stability bar
   - Add a TextMeshPro component for stability percentage text
   - Position it where you want the stability display

2. **Setup StabilitySystem:**
   - Create an empty GameObject named "StabilitySystem"
   - Add the StabilitySystem script to it
   - In the inspector, assign:
     - `stabilityBar` - Your stability slider
     - `stabilityText` - Your stability percentage text
     - `stabilityBarFill` - The fill image of your slider (for color changes)
     - `mainSpriteRenderer` - The SpriteRenderer you want to change based on stability

3. **Generate Placeholder Sprites:**
   - Create an empty GameObject named "SpriteGenerator"
   - Add the PlaceholderSpriteGenerator script to it
   - Run the scene to generate placeholder sprites
   - In the StabilitySystem inspector, assign the generated sprites to `stabilitySprites[]`

### 3. GameManager Setup

1. **Create GameManager:**
   - Create an empty GameObject named "GameManager"
   - Add the GameManager script to it
   - In the inspector, assign references to:
     - `upgradeSystem` - Your UpgradeSystem GameObject
     - `stabilitySystem` - Your StabilitySystem GameObject
     - `clickTracker` - Your existing ClickTracker GameObject

### 4. Testing the System

#### Upgrade System Test:
1. Start the game
2. Click to reach 100 clicks
3. First upgrade button should appear
4. Click the upgrade button to purchase it
5. Each click should now give +10 clicks instead of +1

#### Stability System Test:
1. Click to reach 100 clicks
2. There's a 1/3 chance stability drops to 90%
3. Continue clicking to reach 1,000 clicks
4. Another 1/3 chance for stability to drop to 81%
5. When stability drops below 80%, sprite should change
6. Stability bar should start wavering when below 100%

## Upgrade Milestones & Effects

| Milestone | Cost | Effect | Unlocks At |
|-----------|------|--------|------------|
| Upgrade 1 | 100 clicks | +10 clicks per click | 100 clicks |
| Upgrade 2 | 1,000 clicks | +100 clicks per click | 1,000 clicks |
| Upgrade 3 | 1,000 clicks | +1,000 clicks per click | 10,000 clicks |
| Upgrade 4 | 10,000 clicks | +10,000 clicks per click | 100,000 clicks |
| Upgrade 5 | 100,000 clicks | +100,000 clicks per click | 1,000,000 clicks |

## Stability Thresholds

| Stability Level | Sprite Index | Visual Effect |
|----------------|--------------|---------------|
| 100% - 80% | 0 (Green) | Normal |
| 80% - 60% | 1 (Yellow) | Wavering bar |
| 60% - 40% | 2 (Orange) | Wavering bar |
| 40% - 20% | 3 (Red) | Wavering bar |
| 20% - 0% | 4 (Magenta) | Wavering bar |

## Customization Options

### Modify Upgrade Values:
Edit the arrays in UpgradeSystem.cs:
- `upgradeMilestones[]` - When upgrades unlock
- `upgradeCosts[]` - Cost of each upgrade
- `clickMultipliers[]` - Click bonus from each upgrade

### Modify Stability Settings:
Edit values in StabilitySystem.cs:
- `dropAmount` - How much stability drops (default: 10%)
- `dropProbability` - Chance of drop (default: 0.33 = 1/3)
- `spriteThresholds[]` - When sprites change

### Replace Placeholder Sprites:
1. Create your own sprites in an image editor
2. Import them into Unity
3. Assign them to the `stabilitySprites[]` array in StabilitySystem

## Troubleshooting

- **Upgrades not working:** Check that UpgradeSystem is assigned in GameManager
- **Stability not dropping:** Verify StabilitySystem is assigned and active
- **Sprites not changing:** Ensure mainSpriteRenderer is assigned and sprites are in the array
- **UI not updating:** Check that all UI references are properly assigned in the inspector

## Next Steps

1. Replace placeholder sprites with actual horror-themed artwork
2. Add sound effects for upgrades and stability drops
3. Implement save/load system using GameManager.GetGameState()
4. Add visual effects for stability drops
5. Create more complex upgrade trees
