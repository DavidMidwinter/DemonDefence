using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganGun : BasePlayerUnit
{
    public override void applyModifiers(int move = 0, int str = 0, int tough = 0, int dmg = 0, int attack = 0, int minrange = 0, int maxrange = 0, bool indirectFire = false)
    {
        base.applyModifiers(move, str, tough, dmg, attack, minrange, maxrange, indirectFire);
    }

    public override void resetStats()
    {
        Debug.Log($"Reset Organ Gun: {this}");
        base.resetStats();
    }
}
