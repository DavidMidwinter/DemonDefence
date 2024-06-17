using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRegister : MonoBehaviour
{
    /// <summary>
    /// Class holding all building objects
    /// </summary>

    [SerializeField] private List<Building> buildings = new List<Building>();
    [SerializeField] private List<Building> coreBuildings = new List<Building>();

    public int get_random_building()
    {
        /// Get a random building index from the buildings list
        return UnityEngine.Random.Range(0,buildings.Count);
    }

    public Building get_specific_building(int building)
    {
        /// Get a building from the buildings list by an index
        /// Args:
        ///     int building: The building index
        /// Returns:
        ///     Building: The building at the index.
        return buildings[building];
    }

    public Building getCoreBuilding(string key)
    {
        /// Get a building from the Core Building list
        /// Args:
        ///     string key: Key for the building to get
        /// Returns:
        ///     Building: The building being requested, null if not exists
        return (get_building_by_key(coreBuildings, key));

    }
    public Building get_specific_building_by_key(string key)
    {
        /// Get a building from the Building list
        /// Args:
        ///     string key: Key for the building to get
        /// Returns:
        ///     Building: The building being requested, null if not exists
        return (get_building_by_key(buildings, key));

    }

    public Building get_building_by_key(List<Building> building_list, string key)
    {
        /// Get a building from a list by a given key
        /// Args:
        ///     List<Building> building_list: The building list to pull from
        ///     string key: The name of the building to pull
        /// Returns:
        ///     Building: The building being requested; null if it does not exist.
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
