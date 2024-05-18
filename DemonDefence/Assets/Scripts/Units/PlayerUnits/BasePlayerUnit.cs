using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerUnit : BaseUnit
{
    public GameObject selectionMarker;

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
}
