using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onebytwo : Building
{
    /// <summary>
    /// 1x2 building (1 height, 2 width)
    /// </summary>
    void Awake()
    {
        Debug.Log("Initialise 1x2");
        generateRequiredTiles((2, 1), (3, 2), (0, 0));
    }
}
