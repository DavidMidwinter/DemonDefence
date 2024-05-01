using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class camera : MonoBehaviour
{
    public float xAxisValue = 0;
    public float zAxisValue = 0;
    public int cameraLimit = 500;
    private int _cameraOffset = -20;
    public float speed = 2;
    public Vector3 current_position;

    public Camera Player;

    void Init(int mapSize, int tileSize)
    {
        cameraLimit = mapSize * tileSize;
        _cameraOffset = tileSize * -2;
    }

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
}
