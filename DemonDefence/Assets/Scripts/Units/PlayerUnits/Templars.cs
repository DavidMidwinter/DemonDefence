using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Templars : BasePlayerUnit
{
    [SerializeField] Priest leader;

    public override void setLeader(BaseUnit unit = null)
    {
        if (unit.GetType() == typeof(Priest))
        {
            leader = (Priest) unit;
        }
    }

    public override void onSelect()
    {
        base.onSelect();
        if(leader != null)
        {
            leader.passiveStrength();
        }
    }
}
