using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twobytwo : Building
{
    /// <summary>
    /// 2x2 building (2 height and width)
    /// </summary>
    void Awake()
    {
        Debug.Log($"{this}[twobytwo]: Initialise 2x2");
        generateRequiredTiles((2, 2), (3, 3), (0, 0));
    }
}
