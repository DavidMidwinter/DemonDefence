using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
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
}
