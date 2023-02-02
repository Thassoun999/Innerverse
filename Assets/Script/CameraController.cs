using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    public float mouseSensitivity = 0.001f;
    // last position taken on mouse button down's frame to match against new positions
    public Vector3 lastPosition;
    [SerializeField] private float minFov = 20;
    [SerializeField] private float maxFov = 80; 

    // Start is called before the first frame update
    void Start()
    {
        
        lastPosition = transform.position;
        // ensures that the field of view is within min and max on startup
        UpdateFov(cam.fieldOfView);

    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();

        // FOV change upon mouse scrollwheel change
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput > 0)
            UpdateFov(cam.fieldOfView - 1);
        else if (scrollInput < 0)
            UpdateFov(cam.fieldOfView + 1);
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

    void UpdateFov(float newFov)
    {
        cam.fieldOfView = Mathf.Clamp(newFov, minFov, maxFov);
    }
}
