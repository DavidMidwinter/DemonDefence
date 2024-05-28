using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRegister : MonoBehaviour
{

    [SerializeField] private List<Building> buildings = new List<Building>();

    public int get_random_building()
    {
        return Random.Range(0,buildings.Count);
    }

    public Building get_specific_building(int building)
    {
        return buildings[building];
    }
}
