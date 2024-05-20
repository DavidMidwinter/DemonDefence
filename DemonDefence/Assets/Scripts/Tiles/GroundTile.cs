using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Tile
{
    [SerializeField] protected Material 
        _baseMaterial, 
        _offsetMaterial,
        _movementHighlightMaterial,
        _targetHighlightMaterial;
    [SerializeField] protected GameObject _validHighlight;
    [SerializeField] protected MeshRenderer _validHighlightRenderer;

    public override void Init(Vector3 location)
    {
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
        List<Material> highlightMaterial = new List<Material> { material };
        _validHighlightRenderer.SetMaterials(highlightMaterial);
        _validHighlight.SetActive(true);
    }

    private bool amValidTarget(BaseUnit attacker)
    {

        return (occupiedUnit != null) && occupiedUnit.amValidTarget(attacker);
    }
}
