/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Ange Muhoza & Lucas El Raghibi
 * UCLouvain, EPL
 * academic year 2020-2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float _delay = 0.1f;
    public GameObject _uncouplingList;
    private bool ready = false;
    public GameObject _blackCube;
    public GameObject _whiteCube;
    private GameObject[] CubeList;
    int[] Numbers = new int[] { -2, 1};

    void Start()
    {
        CubeList = new GameObject[2];
        CubeList[0] = _blackCube;
        CubeList[1] = _whiteCube;
    }

    void Update()
    { 
        if (_uncouplingList.transform.GetChild(1).gameObject.activeSelf||_uncouplingList.transform.GetChild(2).gameObject.activeSelf)
        {
            if (ready) { InvokeRepeating("Spawn", _delay, _delay); }
            if (NewTimer.timeOut)
            {
                CancelInvoke();
            }
        }
        ready = false;
    }

    void Spawn()
    {
        if (_uncouplingList.transform.GetChild(1).gameObject.activeSelf)
        {
            Instantiate(_whiteCube, new Vector3(Random.Range(-6, 6), 20, 10), Quaternion.identity);
        }
        if (_uncouplingList.transform.GetChild(2).gameObject.activeSelf)
        {
            Instantiate(CubeList[Random.Range(0, CubeList.Length)], new Vector3(Numbers[Random.Range(0, Numbers.Length)], 20, 10), Quaternion.identity);
        }
    }  

    public void Go()
    {
        if (!ready)
        {
            ready = true;
        }
    }

    public void Reset()
    {
        CancelInvoke();
        ready = false;
    }
}