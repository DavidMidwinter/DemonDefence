using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class UnitDisplay : MonoBehaviour
{
    /// <summary>
    /// Functionality for the display above a unit's head
    /// </summary>
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
        /// Set the threshold text
        /// Args:
        ///     string message: The  message to set
        text.text = message;
    }

    public void setHealthBar(float scale)
    {
        /// Set the health bar to a scale.
        /// Args:
        ///     float scale: The scale to set.
        healthBar.transform.localScale = new Vector3(scale, 1, 1);
    }

    public void hideHealthBar()
    {
        /// Hide the health bar
        healthDisplay.SetActive(false);
    }
    public void showHealthBar()
    {
        /// Show the health bar
        healthDisplay.SetActive(true);
    }
}
