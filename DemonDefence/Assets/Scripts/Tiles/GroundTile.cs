using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Tile
{
    [SerializeField] protected Material _baseMaterial, _offsetMaterial;
    [SerializeField] protected GameObject _validHighlight;

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
        if(UnitManager.Instance.SelectedUnit 
            && UnitManager.Instance.SelectedUnit.isInRangeTile(this)
            && GameManager.Instance.inputEnabled)
            _validHighlight.SetActive(true);
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
}
