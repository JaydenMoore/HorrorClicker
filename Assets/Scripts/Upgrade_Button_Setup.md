# Quick Upgrade Button Setup

## The Issue
The upgrade buttons aren't showing because the `UpgradeSystem` script is looking for UI buttons that don't exist yet.

## Quick Fix (2 minutes):

### Step 1: Create UI Canvas
1. **Right-click in Hierarchy** → UI → Canvas
2. This creates a Canvas for UI elements

### Step 2: Create Upgrade Buttons
1. **Right-click on Canvas** → UI → Button
2. **Rename it** to "Upgrade1"
3. **Repeat 4 more times** to create "Upgrade2", "Upgrade3", "Upgrade4", "Upgrade5"
4. **Position them** where you want (maybe in a column on the right side)

### Step 3: Connect to Script
1. **Select your UpgradeSystem GameObject** (the one with the UpgradeSystem script)
2. **In the Inspector**, find the "Upgrade Buttons" array
3. **Set Size to 5**
4. **Drag each button** from Hierarchy to the array slots:
   - Element 0: Upgrade1
   - Element 1: Upgrade2
   - Element 2: Upgrade3
   - Element 3: Upgrade4
   - Element 4: Upgrade5

### Step 4: Add Button Click Events
For each upgrade button:
1. **Select the button** in Hierarchy
2. **In Inspector**, find "On Click ()" section
3. **Click the + button** to add an event
4. **Drag your UpgradeSystem GameObject** to the object field
5. **Select function**: UpgradeSystem → PurchaseUpgrade
6. **Set the parameter**:
   - Upgrade1: 0
   - Upgrade2: 1
   - Upgrade3: 2
   - Upgrade4: 3
   - Upgrade5: 4

## Alternative: Test Without UI
If you want to test the logic without UI first:

1. **Open the Console** (Window → General → Console)
2. **Click to reach 100 clicks**
3. **In the UpgradeSystem script**, temporarily add this line in the `UpdateUpgradeAvailability()` method:
   ```csharp
   Debug.Log($"Clicks: {Clickable.Clicks}, Should show upgrade 1: {Clickable.Clicks >= 100}");
   ```
4. **Check console** - you should see the upgrade logic working

## What Should Happen:
- At 100 clicks: Upgrade1 button appears
- Click Upgrade1: Costs 100 clicks, gives +10 clicks per click
- At 1,000 clicks: Upgrade2 button appears
- And so on...

The buttons are hidden by default and only show when you reach the milestone AND haven't purchased them yet.
