using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] Animation[] _redLights;
    [SerializeField] Light[] _greenLights;
    [SerializeField] Animator[] _animators;

    bool _didEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train") && !_didEnter)
        {
            _didEnter = true;
            StartCoroutine(GoOn());  
        }
    }

    private IEnumerator GoOn()
    {
        foreach (var animator in _animators)
        {
            animator.SetTrigger("Down");
        }

        foreach (var greenLight in _greenLights)
        {
            greenLight.enabled = false;
        }

        foreach (var redLight in _redLights)
        {
            redLight.enabled = true;
        }

        yield return new WaitForSeconds(3.25f);

        foreach (var animator in _animators)
        {
            animator.SetTrigger("Up");
        }

        foreach (var redLight in _redLights)
        {
            redLight.enabled = false;
        }

        foreach (var greenLight in _greenLights)
        {
            greenLight.enabled = true;
        }

        _didEnter = false;
    }
}
