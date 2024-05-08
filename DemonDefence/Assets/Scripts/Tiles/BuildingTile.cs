using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTile : Tile
{

    [SerializeField] private Material _buildingTileMaterial;

    public override void Init(Vector3 location)
    {
        locationVector = location;
        _highlight.SetActive(false);

        materials.Add(_buildingTileMaterial);

        _renderer.SetMaterials(materials);
    }

    public void OnMouseEnter()
    {
        return;
    }
    public void OnMouseExit()
    {
        return;
    }
}
