using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{

    public List<Vector2> tiles;
    protected List<Vector2> requiredTiles;
    public Vector2 origin;

}
