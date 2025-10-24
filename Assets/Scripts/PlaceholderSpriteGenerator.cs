using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility script to create placeholder sprites for the stability system.
/// This creates simple colored squares that can be replaced with actual art later.
/// </summary>
public class PlaceholderSpriteGenerator : MonoBehaviour
{
    [Header("Placeholder Settings")]
    public int spriteSize = 64;
    public Color[] stabilityColors = {
        Color.green,    // 100% - Full stability
        Color.yellow,   // 80% - First threshold
        new Color(1f, 0.5f, 0f),   // 60% - Second threshold (orange)
        Color.red,      // 40% - Third threshold
        Color.magenta   // 20% - Fourth threshold
    };
    
    [Header("Output")]
    public Sprite[] generatedSprites;
    
    void Start()
    {
        GeneratePlaceholderSprites();
    }
    
    /// <summary>
    /// Generates placeholder sprites for different stability levels
    /// </summary>
    public void GeneratePlaceholderSprites()
    {
        generatedSprites = new Sprite[stabilityColors.Length];
        
        for (int i = 0; i < stabilityColors.Length; i++)
        {
            generatedSprites[i] = CreateColoredSprite(stabilityColors[i], $"StabilitySprite_{i}");
        }
        
        Debug.Log($"Generated {generatedSprites.Length} placeholder sprites for stability system");
    }
    
    /// <summary>
    /// Creates a simple colored sprite
    /// </summary>
    private Sprite CreateColoredSprite(Color color, string name)
    {
        // Create a texture
        Texture2D texture = new Texture2D(spriteSize, spriteSize);
        
        // Fill with color
        Color[] pixels = new Color[spriteSize * spriteSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Create sprite from texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, spriteSize, spriteSize), new Vector2(0.5f, 0.5f));
        sprite.name = name;
        
        return sprite;
    }
    
    /// <summary>
    /// Gets the generated sprites array
    /// </summary>
    public Sprite[] GetGeneratedSprites()
    {
        return generatedSprites;
    }
}
