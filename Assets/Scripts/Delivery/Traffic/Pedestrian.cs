using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation.Examples;

public class Pedestrian : MonoBehaviour
{
    private PathFollower _pathFollower;
    private Pool _pedestrianPool;
    private Vector3 _firstPos;
    private Animator _animator;

    private Rigidbody[] _ragdollRigidBodies;
    private Rigidbody _collisionRb;
    private Obstacle _thisObstacle;
    private Transform _explosionTransform;

    private void Awake()
    {
        _ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        _pathFollower = GetComponent<PathFollower>();
        _pedestrianPool = GetComponentInParent<Pool>();
        _animator = GetComponent<Animator>();
        _collisionRb = GetComponent<Rigidbody>();
        _thisObstacle = GetComponent<Obstacle>();

        _pathFollower.pathCreator = _pedestrianPool.PathCreator;
        transform.position += new Vector3(0, 1, 0);
        _firstPos = transform.position;
    }

    private void OnEnable()
    {
        AccidentController.OnAccident += StopThePedestrian;
        AccidentController.OnAccident += DropThePedestrian;
    }

    private void OnDisable()
    {
        AccidentController.OnAccident -= StopThePedestrian;
        AccidentController.OnAccident -= DropThePedestrian;
    }

    private void Start()
    {
        _explosionTransform = PedestrianSpawner.Instance.ExplosionTransform;
        ChangeRagdollStatus(false);
    }

    public void ChangeRagdollStatus(bool ragdollStatus)
    {
        bool status;

        if (ragdollStatus == true) //ragdoll active
        {
            _animator.enabled = false;
            status = false;
        }
        else //ragdoll deactive
        {
            status = true;
            _animator.enabled = true;
        }

        foreach (Rigidbody item in _ragdollRigidBodies)
        {
            item.isKinematic = status;
        }

        _collisionRb.isKinematic = false;
    }

    public void ResetTheWalker()
    {
        _pathFollower.distanceTravelled = 0f;
        gameObject.SetActive(false);
        //transform.SetAsFirstSibling();
        transform.position = _firstPos;
    }

    private void StopThePedestrian()
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

    private void DropThePedestrian()
    {
        if (!_thisObstacle.DidItInteract)
        {
            return;
        }

        StartCoroutine(Disable(4f));

        IEnumerator Disable(float delayTime)
        {
            Explosion();
            ChangeRagdollStatus(true);
            yield return new WaitForSeconds(delayTime);
            ChangeRagdollStatus(false);
        }
    }

    public void Explosion()
    {
        float randomForce = Random.Range(400f, 500f);

        foreach (Rigidbody rb in _ragdollRigidBodies)
        {
            rb.AddExplosionForce(randomForce, _explosionTransform.position, 50);
        }
    }
}
