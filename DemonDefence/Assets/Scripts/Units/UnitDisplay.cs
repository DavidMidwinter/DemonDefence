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
    public GameObject unitNameObject;
    private TMP_Text text;
    public GameObject healthDisplay;
    private TMP_Text healthText;
    private TMP_Text unitName;

    private void Awake()
    {
        parentUnit.unitDisplay = this;
        text = textObject.GetComponent<TMP_Text>();
        healthText = healthDisplay.GetComponent<TMP_Text>();
        unitName = unitNameObject.GetComponent<TMP_Text>();
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
        hideName();
        
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

    public void setName(string message)
    {
        /// Set the name text
        /// Args:
        ///     string message: The  message to set
        unitName.text = message;
    }

    public void setHealthDisplay(int health, int maxHealth)
    {
        /// Set the health bar to a scale.
        /// Args:
        ///     float scale: The scale to set.
        healthText.text = $"{health}/{maxHealth}";
        if(health <= maxHealth / 2)
        {
            healthText.color = Color.red;
        }
        else
        {
            healthText.color = Color.green;
        }
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

    public void hideName()
    {
        unitNameObject.SetActive(false);
    }

    public void showName()
    {
        unitNameObject.SetActive(true);
    }
}
