/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel
 * UCLouvain, EPL
 * academic year 2019-2020
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace MyFunctions
{
    public class ProsthesisFunction
    {
        public static void AttachObject(GameObject Object, GameObject Prosthesis)
        /** Freeze the GO and make it a child of the prosthesis GO */
        {
            Object.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Object.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            Object.GetComponent<Rigidbody>().useGravity = false;
            Object.GetComponent<Rigidbody>().isKinematic = true;
            Object.transform.SetParent(Prosthesis.transform, true);
        }

        public static void ReleaseObject(GameObject Object)
        /** Release the child GO. */
        {
            Vector3 objectPos = Object.transform.position;
            Object.transform.SetParent(null);
            Object.GetComponent<Rigidbody>().useGravity = true;
            Object.GetComponent<Rigidbody>().isKinematic = false;
            Object.transform.position = objectPos;
        }

        public static float Remap(float value, float min1, float max1, float min2, float max2)
        /** Rescale the current value from [min1, max1] to [min2, max2] */
        {
            return min2 + (value - min1) / (max1 - min1) * (max2 - min2);
        }

        public static int CheckButton(Detector Button, bool changeColor = true)
        /**Checks the state of the button, pushed (1) or not (0)*/
        {
            int state = 0;
            //Check that the button is pushed by the left hand, which is the GameController
            if (Button.entered)
            {
                //Be careful: duplicate the "if". Otherwise, risc of unexisting object.
                if (Button.culprit_object.tag == "GameController")
                {
                    state = 1;
                }
            }
            if (changeColor)
            {
                ButtonChangeColor(Button.transform.GetComponent<Renderer>().material, state);
            }
            return state;
        }

        public static void ButtonChangeColor(Material buttonMaterial, int State)
        {
            if (State == 1)
            {
                buttonMaterial.SetColor("_Color", Color.green);
            }
            else
            {
                buttonMaterial.SetColor("_Color", Color.black);
            }
        }

        public static bool buttonSwitch(int State, int previousState, bool previousBool, bool FlagIdle = true)
        /** Switch boolean value when State rise from 0 to 1 */
        {
            if(State == 1 && previousState == 0 && FlagIdle)
            {
                return (!previousBool);
            }
            else
            {
                return previousBool;
            }
        }

        public static bool CheckAchievedPosition(List<float> LimitAngle, List<GameObject> ObjectList)
        /** Returns true when all the fingers are at their limit position
        *   Otherwise returns false
        */
        {
            bool PositionAchieved = true;
            float InstantAngle;
            for (int i = 0; i < ObjectList.Count; i++)
            {
                InstantAngle = ObjectList[i].transform.localRotation.eulerAngles.z;
                if (Mathf.Abs(InstantAngle - LimitAngle[i]) >= 2)
                    PositionAchieved = false;
            }
            if (PositionAchieved)
                return true;
            else
                return false;
        }

        public static void DisplayFelicitation(int myAimedGrip, int myGrip, List<List<float>> AngleList, List<GameObject> ObjectList, bool CanDisplay, GameObject Display)
        /** Displays the GO "Display" if the right movement is achieved.
        *   Remains visible until Idle is achived.    
        */
        {
            if (myAimedGrip == myGrip)
            {
                if (CheckAchievedPosition(AngleList[myAimedGrip], ObjectList))
                {
                    Display.SetActive(true);
                }
            }

            if (!CanDisplay)
            {
                Display.SetActive(false);
            }
        }

        public static void SecurityAngle(List<float> AngleList, List<GameObject> ObjectList, int Thumb1Index,int sign)
        /** Force the angle of the fingers if it exceeds the realistic values */
        {
            Vector3 currentAngle;
            for (int i = 0; i < ObjectList.Count; i++)
            {
                currentAngle = ObjectList[i].transform.localRotation.eulerAngles;
                if (sign == 1) // The hand is closing
                {
                    //First condition: if the current angle goes above the limit angle
                    //Or 
                    //Second condition: if Thumb1 passed from 360 to 0
                    if (currentAngle.z > AngleList[i] || (i == Thumb1Index && currentAngle.z < 10.0f))
                    {
                        currentAngle.z = AngleList[i] - 1;
                    }
                }
                else // The hand is opening
                {
                    //First condition: if the finger passed from 0 to 360
                    //Or 
                    //Second condition: if Thumb1 goes below the limite angle
                    if ((i != Thumb1Index && currentAngle.z > 350.0f) || (i == Thumb1Index && currentAngle.z < AngleList[i]))
                    {
                        currentAngle.z = AngleList[i] + 1;
                    }
                }
                ObjectList[i].transform.localRotation = Quaternion.Euler(currentAngle);
            }
        }

    }

    public class SharedFunction
    {
        private static int emgMaxVoltage = 5;
        public static void EMGAdjustDisplay(float currentEMG, float currentThreshold, GameObject currentEMGDisplay)
        /** Adjust the EMG gauges of the user. Pay attention: take care of the hierarchy of the Game Objects!*/
        {
            Button LightButton;
            GameObject CurrentThreshold;
            Text CurrentThresholdText;
            Slider CurrentThresholdSlider;
            LightButton = currentEMGDisplay.transform.GetChild(0).gameObject.GetComponent<Button>();
            CurrentThreshold = currentEMGDisplay.transform.GetChild(1).gameObject;
            CurrentThresholdSlider = CurrentThreshold.GetComponentInChildren<Slider>();
            CurrentThresholdText = CurrentThreshold.transform.GetChild(0).transform.GetChild(0).GetComponentInChildren<Text>();

            CurrentThresholdSlider.value = currentThreshold;
            CurrentThresholdText.text = currentThreshold.ToString("f2");
            currentEMGDisplay.GetComponent<Slider>().value = currentEMG;

            if (currentEMG >= currentThreshold)
            {
                LightButton.GetComponent<Image>().color = Color.green;
            }
            else
            {
                LightButton.GetComponent<Image>().color = Color.white;
            }
        }

        public static float[] GetMessage(SerialPort mySP)
        /** Returns the data extracted from the message received via serial port mySP. 
         *  [AbleToRead, emg0, emg1, button0, button1]
         *  Remark: buttons are not used in the cleaned setup.
         */
        {
            float msg0 = 0, msg1 = 0, msg2 = 0, msg3 = 0;
            int AbleToRead = 1;
            if (mySP.IsOpen) // if port available, check message
            {
                long msg = 0;
                try { msg = Int32.Parse(mySP.ReadLine()); }   // get string message and cast 
                catch (System.Exception)
                {
                    AbleToRead = 0; 
                }

                /* Extract info from the message 
                 * msg type: |bt--|----|----|----|emg1|emg1|emg0|emg0|
                 */
                if (AbleToRead == 1)
                {
                    msg0 = (float)((msg >> 0) & 0b11111111) / 255 * emgMaxVoltage;     // emg0 strength, [0, 5], 0 at rest.
                    msg1 = (float)((msg >> 8) & 0b11111111) / 255 * emgMaxVoltage;     // emg1 strength, [0, 5], 0 at rest.
                    msg2 = (msg >> 30) & 0b1;         // button 0 state, 1 if pushed else 0.
                    msg3 = (msg >> 31) & 0b1;         // button 1 state, 1 if pushed else 0
                }
            }
            return new float[] {AbleToRead, msg0, msg1, msg2, msg3 };
        }
    }
}


