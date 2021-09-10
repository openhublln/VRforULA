using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteCube : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name == "default" || collision.transform.name == "PinceLeft" || collision.transform.name == "PinceRight" || collision.transform.name == "Pince"|| collision.transform.name == "Sphere")
        {
            KeepScore.countCollisions++;
        }
        Destroy(gameObject);
    }
}
