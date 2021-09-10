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

public class KeepScore : MonoBehaviour
{
	public GameObject textDisplay;
	public GameObject _uncouplingList;
	public static int countCollisions = 0;

	void Start()
	{
			textDisplay.GetComponent<Text>().text = "Score : " + countCollisions.ToString();
	}

	void Update()
	{
		if (_uncouplingList.transform.GetChild(1).gameObject.activeSelf||_uncouplingList.transform.GetChild(2).gameObject.activeSelf)
		{
			textDisplay.GetComponent<Text>().text = "Score : " + countCollisions.ToString();
			if (NewTimer.timeOut)
			{
				textDisplay.GetComponent<Text>().color = Color.yellow;
			}
		}
	}

	public void Reset()
	{
        countCollisions = 0;
		textDisplay.GetComponent<Text>().text = "Score :" + countCollisions.ToString();
		textDisplay.GetComponent<Text>().color = Color.white;
	}
}

