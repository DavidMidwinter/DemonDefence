using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twobyone : Building
{
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Initialise 2x2");
        requiredTiles = new List<Vector2>();
        requiredTiles.Add(new Vector2(0, 0));
        requiredTiles.Add(new Vector2(0, 1));

        requiredBorderTiles = new List<Vector2>();
        requiredBorderTiles.Add(new Vector2(0, 2));
        requiredBorderTiles.Add(new Vector2(1, 2));
        requiredBorderTiles.Add(new Vector2(1, 0));
        requiredBorderTiles.Add(new Vector2(1, 1));
    }
}
