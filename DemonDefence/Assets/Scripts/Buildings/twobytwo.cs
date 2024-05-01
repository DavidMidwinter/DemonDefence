using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twobytwo : Building
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Initialise 2x2");
        requiredTiles = new List<Vector2>();
        requiredTiles.Add(new Vector2(0, 0));
        requiredTiles.Add(new Vector2(0, 1));
        requiredTiles.Add(new Vector2(1, 0));
        requiredTiles.Add(new Vector2(1, 1));
    }
}
