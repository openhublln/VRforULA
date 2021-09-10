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

public class KeepCocontraction : MonoBehaviour
{
	public GameObject textDisplay;
	public GameObject _uncouplingList;
	public static int countCocontraction = 0;
	private bool ready = false;

	void Start()
	{
		textDisplay.GetComponent<Text>().text = "Cc : " + countCocontraction.ToString();
	}

	void Update()
	{
		if (ready)
        {
			textDisplay.GetComponent<Text>().text = "Cc : " + countCocontraction.ToString();
		}
		if (NewTimer.timeOut)
		{
			textDisplay.GetComponent<Text>().color = Color.yellow;
			ready = false;
		}
		if (Timer.timeOut)
		{
			textDisplay.GetComponent<Text>().color = Color.yellow;
			ready = false;
		}
	}

	public void CocontractionGo() 
	{
		if (!ready)
		{
			ready = true;
		}
	}

	public void Reset()
	{
        countCocontraction = 0;
		textDisplay.GetComponent<Text>().text = "Cc :" + countCocontraction.ToString();
		textDisplay.GetComponent<Text>().color = Color.white;
	}
}
