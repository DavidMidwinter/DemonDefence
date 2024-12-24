using System;
using UnityEngine;

[Serializable]
public class BuildingTemplate
{
    public buildingType thisType;
    public Building prefab;
    public Sprite buildingGraphic;



    public void setBrush()
    {
        BrushManager.Instance.selectedBuilding = this;
        BrushManager.Instance.state = brushState.placeBuilding;
    }
}


