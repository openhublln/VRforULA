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

public class EmgGauges : MonoBehaviour
{
    public GameObject _uncouplingList;
    public GameObject _panelCube0;
    public GameObject _panelCube1;
    public GameObject _panelSphere0;
    public GameObject _panelSphere1;
    public GameObject _panelPince0;
    public GameObject _panelPince1;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void EmgOnOff()
	{
		if (_uncouplingList.transform.GetChild(0).gameObject.activeSelf)
		{
            if(_panelCube0.activeSelf)
            {
                _panelCube0.SetActive(false);
                _panelCube1.SetActive(false);
            }
            else 
            {
                _panelCube0.SetActive(true);
                _panelCube1.SetActive(true);
            }
		}
        
        if (_uncouplingList.transform.GetChild(1).gameObject.activeSelf)
		{
            if(_panelSphere0.activeSelf)
            {
                _panelSphere0.SetActive(false);
                _panelSphere1.SetActive(false);
            }
            else 
            {
                _panelSphere0.SetActive(true);
                _panelSphere1.SetActive(true);
            }
		}
        
        if (_uncouplingList.transform.GetChild(2).gameObject.activeSelf)
		{
            if(_panelPince0.activeSelf)
            {
                _panelPince0.SetActive(false);
                _panelPince1.SetActive(false);
            }
            else 
            {
                _panelPince0.SetActive(true);
                _panelPince1.SetActive(true);
            }
		}
	}
}
