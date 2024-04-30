using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class camera : MonoBehaviour
{
    public float xAxisValue = 0;
    public float zAxisValue = 0;
    public float speed = 5;
    public Camera Player;
    // Update is called once per frame
    void Update()
    {
        xAxisValue = Input.GetAxisRaw("Horizontal");
        zAxisValue = Input.GetAxisRaw("Vertical");
        if (Player != null)
        {
            Vector3 offset = new Vector3(xAxisValue * speed, 0.0f, zAxisValue * speed);
            Player.transform.position += offset;
        }
    }
}
