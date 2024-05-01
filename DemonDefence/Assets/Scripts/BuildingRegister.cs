using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRegister : MonoBehaviour
{

    [SerializeField] private List<Building> buildings = new List<Building>();

    public Building get_random_building()
    {
        return buildings[0];
    }
}
