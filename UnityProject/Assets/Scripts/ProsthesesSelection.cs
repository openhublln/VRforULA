/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel
 * UCLouvain, EPL
 * academic year 2019-2020
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProsthesesSelection : MonoBehaviour
{

    private GameObject[] prosthesesList;
    private int index = 0;
    public string SceneToLoad;
    private void Start()
    {

        index = PlayerPrefs.GetInt("ProsthesisSelected");
        if(index > transform.childCount - 1)
        {
            PlayerPrefs.SetInt("ProsthesisSelected", 0);
            index = 0;
        }

        prosthesesList = new GameObject[transform.childCount];

        // fill the array
        for(int i = 0; i< transform.childCount; i++)
        {
            prosthesesList[i] = transform.GetChild(i).gameObject;
        }


        //toggle off their render
        foreach(GameObject go in prosthesesList)
        {
            go.SetActive(false);
        }

        // toggle on the selected prosthesis
        if (prosthesesList[index])
        {
            prosthesesList[index].SetActive(true);
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
        prosthesesList[index].SetActive(false);

        index--;
        if(index < 0)
        {
            index = prosthesesList.Length - 1;
        }

        // toggel on
        prosthesesList[index].SetActive(true);
    }

    public void ToggleRight()
    {
        // Toggle off the current model
        prosthesesList[index].SetActive(false);

        index++;
        if (index == prosthesesList.Length)
        {
            index = 0;
        }

        // toggel on
        prosthesesList[index].SetActive(true);
    }

    public void ConfirmButton()
    {
        PlayerPrefs.SetInt("ProsthesisSelected",index);
        SceneManager.LoadScene(SceneToLoad);
    }
}
