using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRegister : MonoBehaviour
{

    [SerializeField] private List<Building> buildings = new List<Building>();
    [SerializeField] private List<Building> coreBuildings = new List<Building>();

    public int get_random_building()
    {
        return UnityEngine.Random.Range(0,buildings.Count);
    }

    public Building get_specific_building(int building)
    {
        return buildings[building];
    }

    public Building getCoreBuilding(string key)
    {
        try
        {
            if (coreBuildings.Exists(x => x.buildingName.Equals(key)))
            {
                int index = coreBuildings.FindIndex(x => x.buildingName.Equals(key));
                Debug.Log(index);
                return coreBuildings[index];
            }
            else
            {
                foreach (Building building in coreBuildings) Debug.Log(building.buildingName);
                Debug.LogError($"{key} not in coreBuildings");

                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
        
    }
}
