using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            other.GetComponent<Car>().ResetTheCar();
        }

        if (other.CompareTag("Train"))
        {
            other.GetComponent<Train>().ResetTheCar();
        }
    }
}
