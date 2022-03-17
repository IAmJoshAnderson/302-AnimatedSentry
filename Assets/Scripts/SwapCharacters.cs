using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCharacters : MonoBehaviour
{

    public GameObject player;
    public GameObject sentry;
    public GameObject camera1;
    public GameObject camera2;
    public static bool player1 = true;
    public static bool player2 = false;

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
            player1 = true;
            player2 = false;
        }
        if (Input.GetKeyDown(KeyCode.T)){
            player.SetActive(false);
            camera1.SetActive(false);
            sentry.SetActive(true);
            camera2.SetActive(true);
            player2 = true;
            player1 = false;
        }

    }
}