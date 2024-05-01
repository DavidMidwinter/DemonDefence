using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    /// <summary>
    /// Tile functionality and creation
    /// </summary>
    [SerializeField] private Material _baseMaterial, _offsetMaterial;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    public void Init(bool isOffset)
    {
        /// Initialises the tile: sets the Highlight object to inactive, then checks if
        /// the tile is an offset and updates accordingly.
        /// Args:
        ///     bool isOffset: Whether the tile is an offset tile or not
        _highlight.SetActive(false);

        List<Material> materials = new List<Material>();
        if (isOffset) {
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
