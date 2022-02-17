using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public Transform boneLegLeft;
    public Transform boneLegRight;

    public float walkSpeed = 5; // How fast the guy is moving

    [Range (-10, -1)]
    public float gravity = -1;

    public Camera cam;

    CharacterController pawn;
    private Vector3 inputDir;
    private float velocityVertical = 0;

    private float cooldownJumpWindow = 0;
    public bool IsGrounded
    {
        get
        {
            return pawn.isGrounded || cooldownJumpWindow > 0;
        }
    }
    


    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (cooldownJumpWindow > 0) cooldownJumpWindow -= Time.deltaTime;

        float v = Input.GetAxis("Vertical"); // using "getaxisraw" is getting exact values for smoother movement, if you so want it.
        float h = Input.GetAxis("Horizontal");

        // Turning to face the same direction as the camera, only turning the yaw value

        bool playerWantsToMove = (v != 0 || h != 0);


        if (cam && playerWantsToMove)
        
            //turn player to match camera.

            inputDir = transform.forward * v + transform.right * h;
            if (inputDir.sqrMagnitude > 1) inputDir.Normalize();

//vertical movement;
        bool wantsToJump = Input.GetButtonDown("Jump"); // Boolean to be set to true if the button "Jump" is pressed in the project settings
        if (pawn.isGrounded)
        {
            velocityVertical = 0;
            if (wantsToJump)
            {
                cooldownJumpWindow = 0;
            velocityVertical = 5;
            }
        } 
            velocityVertical += gravity * Time.deltaTime;

        // move player:
            Vector3 moveAmount = inputDir * walkSpeed + Vector3.up * velocityVertical;
            pawn.Move(moveAmount * Time.deltaTime);
        if (pawn.isGrounded) cooldownJumpWindow = .5f;
        

        WalkAnimation();
    }

    void WalkAnimation()
    {

        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);


        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);

        alignment = Mathf.Abs(alignment);

        float degrees = AnimMath.Lerp(10, 40, alignment);
        float speed = 10;
        float wave = Mathf.Sin(Time.time * speed) * degrees; // Outputs a number between -30 and 30

        Quaternion playerRotation = Quaternion.AngleAxis(wave, axis);
        Quaternion targetRotation = Quaternion.AngleAxis(-wave, axis);

        boneLegLeft.localRotation = Quaternion.Euler(wave, 0, 0);
        boneLegRight.localRotation = Quaternion.Euler(-wave, 0, 0);

    }

}
