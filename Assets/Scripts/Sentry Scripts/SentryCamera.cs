using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryCamera : MonoBehaviour
{

    public SentryTargeting sentry;

    public Vector3 targetOffset;

    public float mouseSensitivityX = 5;
    public float mouseSensitivityY = 5;
    public float mouseSensitivityScroll = 5;


    private Camera cam;

    private float pitch = 0;
    private float yaw = 0;
    private float dollyDis = 10;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        if (sentry == null)
        {
            SentryTargeting script = FindObjectOfType<SentryTargeting>();
            if (script != null) sentry = script;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isAiming = (sentry && sentry.target && sentry.sentryWantsToAim);


        if (sentry)
        {
            transform.position = AnimMath.Ease(transform.position, sentry.transform.position + targetOffset, .01f);
        }

        float playerYaw = AnimMath.AngleWrapDegrees(yaw, sentry.transform.eulerAngles.y);

        if (isAiming)
        {
            Quaternion tempTarget = Quaternion.Euler(0, playerYaw, 0);
            transform.rotation = AnimMath.Ease(transform.rotation, tempTarget, .001f);
        }
        else
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            yaw += mx * mouseSensitivityX;
            pitch += my * mouseSensitivityY;

            pitch = Mathf.Clamp(pitch, -10, 60);
            transform.rotation = AnimMath.Ease(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
        }

        dollyDis += Input.mouseScrollDelta.y * mouseSensitivityScroll;
        dollyDis = Mathf.Clamp(dollyDis, 3, 20);

        float tempZ = isAiming ? 2 : dollyDis;

        cam.transform.localPosition = AnimMath.Ease(cam.transform.localPosition, new Vector3(0, 0, -tempZ), .02f);

        if (isAiming)
        {
            Vector3 vToAimTarget = sentry.target.transform.position - cam.transform.position;
            Quaternion worldRot = Quaternion.LookRotation(vToAimTarget);

            Quaternion localRot = worldRot;

            if (cam.transform.parent)
            {
                localRot = Quaternion.Inverse(cam.transform.parent.rotation) * worldRot;
            }

            Vector3 euler = localRot.eulerAngles;
            euler.z = 0;
            localRot.eulerAngles = euler;

            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, localRot, .001f);
        }
        else
        {
            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, Quaternion.identity, .001f);
        }
    }
    private void OnDrawGizmos()
    {
        if (!cam) cam = GetComponentInChildren<Camera>();
        if (!cam) return;

        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.DrawLine(transform.position, cam.transform.position);
    }
}
