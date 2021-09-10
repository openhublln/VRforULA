/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel &  Ange Muhoza & Lucas El Raghibi
 * UCLouvain, EPL
 * academic year 2019-2021
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using MyFunc = MyFunctions.SharedFunction;      // See MyFunctions.cs
using MyPros = MyFunctions.ProsthesisFunction;  // See MyFunctions.cs

public class GreiferScript : MonoBehaviour
{
    public GameObject EMG0User, EMG1User;                   // EMG gauges for operator(!)
    public GameObject ExplanationSlides, TaskSlides;        // Game Object (GO) containing all the slides to display
    public GameObject Prosthesis;                                 // The prosthesis GO itself
    public Detector Wrist, ButtonForeArm, RightTouch, LeftTouch; // Detectors (Wrist, Forearm button, touch pads of the clamp)
    public Button ExplanationButton, TaskButton;            // Operator buttons
    public float CloseSpeed = 25;                           // Clamp closing speed
    public float OpenSpeed = 50;                            // Clamp opening speed
    public float rotationSpeed = 50;                        // Wrist rotation speed
    public float MinStrength = 0, MaxStrength = 10;         // Scale the EMG inensity

    private readonly SerialPort sp = new SerialPort("COM4", 9600);// Serial port, connected to the arduino controller
    private float emg0, emg1;                               // EMG voltage
    private GameObject Left, LeftDefault;                   // Prostehsis body GO references
    private GameObject Right, RightDefault;                 // Prostehsis body GO references
    private GameObject TouchedObjL, TouchedObjR, TouchedObj;// Grapsed GO reference
    private bool canHold = true;                            // Able to catch an object
    private bool isHolding = false;                         // Is holding the object
    private bool prosthesisActivated;                       // Prosthesis state: On/Off
    private bool ClampTouched;                              // Both tips of the clamp are simultaneously touched
    private int buttonState, previousButtonState = 0;       // Forearm button state

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

        // Manage buttons for displaying instruction slides: attach the function to the button
        ExplanationButton.onClick.AddListener(ActivateExplanation); 
        TaskButton.onClick.AddListener(ActivateTask);          

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
        MyFunc.EMGAdjustDisplay(emg0, emg0Threshold, EMG0User);         // Update the emg gauges on the dashboard
        MyFunc.EMGAdjustDisplay(emg1, emg1Threshold, EMG1User);

        ClampTouched = (RightTouch.entered && LeftTouch.entered);       //check if the tips of the clamp are both touching something
        TouchedObjR = RightTouch.culprit_object;                        // Get the GO touched by the right detector
        TouchedObjL = LeftTouch.culprit_object;                         // Get the GO touched by the left detector
        buttonState = MyPros.CheckButton(ButtonForeArm);                // Forearm button
        //prosthesisActivated = MyPros.buttonSwitch(buttonState, previousButtonState, prosthesisActivated); // Uncomment to require to use the forearm button to power on/off the prosthesis.
        prosthesisActivated = true;                                     // Comment if previous line is uncommented
       
        if (prosthesisActivated)
        {
            // MOVEMENTS
            ActivationMovement(emg0, emg1, ClampTouched);
            CorrectAngle(125, 182); // min = 125, max = 182 deg
            ManageGraspedObject(ClampTouched, TouchedObjL, TouchedObjR);
            RotateWrist(Wrist.entered);
        }

        CocontractionCounter(emg0, emg1); // Update cocontraction counter

        // UPDATE variables
        previousButtonState = buttonState;
    }

    void ActivationMovement(float msg0, float msg1, bool ClampTouched)
    /** Manage opening and closing of the prosthesis */
    {
        float strength0 = MyPros.Remap(msg0, emg0Threshold, emg0Max, MinStrength, MaxStrength);
        float strength1 = MyPros.Remap(msg1, emg0Threshold, emg1Max, MinStrength, MaxStrength);
        float CurrentOpeningAngle = LeftDefault.transform.localRotation.eulerAngles.z;
        if ((msg0 > emg0Threshold) && (msg1 <= emg1Threshold) && (ClampTouched==false) && (CurrentOpeningAngle < 182))
        {
            CloseClamp(strength0);     
        }     
        if ((msg1 > emg1Threshold) && (msg0 <= emg0Threshold) && (CurrentOpeningAngle > 125))
        { 
            OpenClamp(strength1);
        }
    }

    void RotateWrist(bool Allowed)
    /** Rotate the wrist if required */
    {
        if (Allowed) // Manual rotation of the wrist
        {
            if (Wrist.culprit_object.tag == "GameController") // rotation of the wrist
            {
                Prosthesis.transform.Rotate(Vector3.right, MaxStrength * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
    }

    void ManageGraspedObject(bool ClampTouched, GameObject TouchedObjL, GameObject TouchedObjR)
    /** If an object is stucked between the two tips of the clamp, the object should be caught. */
    {
        if (ClampTouched && TouchedObjL == TouchedObjR && TouchedObjL.tag == "test_object")
        {
            canHold = true;
            TouchedObj = TouchedObjL;
            MyPros.AttachObject(TouchedObj, Prosthesis);
            isHolding = true;
        }
        else
        {
            canHold = false;
        }

        if (canHold == false && isHolding == true)
        {
            MyPros.ReleaseObject(TouchedObj);
            isHolding = false;
            TouchedObj = null;
        }
    }

    void ActivationWrist(float msg0, float msg1)
    /** Manage wrist rotation according to the emg intensity. 
     *  Currently not used.
     */
    {
        float strength0 = MyPros.Remap(msg0, emg0Threshold, emg0Max, MinStrength, MaxStrength);
        float strength1 = MyPros.Remap(msg1, emg0Threshold, emg1Max, MinStrength, MaxStrength);
        if ((msg0 > emg0Threshold) && (msg1 <= emg1Threshold)) // Rotate Clockwise
        {
            Prosthesis.transform.Rotate(Vector3.right, strength0 * rotationSpeed * Time.deltaTime, Space.Self);
        }
        if ((msg1 > emg1Threshold) && (msg0 <= emg0Threshold)) // Rotate Counter-Clockwise
        {
            Prosthesis.transform.Rotate(Vector3.right, strength1 * -rotationSpeed * Time.deltaTime, Space.Self);
        }
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
            currentRotationR.z = -180+ maxAngle + 1;
        }
        LeftDefault.transform.localRotation = Quaternion.Euler(currentRotationL);
        RightDefault.transform.localRotation = Quaternion.Euler(currentRotationR);
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

    void CocontractionCounter(float msg0, float msg1)
    /** Count the number of cocontraction of the subject */
    {
        if ((msg0 > emg0Threshold) && (msg1 <= emg1Threshold))
        {
            CcCounter = true;
        }
        if ((msg1 > emg1Threshold) && (msg0 <= emg0Threshold))
        {
            CcCounter = true;
        }
        if ((msg1 >= emg1Threshold) && (msg0 >= emg0Threshold))
        {
            if (CcCounter)
            {
                KeepCocontraction.countCocontraction++;
                CcCounter = false;
            }
        }
    }

    public void ActivateExplanation()
    /** Attach to the button "Explanation"
      *  Deactivate the task slides and activate the explanation slides.
      */
    {
        TaskSlides.SetActive(false);
        ExplanationSlides.SetActive(true);
    }

    public void ActivateTask()
    /** Attach to the button "Tasks"
      * Deactivate the explanation slides and activate the task slides.
      */
    {
        ExplanationSlides.SetActive(false);
        TaskSlides.SetActive(true);
    }
}