using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 0.001f; 
    public Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition= transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();
    }
    void PanCamera()
    {
        // saves position of mouse in world space when drag starts (rmb) 
        if (Input.GetMouseButtonDown(1))
        {
            lastPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            // input.mousePosition.z is always zero, use mousePosition.y
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(delta.x * mouseSensitivity, 0, delta.y * mouseSensitivity);
            lastPosition = Input.mousePosition;
        }
    }
}
