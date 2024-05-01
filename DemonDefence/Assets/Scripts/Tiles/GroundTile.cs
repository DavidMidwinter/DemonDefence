using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Tile
{
    [SerializeField] protected Material _baseMaterial, _offsetMaterial;

    public override void Init(Vector2 location)
    {
        _highlight.SetActive(false);
        var isOffset = (location.x + location.y) % 2 == 1;
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
    private void OnMouseEnter()
    {
        /// Activates when the mouse is over a tile
        _highlight.SetActive(true);
    }
    private void OnMouseExit()
    {
        /// Activates when the mouse leaves a tile
        _highlight.SetActive(false);
    }
}
