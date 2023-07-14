using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCharacterFollower : MonoBehaviour
{
    [SerializeField] private Transform _followTransform;

    public static LookAtCharacterFollower Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        transform.position = _followTransform.position;
    }

    public void SetFollowTransfrom(Transform transform)
    {
        _followTransform = transform;
    }
}
