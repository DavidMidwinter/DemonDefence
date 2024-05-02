using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit unitPrefab;
}

public enum Faction
{
    Player = 0,
    Enemy = 1
}
