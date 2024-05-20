using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RollDisplay : MonoBehaviour
{
    public BaseUnit parentUnit;
    public GameObject textObject;
    private TMP_Text text;

    private void Awake()
    {
        parentUnit.rollDisplay = this;
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
        textObject.transform.rotation = Camera.main.transform.rotation;
    }

    public void setText(string message)
    {
        text.text = message;
    }
}
