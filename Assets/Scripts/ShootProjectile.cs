using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public Transform pos1;
    public LineRenderer line;
    public Transform pos2;
    public Transform pos3;
    public GameObject bullet;


    PointAt ray;
    PointAtSentry raySentry;

    public SwapCharacters swap { get; private set; }

    // speed
    public float shootForce, upwardForce;

    //Stats
    public float timeBetweenShooting, spread, timeBetweenShots;

    //We going to shoot or not
    bool shooting, readyToShoot;

    public bool allowInvoke = true; // just for bugs

    public Transform attackPoint;

    // Distance the camera can see the player
    public float visionDistance = 15;

    private float decayTime = 3; // amount of time the bullet should stay on screen

    private List<PlayerObject> validTargets = new List<PlayerObject>();

    // Start is called before the first frame update
    void Start()
    {
        line.positionCount = 2;
        ray = GetComponent<PointAt>();
        raySentry = GetComponent<PointAtSentry>();

    }

    // Update is called once per frame
    void Update()
    {
      scanForTargets();
        MyInput();
    }
    void scanForTargets()
    {
        // cooldownScan = .5f;

        validTargets.Clear();

        PlayerObject[] things = GameObject.FindObjectsOfType<PlayerObject>();

        foreach (PlayerObject thing in things)
        {
            if (CanSeeThing(thing))
            {
                validTargets.Add(thing);
            }
        }
    }

    private void Awake() // only should happen when the player is close enough
    {
            readyToShoot = true;
    }
    private void MyInput()
    {
            shooting = true;

       if (readyToShoot && shooting && visionDistance <= 15)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;       

        // We need to create a ray that goes from the origin to the player.

        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = (pos2.transform.position - ray.origin).normalized;

        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * visionDistance, Color.red);

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, visionDistance))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); // Just a point far away from the player

        // spread of the bullets
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = targetPoint - attackPoint.position;

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); // store instantiated bullet

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(ray.direction * upwardForce, ForceMode.Impulse);

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }


    private bool CanSeeThing(PlayerObject thing)
    {
        if (thing == null)  return false;

        Vector3 vToThing = thing.transform.position - transform.position;

        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false;

        float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

        if (alignment < .4f)
        {
            return false;
        }

        //Creates the laser focus line

        if (thing != null)
        {
            line.SetPosition(0, pos1.position);
            if (SwapCharacters.player1 == true)
            {
                line.SetPosition(1, pos2.position);
            }
            if (SwapCharacters.player2 == true)
            {
                line.SetPosition(1, pos3.position);
            }
                readyToShoot = true;
        }

        return true;
    }
    
}
