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

public class PinceRotation : MonoBehaviour
{
    public GameObject EMG0User, EMG1User;                   // EMG gauges for user
    public GameObject EMG0Operator, EMG1Operator;           // EMG gauges for operator
    public GameObject Prosthesis;                           // The prosthesis GO itself
    public float CloseSpeed = 25;                           // Clamp closing speed
    public float OpenSpeed = 50;                            // Clamp opening speed
    public float MinStrength = 0, MaxStrength = 10;         // Scale the EMG inensity

    public static readonly SerialPort sp = new SerialPort("COM4", 9600);// Serial port, connected to the arduino controller
    private float emg0, emg1;                               // EMG voltage
    private GameObject Left, LeftDefault;                   // Prostehsis body GO references
    private GameObject Right, RightDefault;                 // Prostehsis body GO references

    // User parameters
    private float emg0Threshold, emg1Threshold;             // User emg thresholds
    private float emg0Max, emg1Max;                         // User signal max values
    private bool CcCounter = true;                          // Limit cocontraction counter

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

        // Get game object hierarchy references
        Left = Prosthesis.transform.GetChild(1).gameObject;
        LeftDefault = Left.transform.GetChild(0).gameObject;
        Right = Prosthesis.transform.GetChild(2).gameObject;
        RightDefault = Right.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        float[] data = MyFunc.GetMessage(sp);                           // Data: [ableToRead, emg0, emg1, button0, button1]
        if (data[0] != 0) { emg0 = data[1]; emg1 = data[2]; }           // If able to read serial port, update emg values
        MyFunc.EMGAdjustDisplay(emg0, emg0Threshold, EMG0User);         // Update the user emg gauges on the dashboard
        MyFunc.EMGAdjustDisplay(emg1, emg1Threshold, EMG1User);
        MyFunc.EMGAdjustDisplay(emg0, emg0Threshold, EMG0Operator);     // Update the operator emg gauges on the dashboard
        MyFunc.EMGAdjustDisplay(emg1, emg1Threshold, EMG1Operator);

        ActivationMovement(emg0, emg1);
        CorrectAngle(125, 182); // min = 125, max = 182 deg

    }

    void ActivationMovement(float msg0, float msg1)
    /** Manage opening and closing of the prosthesis */
    {
        float strength0 = MyPros.Remap(msg0, emg0Threshold, emg0Max, MinStrength, MaxStrength);
        float strength1 = MyPros.Remap(msg1, emg0Threshold, emg1Max, MinStrength, MaxStrength);
        float CurrentOpeningAngle = LeftDefault.transform.localRotation.eulerAngles.z;
        if ((msg0 > emg0Threshold) && (msg1 <= emg1Threshold) && (CurrentOpeningAngle < 182))
        {
            CloseClamp(strength0);
            CcCounter = true;
        }
        if ((msg1 > emg1Threshold) && (msg0 <= emg0Threshold) && (CurrentOpeningAngle > 125))
        {
            OpenClamp(strength1);
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

    void CloseClamp(float strength)
    /** Close the clamp */
    {
        LeftDefault.transform.Rotate(Vector3.forward, strength * CloseSpeed * Time.deltaTime);
        RightDefault.transform.Rotate(Vector3.forward, strength * CloseSpeed * Time.deltaTime);
    }

    void OpenClamp(float strength)
    /** Open the clamp */
    {
        LeftDefault.transform.Rotate(Vector3.forward, -strength * OpenSpeed * Time.deltaTime);
        RightDefault.transform.Rotate(Vector3.forward, -strength * OpenSpeed * Time.deltaTime);
    }

    void CorrectAngle(float minAngle, float maxAngle)
    /** Force the opening angle of the clamp if it exceeds the realistic values */
    {
        Vector3 currentRotationL = LeftDefault.transform.localRotation.eulerAngles;
        Vector3 currentRotationR = RightDefault.transform.localRotation.eulerAngles;
        if (currentRotationL.z <= minAngle)
        {
            currentRotationL.z = minAngle - 1;
            currentRotationR.z = -180 + minAngle - 1;

        }
        if (currentRotationL.z >= maxAngle)
        {
            currentRotationL.z = maxAngle + 1;
            currentRotationR.z = -180 + maxAngle + 1;
        }
        LeftDefault.transform.localRotation = Quaternion.Euler(currentRotationL);
        RightDefault.transform.localRotation = Quaternion.Euler(currentRotationR);
    }
}