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

public class MainMenuButtonManager : MonoBehaviour
{
    // Following functions allow to manage each button of the MainMenu scene.
    public void ProsthesisScene()
    {
        SceneManager.LoadScene("ProsthesisSelection");
    }

    public void ThresholdScene()
    {
        SceneManager.LoadScene("ThresholdAdjustment");
    }

    public void UncouplingScene()
    {
        SceneManager.LoadScene("Uncoupling");
    }

    public void PlayScene()
    {
        SceneManager.LoadScene("TestRoom");
    }

    public void JeQuitteLeJeu()
    {
        Application.Quit();
    }
}
