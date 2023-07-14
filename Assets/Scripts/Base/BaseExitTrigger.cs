using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseExitTrigger : MonoBehaviour
{
    private Animator[] _animators;

    private void Awake()
    {
        _animators = transform.parent.GetComponentsInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            foreach (var animator in _animators)
            {
                animator.SetTrigger("Close");
            }
        }
    }
}
