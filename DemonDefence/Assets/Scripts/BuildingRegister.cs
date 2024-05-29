using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRegister : MonoBehaviour
{

    [SerializeField] private List<Building> buildings = new List<Building>();
    [SerializeField] private Dictionary<string, Building> coreBuildings = new Dictionary<string, Building>();

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
            return coreBuildings[key];
        }
        catch (ArgumentOutOfRangeException e) {
            Debug.LogError(e.Message);
            return null;
        }
    }
}
