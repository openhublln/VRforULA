/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel
 * UCLouvain, EPL
 * academic year 2019-2020
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGameObject : MonoBehaviour
{
    public GameObject GameObjToActivate;
    public GameObject GameObjToDisactivate;

    public void ActivateGameObj()
    /** Acitvate the GO and deactivate the other one*/
    {
        GameObjToDisactivate.SetActive(false);
        GameObjToActivate.SetActive(true);   
    }
}
