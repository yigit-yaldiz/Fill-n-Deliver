using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public abstract class Pool : MonoBehaviour
{
    public PathCreator PathCreator;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _spawnCount = 5;

    private void Awake()
    {
        PopulatePool();
    }

    private void PopulatePool(int count = 5)//count
    {
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = Instantiate(_prefab, transform.position, _prefab.transform.rotation, transform);
            prefab.gameObject.SetActive(false);
            prefab.transform.SetAsFirstSibling();
        }
    }

    public void Spawn()
    {
        if (transform.GetChild(0).gameObject.activeSelf)
        {
            PopulatePool(_spawnCount); //it also may be "transform.childCount"
        }

        Transform prefab = transform.GetChild(0);
        prefab.gameObject.SetActive(true);
        prefab.transform.SetAsLastSibling();
    }
}
