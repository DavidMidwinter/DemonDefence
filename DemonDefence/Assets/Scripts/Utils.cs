using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int rollDice()
    {
        int result = Random.Range(1, 10);
        TacticalUI.Instance.setCardText($" {result} ");
        return result;
    }
}
