using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Detachment")]
public class ScriptableDetachment : ScriptableObject
{
    public string unitName;
    public Faction Faction;
    public BaseUnit troopUnit;
    public int numberOfTroops;
    public Texture2D troopImage;
    public BaseUnit leaderUnit;
    public Texture2D leaderImage;
    public string[] leaderAbilities;
}
