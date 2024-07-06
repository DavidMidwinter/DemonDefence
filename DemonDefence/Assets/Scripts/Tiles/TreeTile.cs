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

        float scale = Random.Range(1, 1.5f);
        tree.transform.localScale *= scale;
        base.Awake();
    }

    public (Vector2 location, float rotation, float rotationW, int type) foliageInfo()
    {
        return (get2dLocation() / 10, tree.transform.rotation.y, tree.transform.rotation.w, 0);
    }
}
