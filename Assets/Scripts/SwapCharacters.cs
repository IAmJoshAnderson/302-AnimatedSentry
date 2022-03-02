using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCharacters : MonoBehaviour
{

    public GameObject player;
    public GameObject sentry;
    public GameObject camera1;
    public GameObject camera2;

    void Start()
    {
        player.SetActive(true);
        camera1.SetActive(true);
        sentry.SetActive(false);
        camera2.SetActive(false);
    }

   void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)){
            player.SetActive(true);
            camera1.SetActive(true);
            sentry.SetActive(false);
            camera2.SetActive(false);
            print("R");
        }
        if (Input.GetKeyDown(KeyCode.T)){
            player.SetActive(false);
            camera1.SetActive(false);
            sentry.SetActive(true);
            camera2.SetActive(true);
            print("T");
        }

    }
}