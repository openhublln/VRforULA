/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel
 * UCLouvain, EPL
 * academic year 2019-2020
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    /** The scrip must be attached to GO used as detectors, like a Button or a TouchPad.
     *  Returns entered = true if the GO has been penetrated by an other one.
     *  Returns the culprit_object, the reference to the penetrating GO.
     *  /!\ Do not try to use "culprit_object" before being sure that "entered" is true. Otherwise, a "null reference" error is returned.
     */
    public GameObject culprit_object = null;
    public bool entered = false;

    public void OnTriggerEnter(Collider _collider)
    {
        entered = true;
        culprit_object = _collider.gameObject;
    }

    public void OnTriggerExit(Collider _collider)
    {
        entered = false;
        culprit_object = null;
    }
}