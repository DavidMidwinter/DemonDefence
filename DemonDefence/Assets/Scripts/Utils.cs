using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static int minimumThreshold = 2;
    private static int maximumThreshold = 10;
    private static int defaultThreshold = 6;

    public static int rollDice()
    {
        /// Get a random roll from a D10
        /// Returns:
        ///     int result: The result of the roll.
        int result = Random.Range(1, 11);
        return result;
    }

    public static int calculateThreshold(int strength, int toughness)
    {
        /// Calculate the roll threshold for an attack.
        /// 
        /// Args:
        ///     int strength: The strength of the attack
        ///     int toughness: The toughness of the target
        /// Returns:
        ///     threshold: The calculated threshold
        /// 
        /// First, set threshold to 6. Rolls must equal or exceed this.
        /// Subtract 'strength' from 'toughness' and add the difference to threshold.
        /// If threshold exceeds 10, set to 10; if it subceeds 2, set to 2.
        /// Return threshold.

        int threshold = defaultThreshold;

        int diff = toughness - strength;

        threshold += diff;
        if (threshold > maximumThreshold) threshold = maximumThreshold;
        else if (threshold < minimumThreshold) threshold = minimumThreshold;

        return threshold;
    }
    public static float calculateDistance(Vector3 origin, Vector3 target)
    {
        /// Calculate the distance between two 3D points
        /// Args:
        ///     Vector2 origin: Start point
        ///     Vector2 target: End point
        /// Returns:
        ///     float: The distance
        ///     
        return (origin - target).magnitude;
    }
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

    public static Vector3 calculateBearing(Vector3 origin, Vector3 target)
    {
        /// Calculate the distance between two 2D points
        /// Args:
        ///     Vector2 origin: Start point
        ///     Vector2 target: End point
        /// Returns:
        ///     float: The distance
        ///     
        return (target - origin).normalized;

    }

    public static float absoluteDiff(float x, float y)
    {
        return Mathf.Sqrt(Mathf.Pow(x - y, 2));
    }

    public static Vector3 calculateMidpoint(Vector3 a, Vector3 b)
    {
        /// Gets the midpoint between two vectors
        /// Args:
        ///     Vector3 a: First vector
        ///     Vector3 b: Second vector
        /// Returns:
        ///     Vector3: The midpoint between a and b
        return new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2);
    }

    public static Vector3 getSmallerVector(Vector3 a, Vector3 b)
    {
        return (a.magnitude >= b.magnitude) ? b : a;
    }
   
}
