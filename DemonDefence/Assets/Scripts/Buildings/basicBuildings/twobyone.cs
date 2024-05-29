using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twobyone : Building
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Initialise 2x1");
        generateRequiredTiles((1, 2), (2, 3), (0, 0));
    }
}
