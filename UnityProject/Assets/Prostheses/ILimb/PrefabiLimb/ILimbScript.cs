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

public class ILimbScript : MonoBehaviour
{
    public GameObject EMG0User, EMG1User;                   // Game Object (GO) for displaying emg gauges in the dashboard
    public GameObject ExplanationSlides, TaskSlides;        // GO containing all the slides 
    public GameObject ForeArm, Prosthesis, Finger1_1,       // Prosthesis GO references
                      Finger1_2, Finger2_1, Finger2_2,
                      Finger3_1, Finger3_2, Finger4_1,
                      Finger4_2, Thumb_1, Thumb_2;
    public Button ExplanationButton, TaskButton;            // Button references in the dashboard
    public Detector Touch1, Touch2, Touch3, Touch4, TouchThumb;// Detector of each finger
    public float CloseSpeed = 25;                           // Hand closing speed
    public float OpenSpeed = 50;                            // Hand opening speed
    public float rotationSpeed = 50;                        // Default rotation speed, used for the rotation of the thumb
    public float MinStrength = 0, MaxStrength = 10;         // Scale the EMG inensity

    private bool FlagOpposition;
    private readonly SerialPort sp = new SerialPort("COM4", 9600);// Serial port, connected to the arduino controller
    private float emg0, emg1;                               // EMG voltage
    private GameObject TouchedObj;                          // Reference of the touched GO
    private Animator _ThumbAnimation;                       // Animation for the thumb 
    private Detector[] TouchPadArray;                       // Container for all the detectors
    private List<GameObject> ObjectList;                    // Container for all the body of the prosthesis
    List<List<float>> AngleList = new List<List<float>>();  // List of the list of the limit angles
    private int Thumb1Index;                                // Thumb index reference (see start method)

    //Variables to handle the grip switch 
    public GameObject GripChip;                             // GO of the grip chip, allowing to change grip pattern
    private bool OperationChangeGrip = false;               // True if change grip is accessed
    private bool Step1, Step2, Step3;                       // Step flags for changing grip. Manage index jump.
    private bool FirstPress = false;                        // True if first press recorded
    private float startTime = 0f;                           // Start time for emg1 activation
    private float PressTime;                                // Cumulated emg1 activation time
    private bool FlagIdle = true;                           // True if Idle position
    private float startTimeGripSwitch;                      // Start time for switching grip
    private readonly float holdTime = 1.0f;                 // emg1 duration required for accessing grip change
    private readonly float holdTimeGripSwitch = 1.5f;       // Time window for switching grip
    private float PositionX, PositionY;                     // Current X and Y poistion (world reference)
    private float PreviousEmg1;                             // Previous emg1 value
    private int Grip = 0;                                   // Current grip pattern

    //Grasping object
    private bool isHolding = false;                         // True if the hand is currenctly holding an object
    private bool canHold = true;                            // True if the hand is ready to catch an object

    //Slide
    public GameObject Felicitation;                         // "felicitation" display
    public GameObject PositionFile;                         // Slides displaying the current selected position
    [HideInInspector] public int PositionFileSize;           // Number of "position" slides

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
        DisplayPositionNumber(0);           // Initiate group display
        Felicitation.SetActive(false);      // Ensure "Felicitation" deactivation
        GripChip.SetActive(true);

        // Get and group GO references
        TouchPadArray = new Detector[5] { TouchThumb, Touch1, Touch2, Touch3, Touch4 };
        ObjectList = new List<GameObject> { Finger1_1, Finger1_2, Finger2_1, Finger2_2, Finger3_1, Finger3_2, Finger4_1, Finger4_2, Thumb_1, Thumb_2 };
        Thumb1Index = ObjectList.Count - 2;
        PreviousEmg1 = 0;

        // Other parameters
        Step1 = false;
        Step2 = false;
        Step3 = false;
        FlagOpposition = false;
        _ThumbAnimation = GetComponent<Animator>();
        PositionFileSize = PositionFile.transform.childCount;

        //Grip 0: Prise latérale (LATERAL THUMB)
        AngleList.Add(new List<float> { 70.0f, 80.0f, 70.0f, 80.0f, 70.0f, 80.0f, 70.0f, 80.0f, 345.0f, 80.0f });
        //Grip 1: Index pointe (LATERAL THUMB)
        AngleList.Add(new List<float> { 5.0f, 5.0f, 80.0f, 80.0f, 80.0f, 80.0f, 80.0f, 80.0f, 340.0f, 80.0f });
        //Grip 2: Pince tridigitale (OPPOSITION THUMB)
        AngleList.Add(new List<float> { 50.0f, 80.0f, 50.0f, 80.0f, 105.0f, 90.0f, 105.0f, 90.0f, 300.0f, 70.0f });
        //Grip 3: Prise de force (OPPOSITION THUMB)
        AngleList.Add(new List<float> { 20.0f, 90.0f, 20.0f, 90.0f, 20.0f, 90.0f, 20.0f, 90.0f, 330.0f, 30.0f });

        //CHIPS
        //Grip 4: Prise souris (LATERAL THUMB)
        AngleList.Add(new List<float> { 20.0f, 20.0f, 10.0f, 20.0f, 10.0f, 20.0f, 30.0f, 20.0f, 340.0f, 30.0f });

        //COME BACK TO IDLE POSITION
        //Index 5: Come back for movements
        AngleList.Add(new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 300.0f, 0.0f });
    }

    void Update()
    {
        float[] data = MyFunc.GetMessage(sp);                           // Data: [ableToRead, emg0, emg1, button0, button1]
        if (data[0] != 0) { emg0 = data[1]; emg1 = data[2]; }           // If able to read serial port, update emg values
        MyFunc.EMGAdjustDisplay(emg0, emg0Threshold, EMG0User);         // Update the emg gauges on the dashboard
        MyFunc.EMGAdjustDisplay(emg1, emg1Threshold, EMG1User);

        bool FingersTouched = checkDoubleTouch(TouchPadArray);          // True if the hand has at least one finger and the thumb touching something
        int AimedGrip = ChangeSlide.slideIndex - 1;                     // Aimed grip, according to the task slide display
        MyPros.DisplayFelicitation(AimedGrip, Grip, AngleList, ObjectList, !FlagIdle, Felicitation);// Displays "felicitation"
        DisplayPositionNumber(Grip);

        /*************************** Check commands ***************************/
        // LONG SIGNAL, change grip according to the direction
        /*Initiate the "Jump" of the index if the signals is held during 2 seconds*/
        if ((emg1 > emg1Threshold) && (emg0 <= emg0Threshold) && FlagIdle && !FirstPress && PreviousEmg1 < emg1Threshold)
        {
            startTime = Time.time;
            PressTime = startTime + holdTime;
            FirstPress = true;
        }
        if (FirstPress)
        {
            if ((emg1 > emg1Threshold) && (emg0 <= emg0Threshold) && Time.time >= PressTime)
            {
                FindObjectOfType<AudioManager>().Play("DoubleClick"); // Auditive feedback
                FirstPress = false;
                Step1 = true;
                OperationChangeGrip = true;

            }
            else if ((emg1 < emg1Threshold) && (emg0 > emg0Threshold))
            {
                FirstPress = false;

            }
        }
        CocontractionCounter(emg0, emg1); // Update cocontraction counter
        JumpIndex(Step1);   //STEP 1 : "Jump" of the index 
        StartRecord(Step2); //STEP 2 : Start to record position once the index has reached the "jump" position
        //Wait a few seconds so that the subject performs his/her movement
        if (OperationChangeGrip && ((Time.time - startTimeGripSwitch) >= holdTimeGripSwitch))
        {
            Grip = GiveGrip(PositionX, PositionY, ForeArm.transform.localPosition.x, ForeArm.transform.localPosition.y, Grip); //Determine the main direction of the movement 
            Step3 = true;    //STEP 3: The new grip is selected, now the movements can be performed
        }
        // GRIP CHIP, change grip
        if (FlagIdle && GripChip.GetComponent<Detector>().entered) // If passed above the grip chip
        {
            if (GripChip.GetComponent<Detector>().culprit_object.tag == "Player") // Ensure iLimb detection
            {
                Grip = 4;   // Mouse Grip
            }
        }

        /*************************** Prosthesis movements ***************************/
        ActivationMovement(emg0, emg1, Grip, Step3, FingersTouched);
        ManageGraspedObject(FingersTouched && !FlagIdle, TouchPadArray, Grip);

        // UPDATE variables
        PreviousEmg1 = emg1;
    }

    void ActivationMovement(float msg0, float msg1, int myGrip, bool IdleAfterJump, bool FingersTouched)
    /** Activate the opening/closing movement according to the emg. */
    {
        float strength0 = MyPros.Remap(msg0, emg0Threshold, emg0Max, MinStrength, MaxStrength);
        float strength1 = MyPros.Remap(msg1, emg1Threshold, emg1Max, MinStrength, MaxStrength);
        if ((msg0 > emg0Threshold) && (msg1 <= emg1Threshold) && !OperationChangeGrip && !FingersTouched)
        {
            if (FlagOpposition && (myGrip == 0 || myGrip == 1 || myGrip == 4))
            {
                _ThumbAnimation.SetBool("ActivationOpposition", false);
                FlagOpposition = false;
            }
            else if (!FlagOpposition && (myGrip == 2 || myGrip == 3))
            {
                _ThumbAnimation.SetBool("ActivationOpposition", true);
                FlagOpposition = true;
            }
            Movement(AngleList[myGrip], 1, strength0 * CloseSpeed);
            FlagIdle = false;
        }

        //After the "jump" of the first finger, we come back to the idle position
        else if ((msg0 > emg0Threshold) && (msg1 <= emg1Threshold))
        {
            IdleAfterJumpIndex(IdleAfterJump);
        }

        //COME BACK TO IDLE POSITION
        if ((msg1 > emg1Threshold) && (msg0 < emg0Threshold) && !FlagIdle && !OperationChangeGrip)
        {
            Movement(AngleList[5], -1, strength1 * OpenSpeed);
            //FlagIdle turns to true when all the fingers are at their idle state
            FlagIdle = MyPros.CheckAchievedPosition(AngleList[5], ObjectList);
            //print(FlagIdle);
        }
    }

    void Movement(List<float> AngleList, int sign, float Intensity)
    /**Function which makes the rotation of each finger. 
     * AngleList: contains the limit angle, when the rotation has to stop, for each finger
     * Sign: 1 or -1 says the direction of the rotation
     * intensity: from 0 to 15, gives the intensity of the message received by the electrode 
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

    void StartRecord(bool Allowed)
    /** Check current time, X and Y position (will be used for grip change)*/
    {
        if (Allowed)
        {
            startTimeGripSwitch = Time.time;
            PositionX = ForeArm.transform.localPosition.x;
            PositionY = ForeArm.transform.localPosition.y;
            Step2 = false;
        }
    }

    void JumpIndex(bool Allowed)
    /** When changing grip is allowed, the index "jumps" slightly above the othe fingers */
    {
        if (Allowed)
        {
            float AngleIndex = Finger1_1.transform.localRotation.eulerAngles.z;
            if (Math.Abs(AngleIndex - 340.0f) >= 5.0f)
            {
                Finger1_1.transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                Step1 = false;
                Step2 = true;
            }
            FlagIdle = false;

            //SECURITY CHECK
            Vector3 currentAngle = Finger1_1.transform.localRotation.eulerAngles;
            if (currentAngle.z < 340 && currentAngle.z > 200)
            {
                currentAngle.z = 340.0f;
                Finger1_1.transform.localRotation = Quaternion.Euler(currentAngle);
            }
        }
    }

    void IdleAfterJumpIndex(bool Allowed)
    /** After index "jump", fingers recover the Idle position */
    {
        if (Allowed)
        {
            float AngleIndex = Finger1_1.transform.localRotation.eulerAngles.z;
            if (Math.Abs(AngleIndex - 0.0f) >= 4.0f)
            {
                Finger1_1.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                OperationChangeGrip = false;
                Step3 = false;
                FlagIdle = true;
            }

            //SECURITY CHECK
            Vector3 currentAngle = Finger1_1.transform.localRotation.eulerAngles;
            if (5.0f < currentAngle.z && currentAngle.z < 90.0f)
            {
                currentAngle.z = 0.0f;
                Finger1_1.transform.localRotation = Quaternion.Euler(currentAngle);
            }
        }
    }

    int GiveGrip(float InitialX, float InitialY, float CurrentX, float CurrentY, int currentGrip)
    /** The main direction of the displacement corresponds to the grip. */
    {
        float DeltaX = CurrentX - InitialX;
        float DeltaY = CurrentY - InitialY;
        int newGrip;
        //if the moves are insignificant
        if (Math.Abs(DeltaX) <= 0.05 && Math.Abs(DeltaY) <= 0.05)
        {
            newGrip = currentGrip;
            //we don't change anything
        }
        else if (Math.Abs(DeltaX) >= Math.Abs(DeltaY))
        {
            if (Math.Sign(DeltaX) == 1)
            {
                newGrip = 0; //Droite 
            }
            else
                newGrip = 1; //Gauche 
        }
        else
        {
            if (Math.Sign(DeltaY) == 1)
            {
                newGrip = 2; //Haut
            }
            else
                newGrip = 3; //Bas
        }
        return newGrip;
    }

    bool checkDoubleTouch(Detector[] myTouchPadArray)
    /** Returns true if at least the thumb and finger 1 or finger 2 touch the same GO*/
    {
        if ((myTouchPadArray[1].entered && myTouchPadArray[0].entered) || (myTouchPadArray[2].entered && myTouchPadArray[0].entered))
        {
            if (myTouchPadArray[0].culprit_object.tag == "test_object")
            {
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    void ManageGraspedObject(bool myDouble_Touch, Detector[] myTouchPadArray, int myGrip)
    /** If an object is stucked between the thumb and finger 1 or finger 2 during the tridigital grip pattern, this object should be grasped. */
    {
        if ((myGrip == 2) && myDouble_Touch)
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

    void DisplayPositionNumber(int Selection)
    /** Updates current selection display */
    {
        for (int i = 0; i < PositionFileSize; i++)
        {
            if (i != Selection)
                PositionFile.transform.GetChild(i).gameObject.SetActive(false);
        }
        PositionFile.transform.GetChild(Selection).gameObject.SetActive(true);
    }
}