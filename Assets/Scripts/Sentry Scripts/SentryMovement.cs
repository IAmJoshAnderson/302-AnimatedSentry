using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SentryMovement : MonoBehaviour
{

    public Transform boneTopLeft;
    public Transform boneBottomLeft;
    public Transform boneTopRight;
    public Transform boneBottomRight;
    public Transform allLegs;
    public Transform boneTopLeftFoot;
    public Transform boneTopRightFoot;
    public Transform boneBottomLeftFoot;
    public Transform boneBottomRightFoot;

    public float walkSpeed = 3;

    public float gravity = -1;

    public Camera cam;

    CharacterController pawn2;
    SentryTargeting targeting;

    private Vector3 inputDir; // what makes the turret move

    private float velocityVertical = 0;

    private float cooldownJumpWindow = 0;
    public bool IsGrounded
    {
        get
        {
            return pawn2.isGrounded || cooldownJumpWindow > 0;
        }
    }

    private void Start()
    {
        pawn2 = GetComponent<CharacterController>();
        targeting = GetComponent<SentryTargeting>();

    }

    void Update()
    {

        if (cooldownJumpWindow > 0) cooldownJumpWindow -= Time.deltaTime;

        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");


        bool sentryWantsToMove = (v != 0 || h != 0);

        bool sentryIsAiming = (targeting && targeting.sentryWantsToAim && targeting.target);

        if (sentryIsAiming)
        {
            Vector3 toTarget = targeting.target.transform.position - transform.position;

            toTarget.Normalize();

            Quaternion worldRot = Quaternion.LookRotation(toTarget);
            Vector3 euler = worldRot.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            worldRot.eulerAngles = euler;

            transform.rotation = AnimMath.Ease(transform.rotation, worldRot, .01f);
        }
        else if (cam && sentryWantsToMove)
        {

            float playerYaw = transform.eulerAngles.y;
            float camYaw = cam.transform.eulerAngles.y;

            while (camYaw > playerYaw + 180) camYaw -= 360;
            while (camYaw < playerYaw - 180) camYaw += 360;

            Quaternion playerRotation = Quaternion.Euler(0, playerYaw, 0);
            Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0);

            transform.rotation = AnimMath.Ease(playerRotation, targetRotation, .01f);
        }

        inputDir = transform.forward * v + transform.right * h; // what makes the sentry move?
        if (inputDir.sqrMagnitude > 1) inputDir.Normalize();

        if (IsGrounded)
        {
            WalkAnimation();
            velocityVertical = 0;
        }
        else if (sentryWantsToMove == false)
        {
            IdleAnimation();
            print("IdleAnimation should be playing.");
        }
        velocityVertical += gravity * Time.deltaTime;

        Vector3 moveAmount = inputDir * walkSpeed + Vector3.up * velocityVertical;
        pawn2.Move(moveAmount * Time.deltaTime);
        if (pawn2.isGrounded) cooldownJumpWindow = .5f;

    }
    void WalkAnimation()
    {
        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);

        alignment = Mathf.Abs(alignment);

        float degrees = AnimMath.Lerp(10, 40, alignment);
        float speed = 10;
        float wave = Mathf.Sin(Time.time * speed) * degrees;

        Quaternion playerRotation = Quaternion.AngleAxis(wave, axis);
        Quaternion targetRotation = Quaternion.AngleAxis(-wave, axis);

        if (allLegs)
        {

        }
        if (boneTopLeft || boneTopRight)
        {

        }
        if (boneBottomLeft || boneBottomRight)
        {

        }
        if (boneTopLeftFoot || boneTopRightFoot || boneBottomLeftFoot || boneBottomRightFoot) // attempting to convert from global position to local position
        {
     

            float walkAmount = axis.magnitude;
            float offsetY = Mathf.Cos(Time.time * speed) * walkAmount * .5f;
            boneTopLeftFoot.transform.localPosition = new Vector3(offsetY, offsetY, 0);
            boneTopRightFoot.transform.localPosition = new Vector3(0, offsetY, -offsetY);
            boneBottomLeftFoot.transform.localPosition = new Vector3(0, offsetY, offsetY);
            boneBottomRightFoot.transform.localPosition = new Vector3(-offsetY, offsetY, 0);

        }
    }
    void IdleAnimation()
    {
        // the new vectors are all 0 to reduce all the rotation of the legs when not moving
        boneTopLeftFoot.transform.localPosition = Vector3.zero;
        boneTopRightFoot.transform.localPosition = Vector3.zero;
        boneBottomLeftFoot.transform.localPosition = Vector3.zero;
        boneBottomRightFoot.transform.localPosition = Vector3.zero;
    }
    }


