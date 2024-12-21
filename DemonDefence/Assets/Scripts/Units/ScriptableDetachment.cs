using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Detachment", menuName = "Scriptable Detachment")]
public class ScriptableDetachment : ScriptableObject
{
    public string unitName;
    public Faction Faction;
    public ScriptableUnit troopUnit;
    public int numberOfTroops;
    public ScriptableUnit leaderUnit;
}
