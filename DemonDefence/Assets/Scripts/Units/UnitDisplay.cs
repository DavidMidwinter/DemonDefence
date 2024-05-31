using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class UnitDisplay : MonoBehaviour
{
    public BaseUnit parentUnit;
    public GameObject textObject;
    private TMP_Text text;
    public GameObject healthBar;
    public GameObject healthDisplay;

    private void Awake()
    {
        parentUnit.unitDisplay = this;
        text = textObject.GetComponent<TMP_Text>();
        switch (parentUnit.faction)
        {
            case Faction.Enemy:
                text.color = Color.red;
                break;
            case Faction.Player:
                text.color = Color.blue;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(parentUnit.faction), parentUnit.faction, null);
        }
        
    }
    void FixedUpdate()
    {
        gameObject.transform.rotation = Camera.main.transform.rotation;
    }

    public void setText(string message)
    {
        text.text = message;
    }

    public void setHealthBar(float scale)
    {
        healthBar.transform.localScale = new Vector3(scale, 1, 1);
    }

    public void hideHealthBar()
    {
        healthDisplay.SetActive(false);
    }
    public void showHealthBar()
    {
        healthDisplay.SetActive(true);
    }
}
