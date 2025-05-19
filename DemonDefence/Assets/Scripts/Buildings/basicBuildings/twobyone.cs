using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twobyone : Building
{
    /// <summary>
    /// 2x1 building (2 height, 1 width)
    /// </summary>
    void Awake()
    {
        Debug.Log($"{this}[twobyone]: Initialise 2x1");
        generateRequiredTiles((1, 2), (2, 3), (0, 0));
    }
}
