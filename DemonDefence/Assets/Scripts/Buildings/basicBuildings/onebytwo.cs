using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onebytwo : Building
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Initialise 1x2");
        generateRequiredTiles((2, 1), (3, 2), (0, 0));
    }
}
