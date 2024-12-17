using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    /// <summary>
    /// All functionality relating to the control of the main camera
    /// </summary>
    /// 
    public static CameraController Instance;
    public float xAxisValue = 0;
    public float zAxisValue = 0;
    public int cameraLimit = 500;
    private int _cameraOffset = 0;
    public float speed = 2;
    public Vector3 current_position;
    public Vector3 current_rotation;
    public int current_zoom;
    public (int min, int max) zoom_limit = (5, 50);
    public BaseEnemyUnit selectedEnemy => UnitManager.Instance.SelectedEnemy;

    public GameObject holder;

    public Camera Player;

    public void Awake()
    {
        Instance = this;
    }
    public void Init(int mapSize, int tileSize)
    {
        cameraLimit = mapSize * tileSize;
        current_rotation = holder.transform.rotation.eulerAngles;
        current_zoom = (int)Player.transform.localPosition.y;
    }

    void Update()
    {
        if (PauseMenu.GameIsPaused) return;

        if (GameManager.Instance.cameraCentring && selectedEnemy)
        {
            if (selectedEnemy.attacking)
            {
                centreCamera(Utils.getSmallerVector(
                    selectedEnemy.transform.position,
                    selectedEnemy.target.transform.position));
            }
            else
            {
                centreCamera(selectedEnemy.transform.position);
            }
        }
        else { manualMovement();
        }

        manualDirection();
        manualZoom();
    }

    void manualMovement()
    {
        xAxisValue = Input.GetAxisRaw("Horizontal") * speed;
        zAxisValue = Input.GetAxisRaw("Vertical") * speed;
        if (holder != null)
        {
            current_position = holder.transform.position;

            Vector3 offset = new Vector3(xAxisValue, 0.0f, zAxisValue);

            offset = Quaternion.Euler(current_rotation) * offset;



            offset.x = compareMovement(current_position.x, offset.x);
            offset.z = compareMovement(current_position.z, offset.z);
            holder.transform.position += offset;

        }
    }

    void manualDirection()
    {

        if (Input.GetKeyDown(KeyCode.Q)) rotateCamera(false);
        else if (Input.GetKeyDown(KeyCode.E)) rotateCamera(true);
    }

    void manualZoom()
    {
        if (Input.mouseScrollDelta.y != 0) zoomCamera(Input.mouseScrollDelta.y);
    }

    float compareMovement(float position, float movement)
    {
        /// Check that movement will not take camera off the board
        /// Args:
        ///     float position: The current position in the direction of movement
        ///     float movement: The amount to move
        /// Returns:
        ///     float movement - this is 0 if movement would be invalid, and the passed value otherwise
        if (movement < 0 && position + movement < _cameraOffset)
        {
            movement = 0;
        }
        if (movement > 0 && position + movement > cameraLimit + _cameraOffset)
        {
            movement = 0;
        }
        return movement;
    }

    public void centreCamera(Vector3 position)
    {
        if (!GameManager.Instance.cameraCentring) return;
        Vector3 new_position = new Vector3(position.x, holder.transform.position.y, position.z);
        holder.transform.position = new_position;
    }

    public void centreCameraOnObject(GameObject target, bool forced = false)
    {
        if(forced || !target.GetComponent<Renderer>().isVisible)
        {
            centreCamera(target.transform.position);
        }
    }

    public void rotateCamera(bool right)
    {
        float y = right == true ? -45 : 45;



        holder.transform.rotation *= Quaternion.Euler(0, y, 0);
        current_rotation = holder.transform.rotation.eulerAngles;


    }

    public void zoomCamera(float magnitude)
    {
        current_zoom -= (int) magnitude;
        current_zoom = current_zoom < zoom_limit.min ? zoom_limit.min : current_zoom;
        current_zoom = current_zoom > zoom_limit.max ? zoom_limit.max : current_zoom;

        Player.transform.localPosition = new Vector3(0, current_zoom, current_zoom * -1);
    }
}
