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
        int result = Random.Range(1, 11);
        TacticalUI.Instance.setCardText($" {result} ");
        return result;
    }

    public static int calculateThreshold(int strength, int toughness)
    {
        /// Calculate the roll threshold.
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
}
