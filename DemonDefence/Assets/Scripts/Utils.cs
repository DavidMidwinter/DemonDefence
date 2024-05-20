using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int rollDice()
    {
        int result = Random.Range(1, 11);
        TacticalUI.Instance.setCardText($" {result} ");
        return result;
    }

    public static int calculateThreshold(int strength, int toughness)
    {
        /// Calculate the roll threshold.
        /// First, set threshold to 5. Rolls must exceed this.
        /// Subtract 'strength' from 'toughness' and add the difference to threshold.
        /// If threshold exceeds 9, set to 9; if it subceeds 1, set to 1.
        /// Return threshold.

        int threshold = 5;

        int diff = toughness - strength;

        threshold += diff;
        if (threshold > 9) threshold = 9;
        else if (threshold < 1) threshold = 1;

        return threshold;
    }
}
