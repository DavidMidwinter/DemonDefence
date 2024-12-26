using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public BuildingTemplate[] buildingTypes;


    public static BuildingManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public BuildingTemplate getBuilding(buildingType ident)
    {
        return buildingTypes.Where(u => u.thisType == ident).First();
    }

    public BuildingTemplate[] getAllBuildings()
    {
        return buildingTypes;
    }
}
