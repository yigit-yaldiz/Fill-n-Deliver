using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private List<Checkpoint> _checkpoints = new List<Checkpoint>();

    public static CheckpointManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void StoreCheckedPoints(Checkpoint checkpoint)
    {
        _checkpoints.Add(checkpoint);
    }

    public void SpawnAtLastCheckPoint()
    {
        float distanceTravelled;
        
        if (_checkpoints.Count != 0)
        {
            distanceTravelled = _checkpoints[_checkpoints.Count - 1].DistanceTraveled;
        }
        else
        {
            Debug.LogWarning("There is no checkpoint available !!! You are spawned at the beginning");
            distanceTravelled = 0;
        }

        PlayerController.Instance.ChangeDistanceTravelled(distanceTravelled);
    }

    public void ClearTheCheckpointList()
    {
        _checkpoints.Clear();
    }
}
