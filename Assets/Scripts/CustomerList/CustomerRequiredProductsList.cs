using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;
using Saving;

public class CustomerRequiredProductsList : MonoBehaviour, ISaveable
{
    private Dictionary<int, int> _requiredProductsDict = new Dictionary<int, int>();

    private CustomerTrigger _customer;

    public Dictionary<int, int> RequiredProductsDict { get => _requiredProductsDict; set => _requiredProductsDict = value; }

    private void Start()
    {
        _customer = GetComponent<CustomerTrigger>();
    }

    public void AddToCustomerRequests()
    {
        for (int i = 0; i < _customer.RequiredProducts.Count; i++)
        {
            RequiredProduct productList = _customer.RequiredProducts[i];
            int index = _customer.RequiredProducts.IndexOf(productList);

            if (_requiredProductsDict.ContainsKey(index))
            {
                _requiredProductsDict[index] = _customer.RequiredProducts[i].Count;
            }
            else
            {
                _requiredProductsDict[index] = new int();
                _requiredProductsDict[index] = _customer.RequiredProducts[i].Count;
            }
        }
    }

    public void RemoveFromCustomerRequests(Product product)
    {
        foreach (RequiredProduct requiredProduct in _customer.RequiredProducts)
        {
            if (requiredProduct.Type == product.GetProductType)
            {
                int index = _customer.RequiredProducts.IndexOf(requiredProduct);

                if (_requiredProductsDict.ContainsKey(index) && _requiredProductsDict[index] > 0)
                {
                    _requiredProductsDict[index]--;
                }
                else
                {
                    Debug.Log("There is nothing to decrease");
                }
            }
        }
    }

    [System.Serializable]
    struct SaveData
    {
        public Dictionary<int, int> requiredProductsDict;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();
        saveData.requiredProductsDict = _requiredProductsDict;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;
        _requiredProductsDict = saveData.requiredProductsDict;
    }
}
