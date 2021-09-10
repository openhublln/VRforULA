using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackCube : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name == "default" || collision.transform.name == "PinceLeft" || collision.transform.name == "PinceRight" || collision.transform.name == "Pince")
        {
            KeepScore.countCollisions--;
        }
        Destroy(gameObject);
    }
}