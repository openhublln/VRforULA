/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Ange Muhoza & Lucas El Raghibi
 * UCLouvain, EPL
 * academic year 2020-2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using MyFunc = MyFunctions.SharedFunction;      // See MyFunctions.cs
using MyPros = MyFunctions.ProsthesisFunction;  // See MyFunctions.cs

public class CubeRotation : MonoBehaviour
{
    public GameObject EMG0User, EMG1User;                   // EMG gauges for user
    public GameObject EMG0Operator, EMG1Operator;           // EMG gauges for operator
    public float RotateSpeed = 30f;                         // Cube rotation speed
    public float MinStrength = 0, MaxStrength = 10;         // Scale the EMG inensity

    public static readonly SerialPort sp = new SerialPort("COM4", 9600);// Serial port, connected to the arduino controller
    private float emg0, emg1;                               // EMG voltage

    // User parameters
    private float emg0Threshold, emg1Threshold;             // User emg thresholds
    private float emg0Max, emg1Max;                         // User signal max values
    private bool CcCounter = true;                          // Limit cocontraction counter
    public static bool closeMinigame = false;
    void Start()
    {
        // Manage serial port
        sp.Open();
        sp.ReadTimeout = 20; // set time before time out exception. Pay attention: 1ms is too short for receiving a 32-bit message

        // Get player parameters
        emg0Threshold = PlayerPrefs.GetFloat("EMG0Threshold");
        emg1Threshold = PlayerPrefs.GetFloat("EMG1Threshold");
        emg0Max = PlayerPrefs.GetFloat("EMG0Max");
        emg1Max = PlayerPrefs.GetFloat("EMG1Max");
    }

   
    void Update()
    {
        float[] data = MyFunc.GetMessage(sp);                           // Data: [ableToRead, emg0, emg1, button0, button1]
        if (data[0] != 0) { emg0 = data[1]; emg1 = data[2]; }           // If able to read serial port, update emg values
        MyFunc.EMGAdjustDisplay(emg0, emg0Threshold, EMG0User);         // Update the emg gauges on the dashboard
        MyFunc.EMGAdjustDisplay(emg1, emg1Threshold, EMG1User);
        MyFunc.EMGAdjustDisplay(emg0, emg0Threshold, EMG0Operator);     // Update the operator emg gauges on the dashboard
        MyFunc.EMGAdjustDisplay(emg1, emg1Threshold, EMG1Operator);

        float strength0 = MyPros.Remap(emg0, emg0Threshold, emg0Max, MinStrength, MaxStrength);
        float strength1 = MyPros.Remap(emg1, emg0Threshold, emg1Max, MinStrength, MaxStrength);
        if ((emg0 > emg0Threshold) && (emg1 <= emg1Threshold))
        {
            transform.Rotate(Vector3.up, strength0 * RotateSpeed * Time.deltaTime);
            CcCounter = true;
        }     
        if ((emg1 > emg1Threshold) && (emg0 <= emg0Threshold))
        { 
            transform.Rotate(Vector3.up, -strength1 * RotateSpeed * Time.deltaTime);
            CcCounter = true;
        }
        if ((emg1 >= emg1Threshold) && (emg0 >= emg0Threshold))
        {
            if (CcCounter)
            {
                KeepCocontraction.countCocontraction++;
                CcCounter = false;
            }
        }
    }
}