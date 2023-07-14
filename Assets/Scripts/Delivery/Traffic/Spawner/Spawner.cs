using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField] private Pool[] _pools;
    [SerializeField] private float _spawnInterval;
    private Coroutine _spawnCoroutine;
    private Coroutine _delayCoroutine;

    public virtual void Awake()
    {
        _pools = transform.parent.GetComponentsInChildren<Pool>();
    }

    private void OnEnable()
    {
        AccidentController.OnAccident += StopSpawning;
    }

    private void OnDisable()
    {
        AccidentController.OnAccident -= StopSpawning;
    }

    private void Start()
    {
        _spawnCoroutine = StartCoroutine(Spawn(_spawnInterval));
    }

    private IEnumerator Spawn(float delayTime)
    {
        while (true)
        {
            foreach (Pool carPool in _pools)
            {
                carPool.Spawn();
                yield return new WaitForSeconds(delayTime);
            }
        }
    }

    private void StopSpawning()
    {
        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
        }

        _delayCoroutine = StartCoroutine(SpawnDelay(4f));

        IEnumerator SpawnDelay(float delayTime)
        {
            StopCoroutine(_spawnCoroutine);
            yield return new WaitForSeconds(delayTime + _spawnInterval);
            
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }

            _spawnCoroutine = StartCoroutine(Spawn(_spawnInterval));
        }  
    }
}
