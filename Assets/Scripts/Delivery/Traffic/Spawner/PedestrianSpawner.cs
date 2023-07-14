using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianSpawner : Spawner
{
    [SerializeField] private Transform _explosionTransform;
    public Transform ExplosionTransform => _explosionTransform;
    public static PedestrianSpawner Instance { get; private set; }

    public override void Awake()
    {
        Instance = this;
        base.Awake();
    }
}
