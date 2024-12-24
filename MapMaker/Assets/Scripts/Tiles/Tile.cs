using System;
using UnityEngine;

[Serializable]
public class Tile
{
    public tileType thisType;
    public Sprite tileGraphic;
    public bool canSupportBuilding;

    public void setBrush()
    {
        BrushManager.Instance.selectedTile = this;
    }
}


