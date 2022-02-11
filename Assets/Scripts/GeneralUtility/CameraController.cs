using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// brief: simple camera controller
public class CameraController : MonoBehaviour
{
    private const float minZoom = 0.5f;
    private const float maxZoom = 50f;

    Vector3 moveDirection = Vector3.zero;
    int moveAmount = 1;
    float zoomAmount = 0.5f;
    
    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if(Camera.main.orthographicSize >= maxZoom)
            {
                return;
            }
            Camera.main.orthographicSize += zoomAmount;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (Camera.main.orthographicSize <= minZoom)
            {
                return;
            }
            Camera.main.orthographicSize -= zoomAmount;
        }

        // move camera
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection = new Vector3(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection = new Vector3(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection = new Vector3(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection = new Vector3(1, 0);
        }
    }

    private void FixedUpdate()
    {
        transform.position += moveDirection * moveAmount;
        moveDirection = Vector3.zero;
    }
}
