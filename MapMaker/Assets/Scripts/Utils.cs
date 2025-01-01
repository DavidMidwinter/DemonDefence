using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class Utils
{
    public static string saveDirectory = Path.Combine(Application.dataPath, "Maps");
    public static float calculateDistance(Vector2 origin, Vector2 target)
    {
        /// Calculate the distance between two 2D points
        /// Args:
        ///     Vector2 origin: Start point
        ///     Vector2 target: End point
        /// Returns:
        ///     float: The distance
        ///     
        return (origin - target).magnitude;
    }

    public static void loadEditor()
    {
        SceneManager.LoadScene("MapMaker");

    }
    public static void returnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static void exit()
    {
        Application.Quit();
    }
}
