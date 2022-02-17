using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{

    public bool lockAxisX = false;
    public bool lockAxisY = false;
    public bool lockAxisZ = false;

    private Quaternion startRotation;
    private Quaternion goalRotation;

    private PlayerTargeting playerTargeting;

    // Start is called before the first frame update
    void Start()
    {
        playerTargeting = GetComponentInParent<PlayerTargeting>();
        startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {
        if (playerTargeting && playerTargeting.target && playerTargeting.playerWantsToAim)
        {

            Vector3 vToTarget = playerTargeting.target.transform.position - transform.position;
            vToTarget.Normalize();


            Quaternion worldRot = Quaternion.LookRotation(vToTarget, Vector3.up);
            Quaternion prevRot = transform.rotation;
            Vector3 eulerBefore = transform.localEulerAngles;
            transform.rotation = worldRot;
            Vector3 eulerAfter = transform.localEulerAngles;
            transform.rotation = prevRot;



            //if (transform.parent)
            //{
                //localRot = Quaternion.Inverse(transform.parent.rotation) * worldRot;
            //}

            if (lockAxisX) eulerAfter.x = eulerBefore.x;
            if (lockAxisY) eulerAfter.y = eulerBefore.y;
            if (lockAxisZ) eulerAfter.z = eulerBefore.z;

            goalRotation = Quaternion.Euler(eulerAfter);

        } else // Resets the arm to point at the guy's side
        {
            goalRotation = startRotation;
        }
        transform.localRotation = AnimMath.Ease(transform.localRotation, goalRotation, .001f);
    }
}
