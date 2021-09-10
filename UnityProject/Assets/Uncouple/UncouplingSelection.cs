/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Ange Muhoza & Lucas El Raghibi
 * UCLouvain, EPL
 * academic year 2020-2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UncouplingSelection : MonoBehaviour
{

    private GameObject[] UncouplingList;
    private int index = 0;
    private void Start()
    {

        index = PlayerPrefs.GetInt("UncouplingSelected");
        if(index > transform.childCount - 1)
        {
            PlayerPrefs.SetInt("UncouplingSelected", 0);
            index = 0;
        }

        UncouplingList = new GameObject[transform.childCount];

        // fill the array
        for(int i = 0; i< transform.childCount; i++)
        {
            UncouplingList[i] = transform.GetChild(i).gameObject;
        }


        //toggle off their render
        foreach(GameObject go in UncouplingList)
        {
            go.SetActive(false);
        }

        // toggle on the selected prosthesis
        if (UncouplingList[index])
        {
            UncouplingList[index].SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            ToggleRight();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            ToggleLeft();
        }
    }

    public void ToggleLeft()
    {
        // Toggle off the current model
        if (UncouplingList[0].activeSelf)
        {
            UncouplingList[0].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<CubeRotation>().enabled = false;
            CubeRotation.sp.Close();
        }

        if (UncouplingList[1].activeSelf)
        {
            UncouplingList[1].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<SphereMvt>().enabled = false;
            SphereMvt.sp.Close();
        }

        if (UncouplingList[2].activeSelf)
        {
            UncouplingList[2].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<PinceRotation>().enabled = false;
            PinceRotation.sp.Close();
        }
        UncouplingList[index].SetActive(false);

        index--;
        if(index < 0)
        {
            index = UncouplingList.Length - 1;
        }

        // toggel on
        UncouplingList[index].SetActive(true);
        if (UncouplingList[0].activeSelf)
        {
            UncouplingList[0].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<CubeRotation>().enabled = true;
            //CubeRotation.sp.Open();
        }

        if (UncouplingList[1].activeSelf)
        {
            UncouplingList[1].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<SphereMvt>().enabled = true;
            //SphereMvt.sp.Open();
        }
        
        if (UncouplingList[2].activeSelf)
        {
            UncouplingList[2].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<PinceRotation>().enabled = true;
            //PinceRotation.sp.Open();
        }
    }

    public void ToggleRight()
    {
        // Toggle off the current model
        if (UncouplingList[0].activeSelf)
        {
            UncouplingList[0].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<CubeRotation>().enabled = false;
            CubeRotation.sp.Close();
        }

        if (UncouplingList[1].activeSelf)
        {
            UncouplingList[1].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<SphereMvt>().enabled = false;
            SphereMvt.sp.Close();
        }

        if (UncouplingList[2].activeSelf)
        {
            UncouplingList[2].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<PinceRotation>().enabled = false;
            PinceRotation.sp.Close();
        }
        UncouplingList[index].SetActive(false);

        index++;
        if (index == UncouplingList.Length)
        {
            index = 0;
        }

        // toggel on
        UncouplingList[index].SetActive(true);
        if (UncouplingList[0].activeSelf)
        {
            UncouplingList[0].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<CubeRotation>().enabled = true;
            //CubeRotation.sp.Open();
        }

        if (UncouplingList[1].activeSelf)
        {
            UncouplingList[1].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<SphereMvt>().enabled = true;
            //SphereMvt.sp.Open();
        }

        if (UncouplingList[2].activeSelf)
        {
            UncouplingList[2].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<PinceRotation>().enabled = true;
            //PinceRotation.sp.Open();
        }
    }
}