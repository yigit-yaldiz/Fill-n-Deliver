using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Transform _explosionTransform;
    private Animator _animator;
    private Rigidbody[] _ragdollRigidBodies;
    private List<Transform> _firstRagdollPositions = new List<Transform>();

    private void Awake()
    {
        _ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        _animator = GetComponent<Animator>();
        ChangeRagdollStatus(ragdollStatus: false);
    }

    private void Start()
    {
        foreach (Rigidbody item in _ragdollRigidBodies)
        {
            _firstRagdollPositions.Add(item.transform);
        }
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
    }

    public void Explosion()
    {
        PlayerController player = transform.parent.GetComponentInParent<PlayerController>();

        float randomForce;

        if (player.InteractedWithPedestrian)
        {
            randomForce = Random.Range(500f, 1000f);
        }
        else
        {
            randomForce = Random.Range(1500f, 2000f);
        }

        Physics.IgnoreLayerCollision(13, 3);

        foreach (Rigidbody rb in _ragdollRigidBodies)
        {
            rb.AddExplosionForce(randomForce, _explosionTransform.position, 1000);
        }
    }

    public void ResetArmature()
    {
        transform.GetChild(0).localPosition = Vector3.zero;
        transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    }

    public void ResetRagdollColliderPosition()
    {
        for (int i = 0; i < _ragdollRigidBodies.Length; i++)
        {
            _ragdollRigidBodies[i].transform.localPosition = _firstRagdollPositions[i].localPosition;
            _ragdollRigidBodies[i].transform.localRotation = _firstRagdollPositions[i].localRotation;
            _ragdollRigidBodies[i].transform.localScale = _firstRagdollPositions[i].localScale;

            //Debug.Log(_ragdollRigidBodies[i].transform + " attached to " + _firstRagdollPositions[i]);          
        }
    }
}
