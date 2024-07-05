using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTile : GrassTile
{
    [SerializeField] private GameObject tree;

    new public void Awake()
    {
        Quaternion randomRotation = Random.rotation;
        tree.transform.rotation = new Quaternion(
            0, randomRotation.y, 0, randomRotation.w);
        base.Awake();
    }
}
