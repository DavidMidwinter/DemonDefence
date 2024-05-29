using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class church : Building
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Initialise Church");
        generateRequiredTiles((3, 3), (6, 6), (-1, -1));
    }
}
