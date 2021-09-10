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

public class RetourMenu : MonoBehaviour
{    
    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
