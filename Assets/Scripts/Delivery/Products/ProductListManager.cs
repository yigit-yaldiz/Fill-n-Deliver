using System.Collections.Generic;
using UnityEngine;

public static class ProductListManager
{
    private static Dictionary<int, List<Product>> _placedProductDict = new Dictionary<int, List<Product>>();
    
    public static List<int> _placedProductsKeys = new List<int>();

    public static void AddToPlacedProduct(Product product)
    {
        int productTypeIndex = (int) product.GetProductType;

        if (_placedProductDict.ContainsKey(productTypeIndex))
        {
            _placedProductDict[productTypeIndex].Add(product);
        }
        else
        {
            _placedProductDict[productTypeIndex] = new List<Product>();
            _placedProductDict[productTypeIndex].Add(product);
        }
    }

    public static void RemoveFromPlacedProduct(Product product)
    {
        int productTypeIndex = (int)product.GetProductType;

        if (_placedProductDict.ContainsKey(productTypeIndex))
        {
            _placedProductDict[productTypeIndex].Remove(product);
        }
        else
        {
            Debug.LogWarning("Product which you trying to remove from the dictionary isn't added at the first place !");
        }
    }

    public static List<Product> GetProductList(int productTypeIndex)
    {
        if (_placedProductDict.ContainsKey(productTypeIndex))
        {
            return _placedProductDict[productTypeIndex];
        }
        else
        {
            return null;
        }
    }

    public static void ResetDictionary()
    {
        _placedProductDict.Clear();
        _placedProductsKeys.Clear();
    }

    public static List<int> GetKeys()
    {
        foreach (KeyValuePair<int, List<Product>> value in _placedProductDict)
        {
            _placedProductsKeys.Add(value.Key);
        }

        return _placedProductsKeys;
    }
}
