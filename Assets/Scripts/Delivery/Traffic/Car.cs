using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class Car : MonoBehaviour
{
    private PathFollower _pathFollower;
    private Pool _carPool;
    private Vector3 _firstPos;
    private Animator _animator;

    private void Awake()
    {
        _pathFollower = GetComponent<PathFollower>();
        _carPool = GetComponentInParent<Pool>();
        _animator = GetComponent<Animator>();
        _pathFollower.pathCreator = _carPool.PathCreator;
        _firstPos = transform.position;
    }

    private void OnEnable()
    {
        AccidentController.OnAccident += StopTheCar;
    }

    private void OnDisable()
    {
        AccidentController.OnAccident -= StopTheCar;
    }

    public void ResetTheCar()
    {
        _pathFollower.distanceTravelled = 0f;
        gameObject.SetActive(false);
        //transform.SetAsFirstSibling();
        transform.position = _firstPos;
    }

    public virtual void StopTheCar()
    {
        StartCoroutine(Disable(4f));

        IEnumerator Disable(float delayTime)
        {
            _pathFollower.enabled = false;
            _animator.enabled = false;
            
            yield return new WaitForSeconds(delayTime);

            _pathFollower.enabled = true;
            _animator.enabled = true;
        }
    }
}
