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
    private List<Tile> neighbours = new List<Tile>();

    public bool Walkable => _isWalkable && occupiedUnit == null;

    public virtual void Init(Vector3 location)
    {

    }
    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null)
        {
            unit.OccupiedTile.occupiedUnit = null;
            if(unit.faction == Faction.Player) unit.createPath(this);
        }
        occupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    public void setNeighbour(Tile neighbour)
    {
        neighbours.Add(neighbour);
    }

    public List<Tile> getNeighbours()
    {
        return neighbours;
    }

    public string getNeighbourList()
    {
        string neighbourList = $"Neighbours of {get2dLocation()}:";
        foreach (Tile t in neighbours)
        {
            neighbourList = neighbourList + $"{t.get2dLocation()} ";
        }

        return neighbourList;
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

    public Vector2 get2dLocation()
    {
        return new Vector2(transform.position.x, transform.position.z);
    }
    private void OnMouseDown()
    {
        if (!GameManager.Instance.inputEnabled) return;

        if (GameManager.Instance.State != GameState.PlayerTurn) return;

        if (!_isWalkable) return;

        if (occupiedUnit != null)
        {
            if(occupiedUnit.faction == Faction.Player)
            {
                UnitManager.Instance.SetSelectedHero((BasePlayerUnit)occupiedUnit);
            }
            else
            {

                if (UnitManager.Instance.SelectedUnit != null)
                {
                    var enemy = (BaseEnemy)occupiedUnit;
                    var attacker = (BasePlayerUnit)UnitManager.Instance.SelectedUnit;
                    if (UnitManager.Instance.SelectedUnit.checkRange(enemy))
                    {
                        StartCoroutine(UnitManager.Instance.SelectedUnit.makeAttack(enemy));
                    }
                    
                }
            }
            
        }
        else if (UnitManager.Instance.SelectedUnit != null)
        {
            if (UnitManager.Instance.SelectedUnit.isInRangeTile(this))
            {
                SetUnit(UnitManager.Instance.SelectedUnit);
                UnitManager.Instance.takeAction();
            }

        }
    }

}
