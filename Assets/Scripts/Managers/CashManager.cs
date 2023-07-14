using UnityEngine;
using TMPro;
using UniqueGames.Saving;
using Saving;

public class CashManager : MonoBehaviour, ISaveable
{
    [SerializeField] private TextMeshProUGUI _moneyAmountText;

    private float _money = 0;
    public float GetMoney => _money;

    public static CashManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this; 
    }

    private void Start()
    {
        _moneyAmountText.text = "Money: " + _money;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            MoneyHack();
        }
    }
    public void IncreaseMoneyAmount(Product product)
    {
        _money += product.GetPrice;
        _moneyAmountText.text = "Money: " + _money;
        SaveWrapper.Instance.Save();
    }

    public void DecreaseMoneyAmount(int price)
    {
        _money -= price;
        _moneyAmountText.text = "Money: " + _money;
        SaveWrapper.Instance.Save();
    }

    private void MoneyHack()
    {
        _money += 1000;
        _moneyAmountText.text = "Money: " + _money;
        SaveWrapper.Instance.Save();
    }

    [System.Serializable]
    struct SaveData
    {
        public float money;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();
        saveData.money = _money;
        
        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData) state;
        _money = saveData.money;
        _moneyAmountText.text = "Money: " + _money;
    }
}    
