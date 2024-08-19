using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Tile : MonoBehaviour
{
    /// <summary>
    /// Tile functionality and creation
    /// </summary>
    [SerializeField] protected MeshRenderer _renderer; // Renderer for materials
    [SerializeField] protected GameObject _highlight; // Highlight for when mouse is over the tile
    [SerializeField] protected List<Material> materials = new List<Material>(); // List of materials used by this tile.
    [SerializeField] private bool _isWalkable; // Confirm if a tile type is able to be walked on (if there is no occupying unit)
    public BaseUnit occupiedUnit; // The occupying Unit
    protected List<Tile> neighbours = new List<Tile>(); // All neighbours of this Tile
    private LayerMask buildingMask;
    private Vector3 offset = new Vector3(0, 0.2f, 0);
    public bool givesCover;
    public bool Walkable => _isWalkable && occupiedUnit == null; // If this tile is currently walkable

    public void Awake()
    {
        buildingMask = LayerMask.GetMask("Buildings");
    }
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

    public Vector3 get3dLocation()
    {
        return transform.position;
    }
    private void OnMouseDown()
    {
        if (!GameManager.Instance.inputEnabled)
        {
            Debug.LogWarning("Input disabled");
            return; 
        }

        if (GameManager.Instance.State != GameState.PlayerTurn) {
            Debug.LogWarning("Not player turn");
            return; 
        }

        if (TacticalUI.Instance.IsPointerOverUIElement())
        {
            Debug.LogWarning("Mouse on UI button");
            return; 
        }

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
                    if (attacker.validTargets.Contains(enemy))
                    {
                        StartCoroutine(attacker.makeAttack(enemy));
                    }
                    
                }
            }
            
        }
        else if (UnitManager.Instance.SelectedUnit != null)
        {
            if (UnitManager.Instance.SelectedUnit.isInRangeTile(this))
            {
                SetUnit(UnitManager.Instance.SelectedUnit);
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
        return Utils.calculateDistance(get2dLocation(), target.get2dLocation());
    }

    public Vector3 getBearing(Tile target)
    {
        return Utils.calculateBearing(get3dLocation(), target.get3dLocation());
    }
    public bool checkClearLine(Tile target)
    {
        float dist = getDistance(target);
        if (dist <= 10) return true;

        if (target.givesCover) return false;

        if (target.neighbours.Exists(t => t.givesCover && getDistance(t) < dist-5)) return false;

        Vector3 bearing = getBearing(target);
        bool visible = !Physics.Raycast(get3dLocation() + offset, bearing, dist, buildingMask);
        return visible;
    }

    public Tile getFirstTileCollision(Tile target)
    {
        RaycastHit[] collisions = getAllCollisions(target);

        foreach (RaycastHit collision in collisions)
        {
            GameObject location = collision.collider.gameObject;
            int x = (int)location.transform.position.x / 10, y = (int)location.transform.position.z / 10;

            Vector2 location2d = new Vector2(x, y);
            Tile tile = GridManager.Instance.getTile(location2d);
            if (tile)
                return tile;

        }
        return null;

    }

    public RaycastHit[] getAllCollisions(Tile target)
    {
        float dist = getDistance(target);
        Vector3 bearing = getBearing(target);

        return Physics.RaycastAll(get3dLocation() + offset, bearing, dist, buildingMask);

    }
    public bool collidesWithWall(Tile target)
    {
        RaycastHit[] collisions = getAllCollisions(target);

        foreach (RaycastHit collision in collisions)
        {
            GameObject location = collision.collider.gameObject;
            int x = (int)location.transform.position.x / 10, y = (int)location.transform.position.z / 10;

            Vector2 location2d = new Vector2(x, y);
            Tile tile = GridManager.Instance.getTile(location2d);
            if (tile && tile is WallTile)
                return true;

        }
        return false;

    }

}
