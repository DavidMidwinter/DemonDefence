using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerUnit : BaseUnit
{
    /// <summary>
    /// Contains functionality shared by all player units
    /// </summary>

    public void Awake()
    {
        selectionMarker.SetActive(false);
    }

    override public void allowAction()
    {
        /// Functionality to allow a new action to be taken
        if (UnitManager.Instance.checkRemainingPlayerActions())
        {
            GameManager.Instance.inputEnabled = true;
            TacticalUI.Instance.enableSkip();
        }
    }
    override public void blockAction()
    {
        /// Functionality to block actions from being taken.
        GameManager.Instance.inputEnabled = false;
        TacticalUI.Instance.disableSkip();

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
}
