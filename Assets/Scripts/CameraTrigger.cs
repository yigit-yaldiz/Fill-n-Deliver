using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] GameObject _triggerCamera;
    [SerializeField] GameObject _deliverCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _deliverCamera.SetActive(false);
            _triggerCamera.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _triggerCamera.SetActive(false);
            _deliverCamera.SetActive(true);
        }
    }
}
