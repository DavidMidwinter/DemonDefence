using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerUnit : BaseUnit
{
    public GameObject selectionMarker;

    private void Awake()
    {
        selectionMarker.SetActive(false);
    }
}
