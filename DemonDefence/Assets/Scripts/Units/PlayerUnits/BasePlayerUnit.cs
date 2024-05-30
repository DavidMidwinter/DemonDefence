using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerUnit : BaseUnit
{

    public void Awake()
    {
        Debug.Log($"{this} is Awake");
        selectionMarker.SetActive(false);
    }

    override public void allowAction()
    {
        if (UnitManager.Instance.checkRemainingPlayerActions())
        {
            GameManager.Instance.inputEnabled = true;
        }
    }
    override public void blockAction()
    {
        GameManager.Instance.inputEnabled = false;
    }

    override public void takeAction()
    {
        remainingActions -= 1;
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
        setHealthBar();
    }
}
