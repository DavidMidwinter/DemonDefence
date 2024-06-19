using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerUnit : BaseUnit
{
    /// <summary>
    /// Contains functionality shared by all player units
    /// </summary>
    /// 
    public List<BaseEnemyUnit> validTargets;

    public void Awake()
    {
        selectionMarker.SetActive(false);
        validTargets = new List<BaseEnemyUnit>();
    }

    override public void allowAction()
    {
        /// Functionality to allow a new action to be taken
        if (UnitManager.Instance.checkRemainingPlayerActions())
        {
            GameManager.Instance.inputEnabled = true;
            TacticalUI.Instance.enableSkip();
            getAttackTargets();
        }
    }
    override public void blockAction()
    {
        /// Functionality to block actions from being taken.
        GameManager.Instance.inputEnabled = false;
        TacticalUI.Instance.disableSkip();
        clearTargets();

    }

    override public void takeAction(int actions = 1)
    {
        /// Functionality when actions are taken.
        /// Args:
        ///     int actions: The number of action points an action will take; default 0
        remainingActions -= actions;
        calculateAllTilesInRange();
        if (getRemainingActions() <= 0)
        {
            UnitManager.Instance.SetSelectedHero(null);
            return;
        }
    }

    public void Update()
    {
        if (UnitManager.Instance.SelectedEnemy
             && UnitManager.Instance.SelectedEnemy.target == this
            && UnitManager.Instance.SelectedEnemy.attacking)
        {
            int roll = Utils.calculateThreshold(UnitManager.Instance.SelectedEnemy.strength, toughness);
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

    override protected void GameManagerStateChanged(GameState state)
    {
        if (state == GameState.PlayerTurn)
        {
            resetModifiers();
        }
    }
}
