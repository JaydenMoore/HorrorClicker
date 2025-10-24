# Quick Test Setup for Unity

## Minimal Setup to Test the Systems

### 1. **Basic Test (No UI Required)**
You can test the core logic immediately by:

1. **Open your game scene** (02_Game.unity)
2. **Add the scripts to empty GameObjects:**
   - Create empty GameObject → Add `UpgradeSystem` script
   - Create empty GameObject → Add `StabilitySystem` script  
   - Create empty GameObject → Add `GameManager` script
3. **Run the scene** and start clicking!

**What will work:**
- ✅ Click counting with multipliers
- ✅ Upgrade purchasing (via console logs)
- ✅ Stability drops (via console logs)
- ✅ Sprite changes (if you assign a SpriteRenderer)

**What won't work yet:**
- ❌ Upgrade buttons (no UI)
- ❌ Stability bar display (no UI)
- ❌ Visual feedback

### 2. **Quick UI Setup (5 minutes)**

#### For Upgrade System:
1. **Create Canvas** (right-click → UI → Canvas)
2. **Create 5 Buttons** (right-click on Canvas → UI → Button)
3. **Position buttons** where you want them
4. **In UpgradeSystem inspector:**
   - Drag the 5 buttons to `upgradeButtons[]` array
   - Leave `upgradeCostTexts[]` and `upgradeDescriptionTexts[]` empty for now

#### For Stability System:
1. **Create Slider** (right-click on Canvas → UI → Slider)
2. **Create Text** (right-click on Canvas → UI → Text - TextMeshPro)
3. **In StabilitySystem inspector:**
   - Drag slider to `stabilityBar`
   - Drag text to `stabilityText`
   - Leave other fields empty for now

### 3. **Test Scenarios**

#### Test Upgrade System:
1. **Click to reach 100 clicks**
2. **Check console** - should see "Purchased upgrade 1!" when you click upgrade button
3. **Continue clicking** - each click should now give +10 clicks
4. **Reach 1,000 clicks** - second upgrade should appear

#### Test Stability System:
1. **Click to reach 100 clicks**
2. **Check console** - should see "Stability dropped to 90%" (1/3 chance)
3. **Continue clicking** - stability should drop at each milestone
4. **Check stability bar** - should show decreasing values

### 4. **Console Debugging**

Watch the console for these messages:
- `"Purchased upgrade X! Total multiplier: Y"`
- `"Stability dropped to X%"`
- `"Sprite changed to index X (stability: Y%)"`

### 5. **Quick Fixes if Issues**

#### If upgrades don't work:
- Check that UpgradeSystem GameObject is active
- Verify upgrade buttons are assigned in inspector

#### If stability doesn't drop:
- Check that StabilitySystem GameObject is active
- Look for console messages about stability drops

#### If sprites don't change:
- Assign a SpriteRenderer to `mainSpriteRenderer` in StabilitySystem
- Add placeholder sprites to `stabilitySprites[]` array

## Ready to Test!

The core systems will work immediately once you add the scripts to GameObjects. The UI is just for visual feedback - the logic runs independently.

**Start with the basic test first** to verify the systems work, then add UI elements as needed.
