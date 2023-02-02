using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivity = 0.005f;
    // last position taken on mouse button down's frame to match against new positions
    public Vector3 lastPosition;
    public float lastSize;
    [SerializeField] private float minSize = 2.5f;
    [SerializeField] private float maxSize = 5f;

    private float zoomSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

        lastPosition = transform.position;
        // ensures that the field of view is within min and max on startup
        cam.orthographicSize = 4;
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();

        // FOV change upon mouse scrollwheel change
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput > 0)
            if (cam.orthographicSize - zoomSpeed < 3)
                return;
            else 
            cam.orthographicSize -= zoomSpeed;
        if (scrollInput < 0)
            if (cam.orthographicSize + zoomSpeed > 6)
                return;
            else
                cam.orthographicSize += zoomSpeed;
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
            transform.Translate(-delta.x * mouseSensitivity, -delta.y * mouseSensitivity, 0);
            lastPosition = Input.mousePosition;
        }
    }

    void UpdateFov(float newSize)
    {
        cam.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
    }
}
