using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TacticalStartData
{
    public static int _gridSize;
    public static bool _isCity;
    public static int _citySize;
    public static int _maxBuildings = -1;
    public static int _spawnRadius;
    public static string _fileName;
    public static int _spearmen;
    public static int _cultists;
    public static int _templars;
    public static int _demons;
    public static int _muskets;
    public static int _kites;
    public static int _field_guns;
    public static int _infernal_engines;
    public static bool _walled;
    public static int _treeChance;
    public static int _bushChance;
    public static bool _isNight;

    public static void setGameSettingValues(string lookup, int value)
    {
        switch (lookup)
        {
            case "set-spearmen":
                _spearmen = value;
                break;
            case "set-cultists":
                _cultists = value;
                break;
            case "set-templars":
                _templars = value;
                break;
            case "set-demons":
                _demons = value;
                break;
            case "set-muskets":
                _muskets = value;
                break;
            case "set-kites":
                _kites = value;
                break;
            case "set-field-guns":
                _field_guns = value;
                break;
            case "set-infernal-engines":
                _infernal_engines = value;
                break;
            case "set-buildings":
                _maxBuildings = value;
                break;
            case "set-radius":
                _spawnRadius = value;
                break;
            case "set-grid-size":
                _gridSize = value;
                break;
            case "set-city-size":
                _citySize = value;
                break;
            case "set-tree-chance":
                _treeChance = value;
                break;
            case "set-bush-chance":
                _bushChance = value;
                break;
            default:
                Debug.LogWarning("Lookup not recognised");
                break;
        }
    }
    public static void setGameSettingValues(string lookup, string value)
    {
        switch (lookup)
        {
            case "set-map-name":
                _fileName = value;
                break;
            default:
                Debug.LogWarning("Lookup not recognised");
                break;
        }
    }
    public static void setGameSettingValues(string lookup, bool value)
    {
        switch (lookup)
        {
            case "set-city-exists":
                _isCity = value;
                break;
            case "set-walled":
                _walled = value;
                break;
            case "set-night":
                _isNight = value;
                break;
            default:
                Debug.LogWarning("Lookup not recognised");
                break;
        }
    }
}
