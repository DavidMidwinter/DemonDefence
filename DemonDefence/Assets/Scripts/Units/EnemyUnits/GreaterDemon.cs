using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreaterDemon : BaseEnemyUnit
{
    /// Functionality unique to the Greater Demon unit
    /// 

    public override void addDetachmentMember(BaseUnit unit)
    {
        base.addDetachmentMember(unit);
        unit.setLeader(this);
    }

    public override void onDeath()
    {
        foreach (BaseUnit unit in detachmentMembers) unit.setLeader();
        base.onDeath();
    }
}
