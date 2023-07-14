using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class Train : MonoBehaviour
{
    private PathFollower _pathFollower;
    private Pool _carPool;
    private Vector3 _firstPos;
    private const float _trainSpeed = 25f;

    private void Awake()
    {
        _pathFollower = GetComponent<PathFollower>();
        _carPool = GetComponentInParent<Pool>();
        _pathFollower.pathCreator = _carPool.PathCreator;
        _firstPos = transform.position;
    }

    private void OnEnable()
    {
        AccidentController.OnAccident += StopTheTrain;
    }

    private void OnDisable()
    {
        AccidentController.OnAccident -= StopTheTrain;
    }

    void Update()
    {
        transform.eulerAngles += new Vector3(0, 90, 0);
    }

    void StopRotation()
    {
        StartCoroutine(DeactivateRotation(4f));
    }

    IEnumerator DeactivateRotation(float time)
    {
        enabled = false;
        yield return new WaitForSeconds(time);
        enabled = true;
    }

    public void ResetTheCar()
    {
        _pathFollower.distanceTravelled = 0f;
        gameObject.SetActive(false);
        //transform.SetAsFirstSibling();
        transform.position = _firstPos;
    }

    public virtual void StopTheTrain()
    {
        StartCoroutine(Disable(4f));

        IEnumerator Disable(float delayTime)
        {
            while (_pathFollower.speed >= 0)
            {
                _pathFollower.speed -= Time.deltaTime * 2.5f;
                yield return null;
            }
            
            yield return new WaitForSeconds(delayTime / 2);

            while (_pathFollower.speed <= _trainSpeed)
            {
                _pathFollower.speed += Time.deltaTime * 2.5f;
                yield return null;
            }

            _pathFollower.speed = _trainSpeed;
        }
    }
}
