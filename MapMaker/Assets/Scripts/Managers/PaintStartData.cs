using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PaintStartData
{
    static string filename;
    static int gridSize;
    public static void setPaintSettingValues(string lookup, int value)
    {
        switch (lookup)
        {
            
            case "set-grid-size":
                gridSize = value;
                break;
            default:
                Debug.LogWarning("Lookup not recognised");
                break;
        }
    }
    public static void setPaintSettingValues(string lookup, string value)
    {
        switch (lookup)
        {
            case "set-map-name":
                filename = value;
                break;
            default:
                Debug.LogWarning("Lookup not recognised");
                break;
        }
    }

    public static int getGridSize()
    {
        return gridSize;
    }

    public static string getFilename()
    {
        if (filename == "") return null;
        return filename;
    }
}
