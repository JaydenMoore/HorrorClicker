using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;

public class GlitchEffectController : MonoBehaviour
{
    [Header("Glitch Settings")]
    [SerializeField] private float glitchInterval = 5f; // Time between glitches
    [SerializeField] private float glitchDuration = 0.5f; // How long the glitch lasts
    [SerializeField] private float glitchIntensity = 1f; // Maximum intensity of the glitch
    
    private DigitalGlitch digitalGlitch;
    private AnalogGlitch analogGlitch;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get the glitch components (try both types)
        digitalGlitch = GetComponent<DigitalGlitch>();
        analogGlitch = GetComponent<AnalogGlitch>();
        
        // Start the glitch coroutine
        StartCoroutine(GlitchRoutine());
    }
    
    IEnumerator GlitchRoutine()
    {
        while (true)
        {
            // Wait for the interval
            yield return new WaitForSeconds(glitchInterval);
            
            // Trigger the glitch
            if (digitalGlitch != null)
            {
                float originalIntensity = digitalGlitch.intensity;
                digitalGlitch.intensity = glitchIntensity;
                yield return new WaitForSeconds(glitchDuration);
                digitalGlitch.intensity = originalIntensity;
            }
            
            if (analogGlitch != null)
            {
                float originalScanLine = analogGlitch.scanLineJitter;
                float originalVertical = analogGlitch.verticalJump;
                float originalHorizontal = analogGlitch.horizontalShake;
                float originalColor = analogGlitch.colorDrift;
                
                analogGlitch.scanLineJitter = glitchIntensity;
                analogGlitch.verticalJump = glitchIntensity;
                analogGlitch.horizontalShake = glitchIntensity;
                analogGlitch.colorDrift = glitchIntensity;
                
                yield return new WaitForSeconds(glitchDuration);
                
                analogGlitch.scanLineJitter = originalScanLine;
                analogGlitch.verticalJump = originalVertical;
                analogGlitch.horizontalShake = originalHorizontal;
                analogGlitch.colorDrift = originalColor;
            }
        }
    }
}
