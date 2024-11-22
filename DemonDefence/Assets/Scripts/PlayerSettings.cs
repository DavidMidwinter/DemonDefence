using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerSettings
{
    public static Dictionary<string, int> defaultInts = new Dictionary<string, int>
    {
        { "volume", 100 }
    };
    public static int getPref(string key)
    {
        if (!defaultInts.ContainsKey(key))
        {
            Debug.LogWarning("Setting does not exist");
            return 0;
        }


        if (!PlayerPrefs.HasKey(key))
        {
            setPref(key, defaultInts[key]);
        }

        return PlayerPrefs.GetInt(key);
    }

    public static void setPref(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
}
