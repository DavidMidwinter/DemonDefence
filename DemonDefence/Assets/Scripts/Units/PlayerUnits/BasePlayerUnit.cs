using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BasePlayerUnit : BaseUnit
{
    /// <summary>
    /// Contains functionality shared by all player units
    /// </summary>
    /// 
    [HideInInspector] public List<BaseEnemyUnit> validTargets;
    [HideInInspector] public List<BasePlayerUnit> aura;

    public void Awake()
    {
        aura = new List<BasePlayerUnit>();
        selectionMarker.SetActive(false);
        validTargets = new List<BaseEnemyUnit>();
        initOutline(Color.blue);
        UnitManager.Instance.resetPlayers += resetStats;

    }

    new public void OnDestroy()
    {

        UnitManager.Instance.resetPlayers -= resetStats;
        base.OnDestroy();
    }
    override public void allowAction()
    {
        /// Functionality to allow a new action to be taken
        /// 
        if (getRemainingActions() <= 0)
        {
            UnitManager.Instance.setNextPlayer(this);
            return;
        }
        GameManager.Instance.inputEnabled = true;
        TacticalUI.Instance.enableSkip();
        checkCanAttack();
        calculateAllTilesInRange();
        getAttackTargets();
        base.allowAction();
    }
    override public void blockAction()
    {
        /// Functionality to block actions from being taken.
        GameManager.Instance.inputEnabled = false;
        TacticalUI.Instance.disableSkip();
        clearTargets();
        base.blockAction();

    }

    public override void onSelect()
    {
        TacticalUI.Instance.clearActions();
        TacticalUI.Instance.enableSkip();
        GameManager.Instance.inputEnabled = true;
        base.onSelect();
    }
    override public void takeAction(int actions = 1)
    {
        /// Functionality when actions are taken.
        /// Args:
        ///     int actions: The number of action points an action will take; default 0
        remainingActions -= actions;
    }

    public void Update()
    {
        if (UnitManager.Instance.SelectedEnemy
             && UnitManager.Instance.SelectedEnemy.target == this
            && UnitManager.Instance.SelectedEnemy.attacking)
        {
            int roll = Utils.calculateThreshold(UnitManager.Instance.SelectedEnemy.getStrength(this), getToughness());
            unitDisplay.setText($"{roll}+");
        }
        else
        {
            unitDisplay.setText(null);
        }
    }

    public void getAttackTargets()
    {
        /// Populate the target list
        clearTargets();
        foreach(BaseEnemyUnit enemy in UnitManager.Instance.enemyUnits) {
            if (checkRange(enemy))
            {
                validTargets.Add(enemy);
            }
        
        }
    }

    public void clearTargets()
    {
        /// Clear the target list
        validTargets.Clear();
    }

    public void getAffected(int range)
    {
        if (detachmentMembers != null)
            aura = UnitManager.Instance.allyUnits.FindAll(u => detachmentMembers.Contains(u)
      && getDistance(u) <= range * 10);
        else aura = new List<BasePlayerUnit>();
        aura.Add(this);
    }

    public void addAbilityButton(string name, Action function)
    {
        TacticalUI.Instance.addAction(name, function);
    }


}
