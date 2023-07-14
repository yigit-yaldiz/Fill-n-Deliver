using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class PoliceTrigger : MonoBehaviour
{
    PathFollower _pathFollower;
    [SerializeField] bool _criminalReportDetected = false;
    const float _speed = 0.5f;

    [SerializeField] Animator _characterAnimator;
    [SerializeField] Animator _motorcycleAnimator;

    private void Awake()
    {
        _pathFollower = GetComponentInParent<PathFollower>();
        _pathFollower.enabled = false;
        _characterAnimator.SetTrigger("Sitting");
    }

    private void Update()
    {
        if (_criminalReportDetected)
        {
            _pathFollower.enabled = true;
            _characterAnimator.SetTrigger("Triggered");
            _motorcycleAnimator.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            _pathFollower.speed = 0;
            _motorcycleAnimator.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            _pathFollower.speed = _speed;
        }
        else if (other.CompareTag("Player") && _criminalReportDetected)
        {
            Debug.Log("Busted");
            _pathFollower.speed = 0;
            _motorcycleAnimator.enabled = false;
        }
    }
}
