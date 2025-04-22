using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGuns : BasePlayerUnit
{

    public override void applyModifiers(int move = 0, int str = 0, int tough = 0, int dmg = 0, int attack = 0, int minrange = 0, int maxrange = 0, bool indirectFire = false)
    {
        if (indirectFire)
        {
            fireAnimationEvent(animations.SecondMode);
        }
        base.applyModifiers(move, str, tough, dmg, attack, minrange, maxrange, indirectFire);
    }

    public override void resetStats()
    {
        Debug.Log($"{this}[Field Gun]: Reset Field Gun");
        if(canAttackIndirect)
            fireAnimationEvent(animations.LeaveSecondMode);
        base.resetStats();
    }
}
