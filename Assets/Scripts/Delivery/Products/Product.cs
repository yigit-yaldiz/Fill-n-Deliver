using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillTheDelivery.Vehicle;
using UniqueGames.Saving;

public enum ProductTypes
{
    Coke,
    SodaCup,
    Box,
    Chocolate,
    Cream,
    Cereal,
    GasTank,
    LargeGasTank,
    Ketchup,
    Milk,
    Newspaper,
    LargeOvarian,
    Parquet,
    LargeParquet,
    PizzaCase,
    Soda,
    Spray,
    Carboy,
    Wheel
}

public abstract class Product : MonoBehaviour, ISaveable
{
    public ProductTypes GetProductType => _productType;
    public int GetPrice => _price;

    public bool IsItSold { get => _isItSold; set => _isItSold = value; }

    [System.NonSerialized]
    public bool IsItAtPlacingStage = true;

    [SerializeField] private ProductTypes _productType;
    [SerializeField] private int _price;

    private const float _lerpSpeed = 2f;
    
    private bool _isItSold;

    private Rigidbody _rb;

    private Case _myCase;

    private void Awake()
    {
        _rb = transform.GetChild(0).GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _myCase = GetComponent<Placer>().MyCase;

        if (_isItSold)
        {
            transform.parent.SetParent(ProductListWrapper.Instance.transform);
            transform.localPosition = Vector3.zero;
            transform.parent.gameObject.SetActive(false);
            //Destroy(transform.parent.gameObject);
        }

        if (!_myCase.IsItChecked)
        {
            _myCase.CheckListables();
            _myCase.IsItChecked = true;
        }

    }

    protected virtual void OnEnable()
    {
        StartCoroutine(Started());

        IEnumerator Started()
        {
            while (!IsItAtPlacingStage)
            {
                yield return null;
                GetComponent<Placer>().ResetPlaced();
            }
        }
    }

    public virtual void TransferToCustomer(Customer customer)
    {
        Vector3 startPos = transform.parent.position;
        Vector3 targetPos = customer.transform.position;
        Quaternion startRot = transform.parent.rotation;
        Quaternion targetRot = Quaternion.Euler(new Vector3(-15, 0, -15));
        Quaternion treshhold = Quaternion.Euler(new Vector3(-1, 0, -1));
        
        float t = 0;
        Rigidbody[] _rigidbodies = GetComponentsInChildren<Rigidbody>(true);

        foreach (var item in _rigidbodies)
        {
            SetConstraintsStatus(RigidbodyConstraints.None);
            item.isKinematic = true;
        }

        StartCoroutine(TimeCoroutine());

        IEnumerator TimeCoroutine()
        {
            if (startPos.z < targetPos.z) //left
            {
                targetRot *= treshhold;
            }

            while (t < 1)
            {
                t += Time.deltaTime * _lerpSpeed;
                transform.parent.position = Vector3.Lerp(startPos, targetPos, t);
                float interpolation = t * 2;
                transform.parent.rotation = Quaternion.Lerp(startRot, targetRot, interpolation);

                yield return null;
            }

            GetComponentInChildren<MeshRenderer>().enabled = false;
            transform.parent.SetParent(customer.transform);
        }

        _isItSold = true;
    }

    public virtual void DropProducts(Transform point)
    {
        SetConstraintsStatus(RigidbodyConstraints.None);
        Explode(point);

    }

    public void Explode(Transform point)
    {
        if (AccidentController.Instance.State != AccidentStates.ProductExplosion)
        {
            return;
        }

        Rigidbody rb = GetComponentInChildren<Rigidbody>();
        rb.isKinematic = false;

        float randomForce = Random.Range(190f, 210f);

        rb.AddExplosionForce(randomForce, point.position, 100);
    }

    public void SetConstraintsStatus(RigidbodyConstraints constraints)
    {
        _rb.constraints = constraints;
    }

    public void SetDeactiveTheProduct(float time)
    {
        StartCoroutine(SetActiveFalse(time));

        IEnumerator SetActiveFalse(float time)
        {
            yield return new WaitForSeconds(time);
            GetComponentInChildren<MeshRenderer>(true).enabled = false;
        }
    }

    public void SetTheProductsParent(Customer customer)
    {
        transform.SetParent(customer.transform);
    }

    public void MakeItExplosive()
    {
        gameObject.layer = 12;
        //DeactiveChildren(); //for straight floor
        
        //void DeactiveChildren()
        //{
        //    foreach (Transform child in transform.GetChild(0))
        //    {
        //        child.gameObject.SetActive(false);
        //    }
        //}
    }

    public void FreezeTheProduct(float time)
    {
        StartCoroutine(Freeze(time));

        IEnumerator Freeze(float time)
        {
            _rb.isKinematic = true;
            yield return new WaitForSeconds(time);
            _rb.isKinematic = false;
        }
    }

    public void ReturnToStore(float time)
    {
        StartCoroutine(Return());

        IEnumerator Return()
        {
            yield return new WaitForSeconds(time);
            Placer placer = GetComponent<Placer>();
            placer.ResetPlaced();
            placer.ResetToCase();
            placer.MyCase.AddReturnedPlacer(gameObject);
        }
    }

    [System.Serializable]
    struct SaveData
    {
        public bool productIsSold;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();
        saveData.productIsSold = _isItSold;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;
        _isItSold = saveData.productIsSold;
    }
}
