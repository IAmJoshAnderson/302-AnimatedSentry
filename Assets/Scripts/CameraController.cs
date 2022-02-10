using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;

    public Vector3 targetOffset;

    public float mouseSensitivityX = 5;
    public float mouseSensitivityY = -5;
    public float mouseSensitivityScroll = 5;


    private Camera cam;
    private float pitch = 0;
    private float yaw = 0;
    private float dollyDis = 10;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    { 

        //1. Ease Position.

        if(target)
        {
            transform.position = AnimMath.Ease(transform.position, target.position + targetOffset, .01f); // targetOffset can move the camera off of the very middle of the player.
        }

        //2. Set Rotation

        float mx = Input.GetAxis("Mouse X"); //we got mouseX
        float my = Input.GetAxis("Mouse Y"); // we got mouseY

        yaw += mx * mouseSensitivityX; // However much our mouse has changed in movement.
        pitch += my * mouseSensitivityY;

        pitch = Mathf.Clamp(pitch, -10, 89); // The camera y cannot move farther than these degrees.

        transform.rotation = Quaternion.Euler(pitch, yaw, 0); // Euler XYZ, Pitch, Yaw, Roll.

        //3. Dolly camera in/out

        dollyDis += Input.mouseScrollDelta.y * mouseSensitivityScroll;
        dollyDis = Mathf.Clamp(dollyDis, 3, 20);

        cam.transform.localPosition = AnimMath.Ease(cam.transform.localPosition, new Vector3(0, 0, -dollyDis), .02f); // Vector 3 gives the dolly effect, AnimMath gives an easing effect.


    }

    private void OnDrawGizmos()
    {
        if (!cam) cam = GetComponentInChildren<Camera>(); // Camera can't be seen in the void start, and the cam isn't a typical variable.
        if (!cam) return;

        Gizmos.DrawWireCube(transform.position, Vector3.one); // One meter cube drawn where the rig is.
        Gizmos.DrawLine(transform.position, cam.transform.position); // The line from the camera to the rig.
    }

}
