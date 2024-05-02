using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    /// <summary>
    /// Tile functionality and creation
    /// </summary>
    public Vector3 locationVector;
    [SerializeField] protected MeshRenderer _renderer;
    [SerializeField] protected GameObject _highlight;
    [SerializeField] protected List<Material> materials = new List<Material>();

    public virtual void Init(Vector3 location)
    {

    }

    
}
