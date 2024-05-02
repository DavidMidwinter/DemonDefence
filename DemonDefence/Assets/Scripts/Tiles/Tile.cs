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

    private void OnMouseDown()
    {
        if (GameManager.Instance.State != GameState.PlayerTurn) return;

        if(occupiedUnit != null)
        {
            if(occupiedUnit.faction == Faction.Player)
            {
                Debug.Log($"Occupying Unit {occupiedUnit}");
                UnitManager.Instance.SetSelectedHero((BasePlayerUnit)occupiedUnit);
            }
            else
            {
                if (UnitManager.Instance.SelectedUnit != null)
                {
                    var enemy = (BaseEnemy)occupiedUnit;
                    Destroy(enemy.gameObject);
                    UnitManager.Instance.SetSelectedHero(null);
                }
            }
            
        }
        else if (UnitManager.Instance.SelectedUnit != null)
        {
            SetUnit(UnitManager.Instance.SelectedUnit);
            UnitManager.Instance.SetSelectedHero(null);

        }
    }

}
