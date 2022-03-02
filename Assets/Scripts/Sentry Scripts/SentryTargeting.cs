using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTargeting : MonoBehaviour
{

    public float sentryVision = 20;


    [Range(1, 50)]
    public float roundsPerSecondSentry = 50;

    public PointAt chamber;

    public TargetableObject target { get; private set; }
    public bool sentryWantsToAim { get; private set; }

    public bool sentryWantsToAttack { get; private set; }

    private List<TargetableObject> validTargets = new List<TargetableObject>(); // An array list of objects possible to be targeted.
    private float cooldownScan = 0;
    private float cooldownPickTarget = 0;
    private float cooldownAttack = 0;

    private CameraController cam;

    // Start is called before the first frame update
    private void Start()
    {
        cam = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        sentryWantsToAttack = Input.GetButton("Fire1");
        sentryWantsToAim = Input.GetButton("Fire2");
        cooldownScan -= Time.deltaTime;
        cooldownPickTarget -= Time.deltaTime;
        cooldownAttack -= Time.deltaTime;

        if (sentryWantsToAim) // We Will Scan the Environment, do it every half or quarter to help performance
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
            if (cooldownScan <= 0) ScanForTargets();
            if (cooldownPickTarget <= 0) PickATarget();
        }
        else
        {
            target = null;
        }

        if (chamber) chamber.target = target ? target.transform : null;
        DoAttack();
    }
    void DoAttack()
    {
        float spin = 0;

        if (cooldownAttack > 0) return;
        if (!sentryWantsToAim) return;
        if (!sentryWantsToAttack) return;
        if (target == null) return;
        if (!CanSeeThing(target)) return;

        cooldownAttack = 1f / roundsPerSecondSentry;

        //spawn projectiles...
        // or take health away from target...

        if (spin < 0) return;
        spin -= Time.deltaTime;

        float p = spin / 1;
        p = p * p;
        p = AnimMath.Lerp(1, .98f, p);

        chamber.transform.localEulerAngles += new Vector3(0, p, 0);

        print("Hello!");

    }
    void ScanForTargets()
    {
        cooldownScan = .5f;

        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();

        foreach (TargetableObject thing in things)
        {
            if (CanSeeThing(thing))
            {
                validTargets.Add(thing);
            }
        }    
    }
    void PickATarget()
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

        Vector3 vToThing = thing.transform.position - transform.position;


        if (vToThing.sqrMagnitude > sentryVision * sentryVision) return false;


        float alignment = Vector3.Dot(transform.forward, vToThing.normalized);


        if (alignment < .4f)
        {
            return false;
        }


        Ray ray = new Ray();

        ray.origin = transform.position;
        ray.direction = vToThing;

        RaycastHit hit;


        Debug.DrawRay(ray.origin, ray.direction * sentryVision, Color.red);

        if (Physics.Raycast(ray, out hit, sentryVision))
        {
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
