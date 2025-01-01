using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Camera mainCamera;
    float speed = 5;
    (float min, float max)zoomRestriction;
    public float xAxisValue = 0;
    public float yAxisValue = 0;
    public Vector3 current_position;

    private void Start()
    {
        zoomRestriction = (5f, GridManager.Instance.getGridSize() * 0.75f);

        mainCamera.orthographicSize = zoomRestriction.max;
        current_position = new Vector3(GridManager.Instance.getGridSize() / 2, GridManager.Instance.getGridSize() / 2, -10f);
        transform.position = current_position;
    }
    void Update()
    {
        FrameMove();
    }

    public void FrameMove()
    {
        manualMovement();
        manualZoom();
    }
    void manualMovement()
    {
        xAxisValue = Input.GetAxisRaw("Horizontal") * (speed / 100);
        yAxisValue = Input.GetAxisRaw("Vertical") * (speed / 100);
        current_position = transform.position;

        Vector3 offset = new Vector3(xAxisValue, yAxisValue, 0.0f);



        offset.x = compareMovement(current_position.x, offset.x, (0, GridManager.Instance.getGridSize()));
        offset.y = compareMovement(current_position.y, offset.y, (0, GridManager.Instance.getGridSize()));
        transform.position += offset;


    }
    float compareMovement(float position, float movement, (float min, float max) restriction)
    {
        /// Check that movement will not take camera off the board
        /// Args:
        ///     float position: The current position in the direction of movement
        ///     float movement: The amount to move
        /// Returns:
        ///     float movement - this is 0 if movement would be invalid, and the passed value otherwise
        if (movement < 0 && position + movement < restriction.min)
        {
            movement = 0;
        }
        if (movement > 0 && position + movement > restriction.max)
        {
            movement = 0;
        }
        return movement;
    }



    void manualZoom()
    {
        if (Input.mouseScrollDelta.y != 0) zoomCamera(Input.mouseScrollDelta.y);
    }
    public void zoomCamera(float magnitude)
    {
        float zoom = mainCamera.orthographicSize - magnitude;
        if (zoom <= zoomRestriction.max && zoom >= zoomRestriction.min) mainCamera.orthographicSize = zoom;
    }


}
