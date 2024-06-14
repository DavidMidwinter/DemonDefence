using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Tile
{
    /// <summary>
    /// Functionality for standard Ground Tiles. Ground Tiles can be walked on freely by any units, as long as another unit isn't on them.
    /// </summary>
    [SerializeField] protected Material 
        _baseMaterial, // Material for odd Tiles
        _offsetMaterial, // Material for even Tiles
        _movementHighlightMaterial, // Highlight material when a tile can be moved to
        _targetHighlightMaterial; // Highlight material when a tile's occupier is targetable
    [SerializeField] protected GameObject _validHighlight; // The validHighlight object
    [SerializeField] protected MeshRenderer _validHighlightRenderer; // The renderer for the validHighlight.

    public override void Init(Vector3 location)
    {
        /// Init functionality. Sets the location, disables highlights and determines if this tile is an offset tile (and sets material accordingly)
        /// to create an alternating grid pattern.
        /// Args:
        ///     Vector3 location: This tile's location
        locationVector = location;
        _highlight.SetActive(false);
        _validHighlight.SetActive(false);
        var isOffset = (location.x + location.z) % 2 == 1;
        if (isOffset)
        {
            materials.Add(_offsetMaterial);
        }
        else
        {
            materials.Add(_baseMaterial);

        }

        _renderer.SetMaterials(materials);
    }

    private void Update()
    {
        if (UnitManager.Instance.SelectedUnit
            && UnitManager.Instance.SelectedUnit.isInRangeTile(this)
            && GameManager.Instance.inputEnabled)
            setHighlightMaterial(_movementHighlightMaterial);

        else if (UnitManager.Instance.SelectedUnit
            && amValidTarget(UnitManager.Instance.SelectedUnit)
            && GameManager.Instance.inputEnabled)
            setHighlightMaterial(_targetHighlightMaterial);
        else if (GameManager.Instance.debugMode
            && UnitManager.Instance.SelectedEnemy
            && UnitManager.Instance.SelectedEnemy.pathTiles != null
            && UnitManager.Instance.SelectedEnemy.pathTiles.Exists(t => t.referenceTile.locationVector == locationVector))
            setHighlightMaterial(_movementHighlightMaterial);
        else
            _validHighlight.SetActive(false);
    }
    private void OnMouseEnter()
    {
        if(GameManager.Instance.inputEnabled)
        /// Activates when the mouse is over a tile
            _highlight.SetActive(true);
    }
    private void OnMouseExit()
    {
        /// Activates when the mouse leaves a tile
        _highlight.SetActive(false);
    }

    private void setHighlightMaterial(Material material)
    {
        /// Sets the 'validHighlight' to have a given material and activates it.
        /// Args:
        ///     Material material: The material to set.
        List<Material> highlightMaterial = new List<Material> { material };
        _validHighlightRenderer.SetMaterials(highlightMaterial);
        _validHighlight.SetActive(true);
    }

    private bool amValidTarget(BaseUnit attacker)
    {
        /// Returns if this tile is a valid target for an attacker.
        /// Args:
        ///     BaseUnit attacker: The attacking unit
        /// Returns:
        ///     True if this tile is occupied, and the occupier is a valid target of the attacker
        return (occupiedUnit != null) && occupiedUnit.amValidTarget(attacker);
    }
}
