using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This component supplies a public method that can be
/// called externally, such as through a UI button press.
/// 
/// Quits the game application.
/// </summary>
public class UI_Quit_Game : MonoBehaviour
{
    public void QuitGame()
    {
        #if UNITY_EDITOR
        // If running in the Unity Editor, stop playing
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // If running as a built application, quit the application
        Application.Quit();
        #endif
    }
}