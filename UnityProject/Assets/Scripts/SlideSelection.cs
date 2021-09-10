/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Ange Muhoza & Lucas El Raghibi
 * UCLouvain, EPL
 * academic year 2020-2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideSelection : MonoBehaviour
{
    public GameObject _ProsthesesList;
    public GameObject _GreiferExplanation;
    public GameObject _GreiferTask;
    public GameObject _BebionicExplanation;
    public GameObject _BebionicTask;
    public GameObject _ILimbExplanation;
    public GameObject _ILimbTask;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Explanation()
    {
        if (_ProsthesesList.transform.GetChild(0).gameObject.activeSelf)
        {
            _GreiferTask.SetActive(false);
            _GreiferExplanation.SetActive(true);
        }

        if (_ProsthesesList.transform.GetChild(1).gameObject.activeSelf)
        {
            _BebionicTask.SetActive(false);
            _BebionicExplanation.SetActive(true);
        }

        if (_ProsthesesList.transform.GetChild(2).gameObject.activeSelf)
        {
            _ILimbTask.SetActive(false);
            _ILimbExplanation.SetActive(true);
        }
    }

    public void Task()
    {
        if (_ProsthesesList.transform.GetChild(0).gameObject.activeSelf)
        {
            _GreiferExplanation.SetActive(false);
            _GreiferTask.SetActive(true);
        }

        if (_ProsthesesList.transform.GetChild(1).gameObject.activeSelf)
        {
            _BebionicExplanation.SetActive(false);
            _BebionicTask.SetActive(true);
        }

        if (_ProsthesesList.transform.GetChild(2).gameObject.activeSelf)
        {
            _ILimbExplanation.SetActive(false);
            _ILimbTask.SetActive(true);
        }
    }
}
