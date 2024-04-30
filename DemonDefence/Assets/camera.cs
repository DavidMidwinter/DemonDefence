using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class camera : MonoBehaviour
{
    public float xAxisValue = 0;
    public float zAxisValue = 0;
    public int cameraLimit = 250;
    public float speed = 5;
    public Vector3 current_position;

    public Camera Player;
    // Update is called once per frame
    void Update()
    {
        xAxisValue = Input.GetAxisRaw("Horizontal") * speed;
        zAxisValue = Input.GetAxisRaw("Vertical") * speed;
        if (Player != null)
        {
            current_position = Player.transform.position;

            xAxisValue = compareMovement(current_position.x, xAxisValue);
            zAxisValue = compareMovement(current_position.z, zAxisValue);

            Vector3 offset = new Vector3(xAxisValue, 0.0f, zAxisValue);
            Player.transform.position += offset;
        }
    }

    float compareMovement(float position, float movement)
    {
        if (movement < 0 && position + movement < (0-(cameraLimit)))
        {
            movement = 0;
        }
        if (movement > 0 && position + movement > (cameraLimit))
        {
            movement = 0;
        }
        return movement;
    }
}
