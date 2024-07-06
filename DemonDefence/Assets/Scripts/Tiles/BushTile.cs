using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushTile : GrassTile
{

    [SerializeField] private GameObject bush;

    new public void Awake()
    {
        Quaternion randomRotation = Random.rotation;
        bush.transform.rotation = new Quaternion(
            0, randomRotation.y, 0, randomRotation.w);

        float scale = Random.Range(0.8f, 1.1f);
        bush.transform.localScale *= scale;
        base.Awake();
    }
    public (Vector2 location, float rotation, float rotationW, int type) foliageInfo()
    {
        return (get2dLocation() / 10, bush.transform.rotation.y, bush.transform.rotation.w, 0);
    }
}