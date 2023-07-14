using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListTrigger : MonoBehaviour
{
    [SerializeField] ListTexture _listTexture;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _listTexture.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _listTexture.enabled = false;
        }
    }
}
