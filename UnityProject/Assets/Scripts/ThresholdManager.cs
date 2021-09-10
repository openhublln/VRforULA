/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel
 * UCLouvain, EPL
 * academic year 2019-2020
 */
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO.Ports; // arduino
using MyFunc = MyFunctions.SharedFunction;      // See MyFunctions.cs


public class ThresholdManager : MonoBehaviour
{
    public Slider sliderCommandEMG0, sliderCommandEMG1;         // Slider for adjusting threshold (input)
    public Slider sliderCommandEMG0Max, sliderCommandEMG1Max;   // Slider for adjusting signal max value (input)
    public Slider currentEMG0, currentEMG1;                     // Slider displaying current emg values (output)
    public Slider currentEMG0threshold, currentEMG1threshold;   // Slider displaying current threshold values (output)
    public Slider currentEMG0max, currentEMG1max;               // Slider displaying current max signal values (output)

    public Text currentEMG0thresholdText, currentEMG1thresholdText;// Text displaying current threshold value (output)
    public Text currentEMG0MaxText, currentEMG1MaxText;         // Text displaying current max signal value (output)
    public Button EMG0threshold, EMG1threshold;                 // Indicate if threshold is over (output)
    public GameObject EMG0User, EMG1User;                       // Panels for passive display

    public float emg0Threshold = 1.3f, emg1Threshold = 1.3f;    // Default threshold values
    public float emg0Max = 5.0f, emg1Max = 5.0f;                // Default max signal values
    private float emg0 = 0, emg1 = 0;                           // Current emg voltage [0 - 5]
    private SerialPort sp = new SerialPort("COM4", 9600);       // Serial port, connected to the arduino controller /dev/tty.usbmodem14203


    void Start() // Start is called before the first frame update
    {
        sp.Open();
        sp.ReadTimeout = 25; // set time before time out exception. Pay attention: 1ms is too short for receiving a 32-bit message.
        
        // Get user parameters
        emg0Threshold = PlayerPrefs.GetFloat("EMG0Threshold");
        emg1Threshold = PlayerPrefs.GetFloat("EMG1Threshold");
        emg0Max = PlayerPrefs.GetFloat("EMG0Max");
        emg1Max = PlayerPrefs.GetFloat("EMG1Max"); 
    }  
    void Update() // Update is called once per frame
    {    
        float[] data = MyFunc.GetMessage(sp);   // data: [ableToRead, emg0, emg1, button0, button1]
        if (data[0] != 0)                       // if able to read serial port
        {
            emg0 = data[1];
            emg1 = data[2];
        }        

        EMG0AdjustDisplay(emg0, emg0Threshold, emg0Max);
        EMG1AdjustDisplay(emg1, emg1Threshold, emg1Max);
        MyFunc.EMGAdjustDisplay(emg0, emg0Threshold, EMG0User);
        MyFunc.EMGAdjustDisplay(emg1, emg1Threshold, EMG1User);
    }

    // Following functions are called when sliders are manually adjusted.
    public void AdjustEMG0slider(float newThreshold)
    {
        emg0Threshold = newThreshold;
    }

    public void AdjustEMG1slider(float newThreshold)
    {
        emg1Threshold = newThreshold;
    }

    public void AdjustEMG1MaxSlider(float newThreshold)
    {
        emg1Max = newThreshold;
    }

    public void AdjustEMG0MaxSlider(float newThreshold)
    {
        emg0Max = newThreshold;
    }


    // Following functions are called when some text is written in the text input fields.
    public void AdjustEMG0text(string newText)
    {
        try
        {
            emg0Threshold = float.Parse(newText);
        }
        catch
        {
            print("Format not respected! ");
        }
    }

    public void AdjustEMG1text(string newText)
    {
        try 
        { 
            emg1Threshold = float.Parse(newText);
        }
        catch 
        {
            print("Format not respected! ");
        }
    }

    public void AdjustEMG1Maxtext(string newText)
    {
        try
        {
            emg1Max = float.Parse(newText);
        }
        catch
        {
            print("Format not respected! ");
        }
    }

    public void AdjustEMG0Maxtext(string newText)
        /**
         * Get 
         */
    {
        try
        {
            emg0Max = float.Parse(newText);
        }
        catch
        {
            print("Format not respected! ");
        }
    }

    public void Confirm()
    /** Save the 4 values when confirm button is pressed */
    {
        PlayerPrefs.SetFloat("EMG0Threshold", emg0Threshold);
        PlayerPrefs.SetFloat("EMG1Threshold", emg1Threshold);
        PlayerPrefs.SetFloat("EMG0Max", emg0Max);
        PlayerPrefs.SetFloat("EMG1Max", emg1Max);
    }
    public void RetourMenu()
    /** Load the scene "MainMenu when the button "quitter" is pressed */
    {
        SceneManager.LoadScene("MainMenu");
    }

    void EMG0AdjustDisplay(float currentEMG, float currentThreshold, float currentMax)
    /** Adjust the EMG0 gauges of the operator */
    {
        currentEMG0threshold.value = currentThreshold;
        currentEMG0thresholdText.text = currentThreshold.ToString("f2");
        currentEMG0MaxText.text = currentMax.ToString("f2");
        currentEMG0.value = currentEMG;
        currentEMG0max.value = currentMax;
        if (sliderCommandEMG0.value != currentThreshold)
            sliderCommandEMG0.value = currentThreshold;
        if (sliderCommandEMG0Max.value != currentMax)
            sliderCommandEMG0Max.value = currentMax;

        if (currentEMG >= currentThreshold)
        {
            EMG0threshold.GetComponent<Image>().color = Color.green;
        }
        else
        {
            EMG0threshold.GetComponent<Image>().color = Color.white;
        }
    }

    void EMG1AdjustDisplay(float currentEMG, float currentThreshold, float currentMax)
    /** Adjust the EMG1 gauges of the operator */
    {
        currentEMG1threshold.value = currentThreshold;
        currentEMG1thresholdText.text = currentThreshold.ToString("f2");
        currentEMG1MaxText.text = currentMax.ToString("f2");
        currentEMG1.value = currentEMG;
        currentEMG1max.value = currentMax;
        if (sliderCommandEMG1.value != currentThreshold)
            sliderCommandEMG1.value = currentThreshold;
        if (sliderCommandEMG1Max.value != currentMax)
            sliderCommandEMG1Max.value = currentMax;
        if (currentEMG >= currentThreshold)
        {
            EMG1threshold.GetComponent<Image>().color = Color.green;
        }
        else
        {
            EMG1threshold.GetComponent<Image>().color = Color.white;
        }
    }
}
