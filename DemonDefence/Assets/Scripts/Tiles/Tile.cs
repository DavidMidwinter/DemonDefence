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
    [SerializeField] private bool _isWalkable;
    public BaseUnit occupiedUnit;

    public bool Walkable => _isWalkable && occupiedUnit == null;

    public virtual void Init(Vector3 location)
    {

    }
    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.occupiedUnit = null;
        unit.transform.position = transform.position;
        occupiedUnit = unit;
        unit.OccupiedTile = this;
    }



}
