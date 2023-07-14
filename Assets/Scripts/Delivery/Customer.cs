using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;

public class Customer : MonoBehaviour, ISaveable
{
    [SerializeField] private GameObject _moneyPrefab;
    [SerializeField] private List<MoneyScript> _moneys = new List<MoneyScript>();

    public static int CustomerCount = 0;
    
    private int _requiredProductCount;

    private void Start()
    {
        CustomerCount++;

        _requiredProductCount = GetComponent<CustomerTrigger>().RequiredTotalProductCount;

        for (int i = 0; i < _requiredProductCount; i++)
        {
            Instantiate(_moneyPrefab, transform.position, _moneyPrefab.transform.rotation, transform);
        }

        foreach (Transform child in transform)
        {
            if (child.GetComponent<MoneyScript>())
            {
                MoneyScript money = child.GetComponent<MoneyScript>();
                money.GetComponent<MeshRenderer>().enabled = false;
                _moneys.Add(money);
            }  
        }
    }

    public List<MoneyScript> GetMoneyList()
    {
        return _moneys;
    }

    public void DecreaseMoneyAmount()
    {
        _moneys.RemoveAt(_moneys.Count - 1);
    }

    [System.Serializable]
    struct SaveData
    {
        public int requiredProductCount;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();
        saveData.requiredProductCount = _requiredProductCount;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData) state;
        _requiredProductCount = saveData.requiredProductCount;
    }
}
