using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twobytwo : Building
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Initialise 2x2");
        generateRequiredTiles((2, 2), (3, 3), (0, 0));
    }
}
