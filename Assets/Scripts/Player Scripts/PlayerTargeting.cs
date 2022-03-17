using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public float visionDistance = 10;

    [Range(1, 10)]
    public float roundsPerSecond = 5;


    public PointAt boneShoulderRight;
    public PointAt boneShoulderLeft;

    public TargetableObject target { get; private set; } // in this script it's private, but in other scripts it's public.
    public bool playerWantsToAim { get; private set; }
    public bool playerWantsToAttack { get; private set; }

    private List<TargetableObject> validTargets = new List<TargetableObject>();
    private float cooldownScan = 0;
    private float cooldownPickTarget = 0;
    private float cooldownAttack = 0;

    private CameraController cam;

    private void Start()
    {
        cam = FindObjectOfType<CameraController>();
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        playerWantsToAttack = Input.GetButton("Fire1");
        playerWantsToAim = Input.GetButton("Fire2");
        cooldownScan -= Time.deltaTime;
        cooldownPickTarget -= Time.deltaTime;
        cooldownAttack -= Time.deltaTime;

        if (playerWantsToAim) // We Will Scan the Environment, do it every half or quarter to help performance
        {

            if (target != null)
            {
                // turn towards it

                Vector3 toTarget = target.transform.position - transform.position;
                toTarget.y = 0;

                if (toTarget.magnitude > 3 && !CanSeeThing(target))
                {
                    target = null;
                }
            }
            if (cooldownScan <= 0) scanForTargets(); // Scans for targets
            if (cooldownPickTarget <= 0) PickATarget(); //Calls for a new target to be picked
        }
        else
        {
            target = null;
        }

        if (boneShoulderLeft) boneShoulderLeft.target = target ? target.transform : null;
        if (boneShoulderRight) boneShoulderRight.target = target ? target.transform : null;

        DoAttack();
    }
void DoAttack()
    {
        if (cooldownAttack > 0) return;
        if (!playerWantsToAim) return;
        if (!playerWantsToAttack) return;
        if (target == null) return;
        if (!CanSeeThing(target)) return;

        cooldownAttack = 1f / roundsPerSecond;

        // spawn projectiles...
        // or take health away from target

        boneShoulderLeft.transform.localEulerAngles += new Vector3(-30, 0, 0);
        boneShoulderRight.transform.localEulerAngles += new Vector3(-30, 0, 0);

    

        if (cam) cam.Shake(.25f);

    }
    void scanForTargets()
    {
        cooldownScan = .5f;

        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>(); // Find all Targetable Objects, returns an array list of each object Find Objects, not object

        foreach (TargetableObject thing in things)
        {
            if (CanSeeThing(thing))
            {
                validTargets.Add(thing);
            }

        }

    }
    void PickATarget() // What is the best object to pick? Answer: The closest one. For now.
    {
        if (target) return;

        float closestDistanceSoFar = 0;

        foreach (TargetableObject thing in validTargets)
        {
            Vector3 vToThing = thing.transform.position - transform.position;

            float dis = vToThing.sqrMagnitude;

            if (dis < closestDistanceSoFar || target == null)
            {
                closestDistanceSoFar = dis;
                target = thing;
            }
        }


    }

    private bool CanSeeThing(TargetableObject thing)
    {
        if (thing == null) return false;

        Vector3 vToThing = thing.transform.position - transform.position; // "Thing" refers to the object that's being targeted

        // is too far to see?
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false;

        // how much is in-front of player?
        float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

        // is within so-many degrees of forward
        if (alignment < .4f)
        {
            return false;
        }
        // check for occlusion...
        Ray ray = new Ray();

        ray.origin = transform.position;
        ray.direction = vToThing;

        RaycastHit hit;


        Debug.DrawRay(ray.origin, ray.direction * visionDistance, Color.red);

        if (Physics.Raycast(ray, out hit, visionDistance)) {

            bool canSee = false;
            Transform xform = hit.transform;

            do
            {
                if (xform.gameObject == thing.gameObject)
                {
                    canSee = true;
                    break;
                }
                xform = xform.parent;
            } while (xform != null);

            if (!canSee) return false;
        }
        return true;
    }
}
            

