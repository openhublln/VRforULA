/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel
 * UCLouvain, EPL
 * academic year 2019-2020
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalTime: MonoBehaviour
{

    //public Transform player;
    public Text timerText;
    private float startTime;
    private bool finnished = false;
    


    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (finnished)
        {
            return;
        }
        float t = (Time.time - startTime);
        if(t < 0)
        {
            return;
        }
        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f0");

        timerText.text = minutes + " : " + seconds;
    }

    public void Finnish()
    {
        timerText.color = Color.yellow;
    }
}
