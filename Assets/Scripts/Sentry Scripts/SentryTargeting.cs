using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTargeting : MonoBehaviour
{

    public TargetableObject sentryTarget { get; private set; }

    public bool playerWantstoAim { get; private set; } // in this script it's private, but it's public to other scripts.

    public bool playerWantsToAttack { get; private set; }

    private float coolDownScan = 0;
    public float visionDistance = 20; // How far can you be away before detecting an enemy
    private float cooldownPickTarget = 0;


    private List<TargetableObject> validtargets = new List<TargetableObject>();




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        coolDownScan -= Time.deltaTime;
        cooldownPickTarget -= Time.deltaTime;

        playerWantsToAttack = Input.GetButton("Fire1");
        playerWantstoAim = Input.GetButton("Fire2");

        if (playerWantstoAim)
        {

            if (sentryTarget != null)
            {
                if (!CanSeeTarget(sentryTarget))
                {
                    sentryTarget = null;
                }
            }
            if (coolDownScan <= 0) scanForTargets(); // Scans for targets
            if (cooldownPickTarget <= 0) PickATarget();

            else
            {
                sentryTarget = null;
            }


        }
    }
    void scanForTargets()
    {
        coolDownScan = .5f;

        validtargets.Clear();

        TargetableObject[] objects = GameObject.FindObjectsOfType<TargetableObject>(); // Find all Targetable Objects, returns an array list of each object.

        foreach (TargetableObject thing in objects) // For each of these objects...
        {
            if (CanSeeTarget(thing))
            {
                validtargets.Add(thing); // Add this new target to the array list
            }

        }
    }
    void PickATarget()
    { 
            if (sentryTarget) return;

            float closestDistanceSoFar = 0;

            foreach (TargetableObject thing in validtargets)
            {
                Vector3 vToThing = thing.transform.position - transform.position;

                float dis = vToThing.sqrMagnitude;

                if (dis < closestDistanceSoFar || sentryTarget == null)
                {
                    closestDistanceSoFar = dis;
                    sentryTarget = thing;
                }

            }

    }
    private bool CanSeeTarget(TargetableObject thing)
    {
        Vector3 vToThing = thing.transform.position - transform.position; // The position of the thing, minus the position of the sentry.

        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false; // if the square root of this is greater than vision distance squared

        float alignment = Vector3.Dot(transform.forward, vToThing.normalized); // product of two vectors

        if (alignment < .4f) return false; // product of two vectors less than .4?

        return true;

    }

}
