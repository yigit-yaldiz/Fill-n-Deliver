using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool Checked => _checked;

    public float DistanceTraveled => _distanceTravelled;

    private bool _checked;

    private float _distanceTravelled;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _distanceTravelled = other.GetComponentInParent<PlayerController>().TravelledDistance;
            CheckpointManager.Instance.StoreCheckedPoints(this);
        }
    }
}
