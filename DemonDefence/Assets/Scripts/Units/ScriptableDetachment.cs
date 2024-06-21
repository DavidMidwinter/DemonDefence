using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Detachment")]
public class ScriptableDetachment : ScriptableObject
{
    public Faction Faction;
    public BaseUnit troopUnit;
    public int numberOfTroops;
    public BaseUnit leaderUnit;
}
