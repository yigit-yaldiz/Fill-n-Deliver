using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Pedestrian"))
        {
            other.GetComponent<Pedestrian>().ResetTheWalker();    
        }
    }
}
