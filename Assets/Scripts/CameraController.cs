using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // ~ Instances and Variables ~
    
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivity;
    // last position taken on mouse button down's frame to match against new positions
    public Vector3 lastPosition;
    // designated bounds for the zoom function's min and max zooms
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    // designated field bounds for the min/max of the x and y transform position
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    // float by which zoomSpeed will increment the orthographicSize by
    private float zoomSpeed = 0.5f;


    // ~ Methods ~

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

            // check to make sure that the orthographicSize field  doesn't go past minSize or maxSize
            if (cam.orthographicSize - zoomSpeed < minSize)
                return;
            else
                cam.orthographicSize -= zoomSpeed;
        if (scrollInput < 0)
            if (cam.orthographicSize + zoomSpeed > maxSize)
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
            // basic check to make sure that the position.x of transform doesnt go past certain bounds
            if (transform.position.x - delta.x * mouseSensitivity < minX || transform.position.x - delta.x * mouseSensitivity > maxX)
                return;
            // basic check performed this time with position.y of transform
            if (transform.position.y - delta.y * mouseSensitivity < minY || transform.position.y - delta.y * mouseSensitivity > maxY)
                return;
            else
            {
                transform.Translate(-delta.x * mouseSensitivity * 0.5f, -delta.y * mouseSensitivity, -delta.x * mouseSensitivity * 0.5f);
                lastPosition = Input.mousePosition;
            }
        }
    }
}
