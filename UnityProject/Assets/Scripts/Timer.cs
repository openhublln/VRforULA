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

public class Timer : MonoBehaviour
{
	public GameObject textDisplay;
	public int secondstime = 0;
	public int minutestime = 0;
	public bool takingAway = false;
	private bool ready = false;
	public static bool timeOut = false;

	void Start()
	{
		textDisplay.GetComponent<Text>().text = minutestime + ":" + secondstime;
	}

	void Update()
	{
		if (!takingAway && ready)
		{
			StartCoroutine(TimerTake());
		}
	}

	IEnumerator TimerTake()
	{
		takingAway = true;
		yield return new WaitForSeconds(1);
		secondstime += 1;
		if (secondstime == 60)
		{
			minutestime += 1;
			secondstime = 0;
		}
		textDisplay.GetComponent<Text>().text = minutestime + ":" + secondstime;
		takingAway = false;
	}

	public void TimerGo()
	{
		if (!ready)
		{
			ready = true;
		}
	}

	public void Stop()
	{
		textDisplay.GetComponent<Text>().color = Color.yellow;
		timeOut = true;
		ready = false;
	}

	public void Reset()
	{
		minutestime = 0;
		secondstime = 0;
		textDisplay.GetComponent<Text>().text = minutestime + ":" + secondstime;
		textDisplay.GetComponent<Text>().color = Color.white;
		timeOut = false;
	}
}