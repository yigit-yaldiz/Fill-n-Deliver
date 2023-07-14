using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniqueGames.Saving;
using Saving;

[System.Serializable]
public struct RequiredProduct
{
    public ProductTypes Type;
    public int Count;
}

[RequireComponent(typeof(Customer))]
public class CustomerTrigger : MonoBehaviour, ISaveable
{
    #region Getters
    public int RequiredTotalProductCount => _requiredTotalProductCount;
    public int RequiredProductTypeCount => _requiredTypeCount;
    public List<RequiredProduct> RequiredProducts => _requiredProducts;
    #endregion

    private int _requiredTotalProductCount;
    private int _requiredTypeCount;

    [SerializeField] private List<RequiredProduct> _requiredProducts = new List<RequiredProduct>();

    [Header("UI Elements")] 
    [SerializeField] private ListTexture _listTexture;
    [SerializeField] private List<TextMeshProUGUI> _requirementsTexts = new List<TextMeshProUGUI>();

    [SerializeField] private bool _didCreateProducts;
    [SerializeField] private bool _didTakeAllProducts;
    private bool _isItDeactivated;

    private Customer _customer;
    private CustomerRequiredProductsList _customerList;

    private Animator _customerAnimator;

    private WaitForSeconds _transferDelay = new WaitForSeconds(0.05f); //sure uzun

    private AudioSource _transferSound;

    private void Awake()
    {
        _customerAnimator = GetComponentInChildren<Animator>();
        _customer = GetComponent<Customer>();
        _customerList = GetComponent<CustomerRequiredProductsList>();
        _transferSound = GetComponent<AudioSource>();

        _requiredTypeCount = _requiredProducts.Count;

        CalculateTotalRequiredProductCount();
    }

    private void Start()
    {
        CheckingLevelStatus();

        _requirementsTexts = _listTexture.RequestCountsTexts;

        if (_isItDeactivated || _didTakeAllProducts)
        {
            _listTexture.gameObject.SetActive(false);
            gameObject.SetActive(false);
            Customer.CustomerCount--;

            if (Customer.CustomerCount == 0)
            {
                LevelLoader.Instance.LoadSpecificScene(LevelLoader.Instance.ActiveSceneIndex);
                Customer.CustomerCount = 0;
            }


            return;
        }

        if (_didCreateProducts)
        {
            for (int i = 0; i < _customerList.RequiredProductsDict.Count; i++)
            {
                //Debug.Log(i + " " + transform.name, transform);
                RequiredProduct requiredProduct = _requiredProducts[i];
                requiredProduct.Count = _customerList.RequiredProductsDict[i];
                _requiredProducts[i] = requiredProduct;
            }
        }
        else
        {
            _customerList.AddToCustomerRequests();
            _didCreateProducts = true;
            SaveWrapper.Instance.Save();
        }

        CheckProgressBar();

        PrintCustomersPaper();

        for (int i = 0; i < _requiredProducts.Count; i++)
        {
            if (_requiredProducts[i].Count == 0)
            {
                _listTexture.RequestCountsTexts.RemoveAt(_requiredProducts.IndexOf(_requiredProducts[i]));
            }
        }

        CheckListCounts();
    }

    private void CheckingLevelStatus()
    {
        if (Cases.Instance.CaseList.Count == 0)
        {
            JobDone();
            CasePanelInput.Instance.CompletedImage.gameObject.SetActive(true);
            LevelLoader.Instance.LoadNextLevel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_didTakeAllProducts)
        {
            StartCoroutine(GiveRequiredProductsToCustomer(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _didTakeAllProducts)
        {
            StartCoroutine(DeactiveTheCustomer(5f));
            _isItDeactivated = true;
        }
    }

    private void PrintCustomersPaper()
    {
        for (int i = 0; i < _requiredProducts.Count; i++)
        {
            _requirementsTexts[i].text = "" + _requiredProducts[i].Count.ToString();
            //Debug.Log(_requiredProducts[i].Type + " " + _requiredProducts[i].Count.ToString(), transform);
        }
    }

    private void CalculateTotalRequiredProductCount()
    {
        for (int i = 0; i < _requiredProducts.Count; i++)
        {
            _requiredTotalProductCount += _requiredProducts[i].Count; //customer's total requiredProduct count
        }
    }
    
    private IEnumerator GiveRequiredProductsToCustomer(Transform player)
    {
        int textCounter = 0;

        Physics.IgnoreLayerCollision(8, 8);

        int givableProductCount = 0;

        for (int i = 0; i < _requiredProducts.Count; i++)
        {
            List<Product> requiredProductListByType = ProductListManager.GetProductList((int)_requiredProducts[i].Type);

            givableProductCount = 0;

            if (requiredProductListByType == null)
            {
                givableProductCount = 0;
            }
            else if (requiredProductListByType.Count < _requiredProducts[i].Count)
            {
                givableProductCount = requiredProductListByType.Count;
            }
            else
            {
                givableProductCount = _requiredProducts[i].Count;
            }

            ChangeTheLidStatus(givableProductCount, true);

            for (int j = 0; j < givableProductCount; j++)
            {
                _transferSound.Play();

                requiredProductListByType[requiredProductListByType.Count - 1].TransferToCustomer(_customer); // products to customer
                CashManager.Instance.IncreaseMoneyAmount(requiredProductListByType[requiredProductListByType.Count - 1]);

                _customerList.RemoveFromCustomerRequests(requiredProductListByType[requiredProductListByType.Count - 1]);

                ProductListManager.RemoveFromPlacedProduct(requiredProductListByType[requiredProductListByType.Count - 1]);

                _customer.GetMoneyList()[_customer.GetMoneyList().Count - 1].TransferMoneyToPlayer(player);
                _customer.DecreaseMoneyAmount();

                RequiredProduct requiredProduct = _requiredProducts[i];
                requiredProduct.Count--;
                _requiredProducts[i] = requiredProduct;

                _requirementsTexts[textCounter].text = "" + _requiredProducts[i].Count;

                if (requiredProduct.Count == 0)
                {
                    int index = _requiredProducts.IndexOf(requiredProduct);
                    _listTexture.Animators[index].SetTrigger("Complete"); //progressBar
                    _listTexture.Animators.RemoveAt(index);
                    _requirementsTexts[textCounter].text = "" + 0.ToString();
                    _requiredProducts.RemoveAt(i);
                    i--;    //!!! List count decreased. Don't do any operation with "i" at the underside !!!
                }

                if (_requiredProducts.Count == 0)
                {
                    JobDone();
                    StartCoroutine(ListTextureDeactivatingDelay());
                }
                
                yield return _transferDelay;
            }

            textCounter++;

            ChangeTheLidStatus(givableProductCount, false);
        }

        if (givableProductCount > 0)
        {
            SaveWrapper.Instance.Save();
        }

        Physics.IgnoreLayerCollision(8, 8, false);
    }

    private void JobDone()
    {
        _didTakeAllProducts = true;
        Customer.CustomerCount--;
        _customerAnimator.SetTrigger("JobDone");

        if (Customer.CustomerCount == 0)
        {
            GameManager.Instance.ChangeLevelStatusToFinished();
            GameManager.Instance.ActivateCompletePanel(delayTime: 1f);
        }
    }

    void CheckListCounts()
    {
        for (int i = 0; i < _requiredProducts.Count; i++)
        {
            if (_requiredProducts[i].Count == 0)
            {
                _requiredProducts.RemoveAt(i);
            }
        }
    }

    void CheckProgressBar()
    {
        for (int i = 0; i < _requiredProducts.Count; i++)
        {
            if (_requiredProducts[i].Count == 0)
            {
                _listTexture.Animators[_requiredProducts.IndexOf(_requiredProducts[i])].SetTrigger("Complete");
                _listTexture.Animators.RemoveAt(_requiredProducts.IndexOf(_requiredProducts[i]));
            }
        }
    }

    IEnumerator ListTextureDeactivatingDelay()
    {
        yield return new WaitForSeconds(1f);
        _listTexture.gameObject.SetActive(false);
    }

    IEnumerator DeactiveTheCustomer(float time)
    {
        yield return new WaitForSeconds(time); //2f
        gameObject.SetActive(false);
    }

    void ChangeTheLidStatus(int givableProductCount, bool status)
    {
        if (givableProductCount <= 0)
        {
            return;
        }

        if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<MotorcycleColliderUpgrader>())
        {
            MotorcycleColliderUpgrader.ResetLidAnimator();
            MotorcycleColliderUpgrader.ChangeLidStatus(status);
        }
        else if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
        {
            TukTukColliderUpgrader.ResetLidAnimator();
            TukTukColliderUpgrader.ChangeLidStatus(status);
        }
    }

    [System.Serializable]
    struct SaveData
    {
        public bool didCreateProducts;
        public bool didTakeAllProduct;
        public bool isItDeactivated;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();
        saveData.didCreateProducts = _didCreateProducts;
        saveData.didTakeAllProduct = _didTakeAllProducts;
        saveData.isItDeactivated = _isItDeactivated;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;
        _didCreateProducts = saveData.didCreateProducts;
        _didTakeAllProducts = saveData.didTakeAllProduct;
        _isItDeactivated = saveData.isItDeactivated;
    }
}
