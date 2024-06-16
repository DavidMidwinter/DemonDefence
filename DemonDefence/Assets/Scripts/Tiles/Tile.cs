using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    /// <summary>
    /// Tile functionality and creation
    /// </summary>
    public Vector3 locationVector; // This tile's location.
    [SerializeField] protected MeshRenderer _renderer; // Renderer for materials
    [SerializeField] protected GameObject _highlight; // Highlight for when mouse is over the tile
    [SerializeField] protected List<Material> materials = new List<Material>(); // List of materials used by this tile.
    [SerializeField] private bool _isWalkable; // Confirm if a tile type is able to be walked on (if there is no occupying unit)
    public BaseUnit occupiedUnit; // The occupying Unit
    private List<Tile> neighbours = new List<Tile>(); // All neighbours of this Tile

    public bool Walkable => _isWalkable && occupiedUnit == null; // If this tile is currently walkable

    public virtual void Init(Vector3 location)
    {
        /// Init functionality. This is overridden by child objects.
    }
    public void SetUnit(BaseUnit unit)
    {
        /// Set the current tile occupant. If the occupant is a Player unit, create a path to this tile.
        /// Args:
        ///     BaseUnit unit: The unit to set as the occupant.
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
        /// Add a tile to the neighbour list
        /// Args:
        ///     Tile neighbour: The tile to add
        neighbours.Add(neighbour);
    }

    public List<Tile> getNeighbours()
    {
        /// Get all the neighbours of this tile
        /// Returns:
        ///     List<Tile> neighbours: List of neighbour tiles
        return neighbours;
    }

    public string getNeighbourList()
    {
        /// Get the neighbour list as a string: only used for debug purposes
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
        /// Get this tile's location as a Vector2. 'y' is ignored as vertical distance is never used.
        /// Returns:
        ///     Vector2: This tile's 2D location
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
                    var enemy = (BaseEnemyUnit)occupiedUnit;
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
                UnitManager.Instance.SelectedUnit.takeAction();
            }

        }
    }

    public float getDistance(Tile target)
    {
        /// Returns the distance to a given tile
        /// Args:
        ///     Tile target: The tile to get the distance to
        /// Returns:
        ///     float: The distance to the target
        return Utils.calculateDistance(transform.position, target.transform.position);
    }

}
