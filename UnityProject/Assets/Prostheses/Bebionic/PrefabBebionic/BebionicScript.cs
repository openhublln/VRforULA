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

public class BebionicScript : MonoBehaviour
{
    public GameObject EMG0User, EMG1User;                   // Game Object (GO) for displaying emg gauges in the dashboard
    public GameObject ExplanationSlides, TaskSlides;        // GO containing all the slides
    public GameObject ForeArm, Prosthesis, Finger1_1,       // Prosthesis GO references
                      Finger1_2, Finger2_1, Finger2_2, 
                      Finger3_1, Finger3_2, Finger4_1, 
                      Finger4_2, Thumb_1, Thumb_2;
    public Button ExplanationButton, TaskButton;            // Button references in the dashboard
    public Material ButtonMaterial;                         // Material of the button of the prosthesis
    public Detector Touch1, Touch2, Touch3, Touch4, TouchThumb;// Detector of each finger
    public float CloseSpeed = 25;                           // Hand closing speed
    public float OpenSpeed = 50;                            // Hand opening speed
    public float rotationSpeed = 50;                        // Default rotation speed, used for the rotation of the thumb
    public float MinStrength = 0, MaxStrength = 10;         // Scale the EMG inensity

    private readonly SerialPort sp = new SerialPort("COM4", 9600);// Serial port, connected to the arduino controller
    private float emg0, emg1;                               // EMG voltage
    private GameObject TouchedObj;                          // Reference of the touched GO
    private Animator _ThumbAnimation;                       // Animation for the thumb 
    private Detector[] TouchPadArray;                       // Container for all the detectors
    private List<GameObject> ObjectList;                    // Container for all the body of the prosthesis
    List<List<float>> AngleList = new List<List<float>>();  // List of the list of the limit angles
    private bool OppositionFlag = false;                    // True when the thumb is in opposition, false when it is in lateral position
    private bool FlagRotationThumb = false;                 // True when Thumb1 is rotating during the opposition/non-opposition transititions
    private bool ButtonFlag = false;                        // Changes its state when the button (located on the top of the hand) is pressed
    private int PreviousButtonState = 0, PreviousButtonStateThumb = 0;
    private int Thumb1Index;                                // Thumb index reference (see start method)
    public Detector ButtonTopHand, ButtonThumb;             // Button detectors

    //Variable to handle switch between grips (double opening signal) 
    private bool FlagIdle = true;                           // True if Idle position
    private bool firstSignalEmitted = false;                // True if the first signal is emitted                        
    private float timeOfFirstSignal;                        // Time when first signal was recorded
    private float PreviousEmg1 = 0;                         // Previous emg1 value 
    private int GripSet = 0;                                // 0 (for primary movements: Grip 0, 2, 4, and 6) and 1 (for secondary movements: Grip 1, 3, 5 and 7)

    //Grasping object
    private bool isHolding = false;                         // True if the hand is currenctly holding an object
    private bool canHold = true;                            // True if the hand is ready to catch an object

    //Slide
    public GameObject Felicitation;                         // "felicitation" display
    public GameObject GroupFile;                            // Slides displaying the current group position
    [HideInInspector] public int GroupFileSize;             // Number of "group" slides

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
        DisplayGroupNumber(1);          // Initiates group display
        Felicitation.SetActive(false);  // Ensure "Felicitation" deactivation

        // Get and group GO references
        TouchPadArray = new Detector[5] {TouchThumb, Touch1, Touch2, Touch3, Touch4};
        ObjectList = new List<GameObject> {Finger1_1, Finger1_2, Finger2_1, Finger2_2, Finger3_1, Finger3_2, Finger4_1, Finger4_2, Thumb_1, Thumb_2};
        
        // Others parameters
        Thumb1Index = ObjectList.Count - 2;
        _ThumbAnimation = GetComponent<Animator>();
        GroupFileSize = GroupFile.transform.childCount;
        
        //LATERAL THUMB
        //Grip 0: Prise latérale
        AngleList.Add(new List<float> { 70.0f, 80.0f, 70.0f, 80.0f, 70.0f, 80.0f, 70.0f, 80.0f, 345.0f, 80.0f });
        //Grip 1: Index pointe 
        AngleList.Add(new List<float> { 5.0f, 5.0f, 80.0f, 80.0f, 80.0f, 80.0f, 80.0f, 80.0f, 340.0f, 80.0f });
        //Grip 2: Poussoir
        AngleList.Add(new List<float> { 50.0f, 90.0f, 90.0f, 90.0f, 90.0f, 90.0f, 90.0f, 90.0f, 350.0f, 80.0f });
        //Grip 3: Prise souris 
        AngleList.Add(new List<float> { 20.0f, 20.0f, 10.0f, 20.0f, 10.0f, 20.0f, 30.0f, 20.0f, 340.0f, 30.0f });

        //OPPOSITION THUMB
        //Grip 4: Pince tridigitale
        AngleList.Add(new List<float> { 50.0f, 80.0f, 50.0f, 80.0f, 105.0f, 90.0f, 105.0f, 90.0f, 300.0f, 70.0f });
        //Grip 5: Prise de force
        AngleList.Add(new List<float> { 20.0f, 90.0f, 20.0f, 90.0f, 20.0f, 90.0f, 20.0f, 90.0f, 330.0f, 30.0f });
        //Grip 6: Gachette
        AngleList.Add(new List<float> { 5.0f, 5.0f, 60.0f, 70.0f, 60.0f, 70.0f, 60.0f, 70.0f, 300.0f, 70.0f });
        //Grip 7: Pince tridigitale
        AngleList.Add(new List<float> { 50.0f, 80.0f, 50.0f, 80.0f, 105.0f, 90.0f, 105.0f, 90.0f, 300.0f, 70.0f });

        //COME BACK TO IDLE POSITION
        //Index 8: Come back for movements when lateral thumb
        AngleList.Add(new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 330.0f, 0.0f });
        //Index 9: Come back for movements when opposition thumb
        AngleList.Add(new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 290.0f, 0.0f });

        /*             |         Lateral Thumb            |          Opposition Thumb    
         *             |      OppositionFlag==false       |         OppositionFlag==true
         *-------------|----------------------------------|------------------------------------
         *             | GROUP1:     Grip 0               | GROUP3:      Grip 4
         * ButtonFlag  |                                  | 
         *  ==false    |             Grip 1               |              Grip 5
         * ------------|----------------------------------|------------------------------------
         *             | GROUP2:     Grip 2               | GROUP4:      Grip 6
         * ButtonFlag  |                                  |
         * ==true      |             Grip 3               |              Grip 7
         * ------------|----------------------------------|------------------------------------
         */       
    }

    void Update()
    {
        float[] data = MyFunc.GetMessage(sp);                                                       // Data: [ableToRead, emg0, emg1, button0, button1]
        if (data[0] != 0) { emg0 = data[1]; emg1 = data[2]; }                                        // If able to read serial port, update emg values
        MyFunc.EMGAdjustDisplay(emg0, emg0Threshold, EMG0User);                                     // Update the emg gauges on the dashboard
        MyFunc.EMGAdjustDisplay(emg1, emg1Threshold, EMG1User);

        int ButtonStateHand = MyPros.CheckButton(ButtonTopHand);                                    // Button (top hand) state: 0 or 1 (pushed)
        int ButtonStateThumb = MyPros.CheckButton(ButtonThumb, false);                              // Thumb touched state: 0 or 1 (pushed) 
        bool FingersTouched = ((Touch1.entered && TouchThumb.entered) || (Touch2.entered && TouchThumb.entered)); // check if the tips of at least one finger and the thumb are both touching something
        int Group = GiveGroup(OppositionFlag, ButtonFlag);                                          // Selection of the group: 1,2,3 or 4
        int Grip = GiveGrip(Group, GripSet);                                                        // Selection of the grip: 1,2,3,4,5,6,7 or 8
        int AimedGrip = ChangeSlide.slideIndex - 1;                                                 // Aimed grip, according to the task slide display
        MyPros.DisplayFelicitation(AimedGrip, Grip, AngleList, ObjectList, !FlagIdle, Felicitation);// Displays "felicitation"

        /*************************** Check commands ***************************/
        // THUMB TOUCHED, Change of group
        if (ButtonStateThumb == 1 && PreviousButtonStateThumb == 0 && FlagIdle)
        {
            OppositionFlag = !OppositionFlag;
            _ThumbAnimation.SetBool("ActivationOpposition", OppositionFlag); // Activate animation placing the thumb.
            FlagRotationThumb = true;
            ButtonFlag = false; //The button comes back to its original state
            GripSet = 0;
        }
        // BUTTON TOUCHED, Change of group
        if (MyPros.buttonSwitch(ButtonStateHand, PreviousButtonState, ButtonFlag, FlagIdle) != ButtonFlag)
        {
            ButtonFlag = !ButtonFlag;
            GripSet = 0;
        }
        // GRIP CHANGE INSIDE A SAME GROUP: double opening signal  
        if ((emg1 > emg1Threshold) && (emg0 <= emg0Threshold) && FlagIdle && PreviousEmg1 <= emg1Threshold)
        {
            if (firstSignalEmitted)
            {
                if (((Time.time - timeOfFirstSignal) > 0.07) && ((Time.time - timeOfFirstSignal) < 1.3)) // time window accepting second emitted signal
                {
                    FindObjectOfType<AudioManager>().Play("DoubleClick");
                    if (GripSet == 0)
                        GripSet = 1;
                    else
                        GripSet = 0;   
                }
                firstSignalEmitted = false;
            }
            else
            {
                firstSignalEmitted = true;
                FindObjectOfType<AudioManager>().Play("DoubleClick");
                timeOfFirstSignal = Time.time;
            }
        }

        /*************************** Prosthesis movements ***************************/
        RotationOfTheThumb(FlagRotationThumb, OppositionFlag);
        ActivationMovement(emg0, emg1, Grip, FingersTouched);
        ManageGraspedObject(FingersTouched && !FlagIdle, TouchPadArray, Grip);

        CocontractionCounter(emg0, emg1); // Update cocontraction counter

        // UPDATE variables
        PreviousEmg1 = emg1;
        PreviousButtonState = ButtonStateHand;
        PreviousButtonStateThumb = ButtonStateThumb;
    }

    /*=================== Fonctions ===============*/
    void RotationOfTheThumb(bool Activate, bool OppositionFlag)
    /** Drives the Thumb1 to its idle angle when there is a transition Opposition/Lateral (or vis versa). 
     * Note that this action was not implemented through an animation because we wanted to have the control of the 
     * Thumb1 inside the script. However it is not possible to control a body both via an animation and the script,
     * the animation that we use only has the control on the pivot of the thumb. 
     */
    {
        if (Activate)
        {
            float AngleThumb = Thumb_1.transform.localRotation.eulerAngles.z;
            if (OppositionFlag & (Math.Abs(290.0f - AngleThumb) >= 5.0f))
            {
                Thumb_1.transform.Rotate(-Vector3.forward * MaxStrength * rotationSpeed * Time.deltaTime, Space.Self);
                MyPros.SecurityAngle(AngleList[9], ObjectList, Thumb1Index, -1);
            }
            else if (!OppositionFlag & (Math.Abs(330.0f - AngleThumb) >= 5.0f))
            {
                Thumb_1.transform.Rotate(Vector3.forward * MaxStrength * rotationSpeed * Time.deltaTime, Space.Self);
                MyPros.SecurityAngle(AngleList[8], ObjectList, Thumb1Index, 1);
            }
            else
            {
                //Bool sets to false once the idle angle is reached
                FlagRotationThumb = false;
            }
        }
    }

    void ActivationMovement(float msg0, float msg1, int myGrip, bool FingersTouched)
    /** Activate the opening/closing movement according to the emg. */
    {
        float strength0 = MyPros.Remap(msg0, emg0Threshold, emg0Max, MinStrength, MaxStrength);
        float strength1 = MyPros.Remap(msg1, emg0Threshold, emg1Max, MinStrength, MaxStrength);
        if (FingersTouched == false)
        {
            if ((msg0 > emg0Threshold) && (msg1 <= emg1Threshold))
            {
                Movement(AngleList[myGrip], 1, CloseSpeed * strength0);
                FlagIdle = false;
            }
        }

        //COME BACK TO IDLE POSITION
        if ((msg1 > emg1Threshold) && (msg0 <= emg0Threshold) && !FlagIdle)
        {
            int index =0;
            if (!OppositionFlag) { index = 8; }
            else { index = 9; }
            Movement(AngleList[index], -1, OpenSpeed * strength1);
            FlagIdle = MyPros.CheckAchievedPosition(AngleList[index], ObjectList); //FlagIdle turns to true when all the fingers are at their idle state
        }
    }

    void Movement(List<float> AngleList, int sign, float Intensity)
    /**Function that makes the rotation of each finger. 
     * AngleList: contains the limit angle, when the rotation has to stop, for each finger
     * Sign: 1 or -1 says the direction of the rotation
     * Intensity: from 0 to 5, gives the intensity of the message received by the electrode 
     */
    {
        float InstantAngle;
        for (int i = 0; i < ObjectList.Count; i++)
        {
            InstantAngle = ObjectList[i].transform.localRotation.eulerAngles.z;
            if (Mathf.Abs(InstantAngle - AngleList[i]) >= 2)
            {
                //FindObjectOfType<AudioManager>().Play("WorkingEngine");
                ObjectList[i].transform.Rotate(sign * Vector3.forward * Intensity * Time.deltaTime, Space.Self);
            }
        }
        MyPros.SecurityAngle(AngleList, ObjectList, Thumb1Index, sign);
    }

    void ManageGraspedObject(bool myFingersTouched, Detector[] myTouchPadArray, int myGrip)
    /** If an object is stucked between the thumb and finger 1 or finger 2 during the tridigital grip pattern, this object should be grasped. */
    {
        if ((myGrip == 4 || myGrip == 7) && myFingersTouched)
        {
            if (myTouchPadArray[0].culprit_object == myTouchPadArray[1].culprit_object || myTouchPadArray[0].culprit_object == myTouchPadArray[2].culprit_object && myTouchPadArray[0].culprit_object.tag == "test_object")
            {
                if (myTouchPadArray[0].culprit_object.tag == "test_object")
                {
                    canHold = true;
                    TouchedObj = myTouchPadArray[0].culprit_object;
                    MyPros.AttachObject(TouchedObj, Prosthesis);
                    isHolding = true;
                }
            }
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

    int GiveGroup(bool MyFlagOpposition, bool MyFlagButton, bool DisplayGroup = true)
    /** Returns the group number and adapt the group display. */
    {
        int group;
        if (!MyFlagOpposition & !MyFlagButton)
        {
            group = 1;
        }
        else if (!MyFlagOpposition & MyFlagButton)
        {
            group = 2;
        }
        else if (MyFlagOpposition & !MyFlagButton)
        {
            group = 3;
        }
        else
        {
            group = 4;
        }
        if(DisplayGroup){
            DisplayGroupNumber(group);
        }
        return group;
    }

    int GiveGrip(int myGroup, int myGripNumber)
    /** Returns the grip number */
    {
        return (myGroup - 1) * 2 + myGripNumber;
        // Explanation of the AngleList index: Group =[1, 2, 3, 4], Grip =[0, 1, 2, 3, 4, 5, 6, 7, 8] and GripSet =[0, 1]
        // To have access on Grip 3, which is in group 2, (2-1)*2 + GripSet = 2 + 0 = 2
    }

    void DisplayGroupNumber(int CurrentGroup)
    /** Updates group display */
    {
        GroupFile.transform.GetChild(CurrentGroup - 1).gameObject.SetActive(true);
        for (int i = 0; i < GroupFileSize; i++)
        {
            if (i != (CurrentGroup - 1))
                GroupFile.transform.GetChild(i).gameObject.SetActive(false);
        }
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
