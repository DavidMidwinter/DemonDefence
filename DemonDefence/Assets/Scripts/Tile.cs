using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Material _baseMaterial, _offsetMaterial;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    public void Init(bool isOffset)
    {
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
        _highlight.SetActive(true);
    }
    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }
}
