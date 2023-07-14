using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductListWrapper : MonoBehaviour
{
    [SerializeField] private Transform _explosionCenter;
    private const float _dropPercentage = 0.5f;

    #region Singleton
    public static ProductListWrapper Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_explosionCenter.position, 0.5f);
    }

    public void DecreaseProductAmount()
    {
        ProductListManager.GetKeys();

        Physics.IgnoreLayerCollision(8, 8);
        Physics.IgnoreLayerCollision(11, 8);

        foreach (int key in ProductListManager._placedProductsKeys)
        {
            int count = ProductListManager.GetProductList(key).Count;
            float rate = count * _dropPercentage;
            int removableCount = Mathf.CeilToInt(rate);
            int remainCount = count - removableCount;

            if (removableCount >= 1)
            {
                for (int j = 0; j < removableCount; j++)
                {
                    ProductListManager.GetProductList(key)[count - (j + 1)].MakeItExplosive(); //explosive layer added
                    ProductListManager.GetProductList(key)[count - (j + 1)].DropProducts(_explosionCenter); //animation line
                    ProductListManager.GetProductList(key)[count - (j + 1)].SetDeactiveTheProduct(4f); //set active false
                    ProductListManager.GetProductList(key)[count - (j + 1)].ReturnToStore(4f);
                    //ProductListManager.RemoveFromPlacedProduct(ProductListManager.GetProductList(key)[count - (j + 1)]); //remove operation from dictionary
                }
            }

            if (remainCount >= 1)
            {
                for (int i = 0; i < remainCount; i++)
                {
                    ProductListManager.GetProductList(key)[remainCount - (i + 1)].FreezeTheProduct(4f);
                }
            }
        }
    }
}
