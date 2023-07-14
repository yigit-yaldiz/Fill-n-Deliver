using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ghost : MonoBehaviour
{
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();
    private bool _isOccupied => _inCount != 0;
    public bool GetIsOccupied => _isOccupied;
    
    private int _inCount = 0;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        if (GetComponent<MeshRenderer>())
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        else if (GetComponentInChildren<MeshRenderer>())
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _meshRenderer.enabled = true;
        _inCount++;
        gameObjects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        _meshRenderer.enabled = false;
        _inCount--;
        gameObjects.Remove(other.gameObject);
    }

    public void ResetIncount()
    {
        _inCount = 0;
        gameObjects.Clear();
    }
}
