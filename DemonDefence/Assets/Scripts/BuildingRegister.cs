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
        return (get_building_by_key(coreBuildings, key));

    }
    public Building get_specific_building_by_key(string key)
    {
        return (get_building_by_key(buildings, key));

    }

    public Building get_building_by_key(List<Building> building_list, string key)
    {
        try
        {
            if (building_list.Exists(x => x.buildingName.Equals(key)))
            {
                int index = building_list.FindIndex(x => x.buildingName.Equals(key));
                Debug.Log(index);
                return building_list[index];
            }
            else
            {
                Debug.LogError($"{key} does not exist");
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
