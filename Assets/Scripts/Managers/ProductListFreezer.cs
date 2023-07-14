using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductListFreezer : MonoBehaviour
{
    #region Singleton Pattern
    public static ProductListFreezer Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public void FreezeAllProductsInBasket()
    {
        foreach (int key in ProductListManager._placedProductsKeys)
        {
            int count = ProductListManager.GetProductList(key).Count;

            for (int j = 0; j < count; j++)
            {
                ProductListManager.GetProductList(key)[count - (j + 1)].GetComponent<Placer>().FreezeRotationOfProduct();
            }
        }
    }

    public void DefrostAllProductsInBasket()
    {
        foreach (int key in ProductListManager._placedProductsKeys)
        {
            int count = ProductListManager.GetProductList(key).Count;

            for (int j = 0; j < count; j++)
            {
                ProductListManager.GetProductList(key)[count - (j + 1)].GetComponent<Placer>().DefrostRotationOfProduct();
            }
        }
    }
}
