using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTile : Tile
{
    /// <summary>
    /// Functionality for building tiles. Building tiles contain structures and are not able to be moved to or on by any units (currently)
    /// </summary>

    [SerializeField] private Material _buildingTileMaterial; // The material for all building tiles

    public override void Init(Vector3 location)
    {
        /// Init functionality. Sets the location and tile material
        /// Args:
        ///     Vector3 location: This tile's location
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
