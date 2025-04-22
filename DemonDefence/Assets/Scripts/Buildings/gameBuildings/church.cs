using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class church : Building
{
    /// <summary>
    /// Church building (3 height and width, borders on all sides)
    /// </summary>
    void Awake()
    {
        Debug.Log($"{this}[church]: Initialise Church");
        generateRequiredTiles((3, 3), (6, 6), (-1, -1));
    }
}
