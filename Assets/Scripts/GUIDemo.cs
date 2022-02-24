using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; //Recognizes Slider Class from Time Demo

public class GUIDemo : MonoBehaviour
{

    public GameObject camera1;
    public GameObject camera2;


    // Start is called before the first frame update
    void Start()
    {
        camera1.SetActive(true);
        camera2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnableCamera1()
    {
        camera1.SetActive(true);
        camera2.SetActive(false);
    }

    public void EnableCamera2()
    {
        camera1.SetActive(false);
        camera2.SetActive(true);
    }
}
