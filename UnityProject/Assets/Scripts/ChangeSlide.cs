/**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel
 * UCLouvain, EPL
 * academic year 2019-2020
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSlide : MonoBehaviour
{
    public GameObject SlidePanel;       // Panel displaying the slides
    public Button Previous, Next;       // Button references
    public static int slideIndex;       // Currently displayed slide

    private int  lastIndex;             // Last slide index
   
    void Start()
    {
        slideIndex = 0;
        lastIndex = SlidePanel.transform.childCount - 1; // Count the number of contained slides
        Previous.onClick.AddListener(PreviousSlide);     // Attach function to the button
        Next.onClick.AddListener(NextSlide);             // Attach function to the button
        InitialSlide();                                 
    }

    public void NextSlide()
    /** Deactivate the current slide and activate the next one */
    { 
        SlidePanel.transform.GetChild(slideIndex).gameObject.SetActive(false);
        if (slideIndex == lastIndex)
            slideIndex = 0;
        else
            slideIndex = slideIndex + 1;

        SlidePanel.transform.GetChild(slideIndex).gameObject.SetActive(true);
    }
    public void PreviousSlide()
    /** Deactivate the current slide and activate the previous one */
    {
        SlidePanel.transform.GetChild(slideIndex).gameObject.SetActive(false);
        if (slideIndex == 0)
            slideIndex = lastIndex;
        else
            slideIndex = slideIndex - 1;

        SlidePanel.transform.GetChild(slideIndex).gameObject.SetActive(true);
    }
    void InitialSlide()
    /** Reset slide display: deactivate all the slides except the first one */
    {
        SlidePanel.transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 1; i <= lastIndex; i++)
        {
            SlidePanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
