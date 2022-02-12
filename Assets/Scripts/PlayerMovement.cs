using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public float walkSpeed = 5; // How fast the guy is moving

    public Camera cam;

    CharacterController pawn;


    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical"); // using "getaxisraw" is getting exact values for smoother movement, if you so want it.
        float h = Input.GetAxis("Horizontal");

        // Turning to face the same direction as the camera, only turning the yaw value

        bool playerWantsToMove = (v != 0 || h != 0);


        if (cam && playerWantsToMove)
        {
            //turn player to match camera.

            float playerYaw = transform.eulerAngles.y;
            float camYaw = cam.transform.eulerAngles.y;

            //while (camYaw > playerYaw + 180) camYaw -= 360;
           // while (camYaw < playerYaw - 180) camYaw += 360;

            Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0);

            transform.rotation = AnimMath.Ease(transform.rotation, targetRotation, .01f);
        }

        Vector3 moveDir = transform.forward * v + transform.right * h; // Where the character can move, vertical and horizontal!
        if (moveDir.sqrMagnitude > 1) moveDir.Normalize(); // Make sure the diagonal movement is not faster by combining the two acceleration values.

        pawn.SimpleMove(moveDir * walkSpeed);
    }
}
