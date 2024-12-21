using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public string unitName;
    public BaseUnit unitPrefab;
    public Texture2D unitImage;
    public string[] abilities;

}
