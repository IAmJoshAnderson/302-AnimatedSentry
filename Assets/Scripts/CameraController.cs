using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public PlayerTargeting player;

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
        if (player == null)
        {
            PlayerTargeting script = FindObjectOfType<PlayerTargeting>();
            if (script != null) player = script;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //is the player aiming?
        bool isAiming = (player && player.target && player.playerWantsToAim);



        //1. Ease rig Position.

        if(player)
        {
            transform.position = AnimMath.Ease(transform.position, player.transform.position + targetOffset, .01f); // targetOffset can move the camera off of the very middle of the player.
        }

        //2. Set rig Rotation

        float playerYaw = AnimMath.AngleWrapDegrees(yaw, player.transform.eulerAngles.y);

        while (yaw > playerYaw + 180) playerYaw += 360;
        while (yaw < playerYaw - 180) playerYaw -= 360;

        if (isAiming)
        {
            Quaternion tempTarget = Quaternion.Euler(0, playerYaw, 0);

            transform.rotation = AnimMath.Ease(transform.rotation, tempTarget, .001f);

            Vector3 vToAimTarget = player.target.transform.position - cam.transform.position;

            Vector3 euler = Quaternion.LookRotation(vToAimTarget).eulerAngles;

            while (playerYaw > euler.y + 180) euler.y += 360;
            while (playerYaw < euler.y - 180) euler.y -= 360;

            Quaternion temp = Quaternion.Euler(euler.x, euler.y, 0);

            cam.transform.rotation = AnimMath.Ease(cam.transform.localRotation, temp, .001f);
        }
        else
        {
            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, Quaternion.identity, .001f);
        }

        //3. Dolly camera in/out

        dollyDis += Input.mouseScrollDelta.y * mouseSensitivityScroll;
            dollyDis = Mathf.Clamp(dollyDis, 3, 20);

        float tempZ = isAiming ? 2 : dollyDis; // true = 2, false = dollyDis

            cam.transform.localPosition = AnimMath.Ease(cam.transform.localPosition, new Vector3(0, 0, -tempZ), .02f); // Vector 3 gives the dolly effect, AnimMath gives an easing effect.
        
        // 4. rotate the camera object

        if (isAiming)
        {
            Vector3 vToAimTarget = player.target.transform.position - cam.transform.position;

            Vector3 euler = Quaternion.LookRotation(vToAimTarget).eulerAngles;

            euler.y = AnimMath.AngleWrapDegrees(playerYaw, euler.y);


        }else
        {
            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, Quaternion.identity, .001f); // identity means no rotation
        }


    }

    private void OnDrawGizmos()
    {
        if (!cam) cam = GetComponentInChildren<Camera>(); // Camera can't be seen in the void start, and the cam isn't a typical variable.
        if (!cam) return;

        Gizmos.DrawWireCube(transform.position, Vector3.one); // One meter cube drawn where the rig is.
        Gizmos.DrawLine(transform.position, cam.transform.position); // The line from the camera to the rig.
    }

}
