/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel
 * UCLouvain, EPL
 * academic year 2019-2020
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presentoire : MonoBehaviour
{
    public GameObject BeautifullObject;
    public float rotationSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BeautifullObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
