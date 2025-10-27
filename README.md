# HorrorClicker

A Unity-based incremental clicker game with horror-themed mechanics where player success leads to character corruption and potential demise.

## 🎮 Gameplay

**HorrorClicker** is a unique twist on the classic incremental clicker genre. Players click to accumulate points, but as they progress, the game becomes increasingly unstable and glitchy, creating a horror atmosphere where success directly leads to corruption.

### Game Flow

1. **🥚 The Beginning**: Start with an egg sprite at 100% stability
2. **🎬 The Awakening**: First click triggers egg breaking animation → baby appears
3. **👶 The Innocent Stage**: Baby sprite remains stable (1-99 clicks)
4. **📈 The Progression**: At 100 clicks, stability can drop and progression begins
5. **🎯 The Horror**: Multiple branching paths with 50% chance for special game over states
6. **💀 The End**: Various endings based on progression choices

### Key Features

- **Egg Breaking Mechanic**: Interactive first click triggers animation sequence
- **Stability System**: Probabilistic drops (20%-60%) at click milestones
- **Branching Paths**: 50% chance for special endings at toddler/teen stages
- **Upgrade System**: 5 tiers with massive click multipliers
- **Visual Progression**: 6 different sprites showing character corruption
- **Horror Effects**: Glitch effects, wavering UI, color changes

## 🛠️ Setup Instructions

### Prerequisites
- Unity 2022.3 LTS or later
- TextMeshPro package (included in project)

### Installation

1. **Clone the repository**:
   ```bash
   git clone [repository-url]
   cd HorrorClicker
   ```

2. **Open in Unity**:
   - Launch Unity Hub
   - Click "Add" and select the HorrorClicker folder
   - Open the project

3. **Configure Sprites**:
   - Open the main scene
   - Select the StabilitySystem GameObject
   - In the Inspector, assign sprites to:
     - `Egg Sprite`: Initial egg sprite
     - `Stability Sprites`: Array of 6 progression sprites
     - `Necrosisteen Sprite`: Special branching sprite
     - `Pupilsfalloff Sprite`: Special branching sprite
     - `Game Over Sprite`: Final game over sprite

4. **Configure Animations**:
   - Select the TransitionAnimationSystem GameObject
   - Assign egg breaking animation sprites to `Animation 0`
   - Set `Panel Duration` (recommended: 0.5 seconds)

5. **Run the Game**:
   - Press Play in Unity
   - Click the egg to start!

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Clickable.cs              # Main clicking mechanics
│   ├── ClickTracker.cs           # UI display for clicks
│   ├── StabilitySystem.cs        # Core stability and progression system
│   ├── TransitionAnimationSystem.cs # Animation management
│   ├── UpgradeSystem.cs          # Upgrade mechanics
│   ├── GameManager.cs            # Main game coordination
│   ├── GlitchEffectController.cs # Visual effects
│   └── UI_*.cs                   # UI management scripts
├── Sprites/                      # All game sprites
├── Scenes/                       # Game scenes
├── VFX/                          # Visual effects (Kino Glitch)
└── UIResources/                  # UI assets
```

## 🎯 Game Mechanics

### Stability System
- **Initial**: 100% stability (locked until 100 clicks)
- **Drops**: Random 20%-60% at milestones (100, 1K, 10K, 100K, 1M clicks)
- **Probability**: 80% chance to drop at each milestone
- **Visual**: Bar wobbles and changes color as stability decreases

### Upgrade System
- **Tier 1**: 100 clicks → +10x multiplier (cost: 100 clicks)
- **Tier 2**: 1,000 clicks → +100x multiplier (cost: 100 clicks)
- **Tier 3**: 10,000 clicks → +1,000x multiplier (cost: 1,000 clicks)
- **Tier 4**: 100,000 clicks → +10,000x multiplier (cost: 10,000 clicks)
- **Tier 5**: 1,000,000 clicks → +100,000x multiplier (cost: 100,000 clicks)

### Branching System
- **Toddler Stage**: 50% chance → Pupils fall off → Game Over
- **Teen Stage**: 50% chance → Necrosisteen → Game Over
- **Normal Path**: Continues to final stages

## 🎨 Visual Progression

1. **EGG** → **EGG ANIMATION** → **BABY**
2. **BABY** → **BABYWITHDRAWING** → **GOODENDINGTODDLER**
3. **GOODENDINGCHILD** → **GOODENDINGTEEN** → **DEATHBYEESCAPE**

## 🔧 Customization

### Modifying Stability Drops
Edit `StabilitySystem.cs`:
```csharp
public float minDropAmount = 20f;    // Minimum drop percentage
public float maxDropAmount = 60f;    // Maximum drop percentage
public float dropProbability = 0.8f; // Chance to drop at milestones
```

### Adding New Sprites
1. Add sprites to the `stabilitySprites` array
2. Update `spriteThresholds` array to match
3. Adjust branching logic in `CheckForSpriteChange()`

### Modifying Upgrades
Edit `UpgradeSystem.cs`:
```csharp
public int[] upgradeMilestones = { 100, 1000, 10000, 100000, 1000000 };
public int[] upgradeCosts = { 100, 100, 1000, 10000, 100000 };
public int[] clickMultipliers = { 10, 100, 1000, 10000, 100000 };
```

## 🐛 Troubleshooting

### Common Issues

1. **Egg doesn't appear**: Check that `Egg Sprite` is assigned in StabilitySystem
2. **Stability starts below 100%**: Ensure `Initial Stability` is set to 100
3. **Animations don't play**: Verify `Animation 0` sprites are assigned
4. **Sprites don't change**: Check `Stability Sprites` array assignments

### Debug Mode
Enable debug mode in GameManager to see additional console output.

## 📄 License

This project is open source. Feel free to modify and distribute.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📞 Support

For issues or questions, please open an issue on GitHub.

---

**Enjoy the horror! 👻**
