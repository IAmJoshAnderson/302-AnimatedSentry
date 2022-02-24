using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public Transform boneLegLeft;
    public Transform boneLegRight;
    public Transform boneHip;
    public Transform boneSpine;

    public float walkSpeed = 5; // How fast the guy is moving

   [Range (-10, -1)]
   public float gravity = -1;

    public Camera cam;

    CharacterController pawn;
    PlayerTargeting targeting; // references to other scripts!


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
        targeting = GetComponent<PlayerTargeting>();
    }

    // Update is called once per frame
    void Update()
    {

        if (cooldownJumpWindow > 0) cooldownJumpWindow -= Time.deltaTime;

        float v = Input.GetAxis("Vertical"); // using "getaxisraw" is getting exact values for smoother movement, if you so want it.
        float h = Input.GetAxis("Horizontal");

        // Turning to face the same direction as the camera, only turning the yaw value

        bool playerWantsToMove = (v != 0 || h != 0);

        bool playerIsAiming = (targeting && targeting.playerWantsToAim && targeting.target);

        if (playerIsAiming)
        {
            Vector3 toTarget = targeting.target.transform.position;

                toTarget.Normalize();

                Quaternion worldRot = Quaternion.LookRotation(toTarget);
            Vector3 euler = worldRot.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            worldRot.eulerAngles = euler;

            transform.rotation = AnimMath.Ease(transform.rotation, worldRot, .01f);
        }
        else if (cam && playerWantsToMove)
        {
            //turn player to match camera.

            float playerYaw = transform.eulerAngles.y;
            float camYaw = cam.transform.eulerAngles.y;

            while (camYaw > playerYaw + 180) camYaw -= 360;
            while (camYaw < playerYaw - 180) camYaw += 360;

            Quaternion playerRotation = Quaternion.Euler(0, playerYaw, 0);
            Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0);

        }


        inputDir = transform.forward * v + transform.right * h;
            if (inputDir.sqrMagnitude > 1) inputDir.Normalize();

//vertical movement;
        bool wantsToJump = Input.GetButtonDown("Jump"); // Boolean to be set to true if the button "Jump" is pressed in the project settings
        if (pawn.isGrounded)
        {

            cooldownJumpWindow = .5f;
            velocityVertical = 0;
            WalkAnimation();
        }
        else
        {
            AirAnimation();
        
           
        } 
            velocityVertical += gravity * Time.deltaTime;

        // move player:
            Vector3 moveAmount = inputDir * walkSpeed + Vector3.up * velocityVertical;
            pawn.Move(moveAmount * Time.deltaTime);
        if (pawn.isGrounded) cooldownJumpWindow = .5f;
        

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


        // the bob up and down in the hips
        if (boneHip) {

            float walkAmount = axis.magnitude;
            float offsetY = Mathf.Cos(Time.time * speed) * walkAmount * .05f;
        boneHip.localPosition = new Vector3(0, offsetY, 0);
        }
    }
    void AirAnimation()
    {


        
    }
}
