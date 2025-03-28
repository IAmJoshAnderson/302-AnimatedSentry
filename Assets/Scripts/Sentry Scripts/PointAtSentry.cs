using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtSentry : MonoBehaviour
{
    public Transform target;

    public bool lockAxisX = false;
    public bool lockAxisY = false;
    public bool lockAxisZ = false;

    private Quaternion startRotation;
    private Quaternion goalRotation;


    // Start is called before the first frame update
    void Start()
    {
        startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        TurnTowardsTarget();
    }
    private void TurnTowardsTarget()
    {
        if (target != null)// if there is a target
        {
            Vector3 vToTarget = target.position - transform.position;
            vToTarget.Normalize();

            Quaternion worldRot = Quaternion.LookRotation(vToTarget, Vector3.up);
            Quaternion localRot = worldRot;

            if (transform.parent)
            {
                //convert from world to local
                localRot = Quaternion.Inverse(transform.parent.rotation) * worldRot;
            }

            Vector3 euler = localRot.eulerAngles;

            if (lockAxisX) euler.x = startRotation.eulerAngles.x;
            if (lockAxisY) euler.y = startRotation.eulerAngles.y;
            if (lockAxisZ) euler.z = startRotation.eulerAngles.z;

            localRot.eulerAngles = euler;

            goalRotation = localRot;
        }
        else
        {
            goalRotation = startRotation;
        }
        transform.localRotation = AnimMath.Ease(transform.localRotation, goalRotation, .001f);
    }
}
