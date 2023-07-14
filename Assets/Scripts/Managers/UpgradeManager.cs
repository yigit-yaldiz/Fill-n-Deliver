using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniqueGames.Saving;
using Saving;

namespace FillTheDelivery.UI
{
    public class UpgradeManager : MonoBehaviour, ISaveable
    {
        public enum UpgradeType 
        { 
            Width, 
            Depth, 
            Height
        }

        [Header("Upgrade Levels")]
        private int _widthLevel = 0;
        private int _depthLevel = 0;
        private int _heightLevel = 0;
        private int _tierLevel = 1;
        public int WidthLevel => _widthLevel;
        public int DepthLevel => _depthLevel;
        public int HeightLevel => _heightLevel;
        public int GetTierLevel => _tierLevel;

        [Header("Progress Images")]
        [SerializeField] private Image[] _widthProgress;
        [SerializeField] private Image[] _depthProgress;
        [SerializeField] private Image[] _heightProgress;

        [Header("Max Objects")]
        [SerializeField] private GameObject _widthMaxObject;
        [SerializeField] private GameObject _depthMaxObject;
        [SerializeField] private GameObject _heightMaxObject;

        [Header("Panels")]
        [SerializeField] private TextMeshProUGUI _tierLevelText;
        [SerializeField] private GameObject _tierUpPanel;
        [SerializeField] private GameObject _moneyWarningPanel;

        [Header("ScriptableObjects")]
        [SerializeField] private CostsAmounts _widthCostsAmounts;
        [SerializeField] private CostsAmounts _depthCostsAmounts;
        [SerializeField] private CostsAmounts _heightCostsAmounts;

        [Header("Price Texts")]
        [SerializeField] private TextMeshProUGUI _widthCostText;
        [SerializeField] private TextMeshProUGUI _depthCostText;
        [SerializeField] private TextMeshProUGUI _heightCostText;

        private bool _isItMaxed => (_widthLevel >= 4) && (_depthLevel >= 4) && (_heightLevel >= 4);

        #region Events           
        public delegate void LevelUpAction(UpgradeType type, int level);
        public static event LevelUpAction OnLevelUp;

        public delegate void TierUpAction(int tierLevel);
        public static event TierUpAction OnTierUp;
        #endregion

        #region Singelton Pattern
        public static UpgradeManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        #endregion

        private void OnEnable()
        {
            WriteUpgradeCosts();
        }

        public void UpgradeWidthButton()
        {
            if (_widthLevel >= 4)
            {
                return;
            }

            if (_widthCostsAmounts.UpgradeCosts[_widthLevel] > CashManager.Instance.GetMoney)
            {
                _moneyWarningPanel.SetActive(true);
                //gameObject.SetActive(false);
                return;
            }

            CashManager.Instance.DecreaseMoneyAmount(_widthCostsAmounts.UpgradeCosts[_widthLevel]);
            
            _widthLevel++;

            _widthCostText.text = _widthCostsAmounts.UpgradeCosts[_widthLevel].ToString();

            StartCoroutine(AnimateCoroutine());

            IEnumerator AnimateCoroutine()
            {
                int index = _widthLevel - 1;

                OnLevelUp(UpgradeType.Width, _widthLevel);

                float t = 0;

                while (t < 1)
                {
                    t += 1 * Time.deltaTime;
                    _widthProgress[index].fillAmount = t;
                    yield return null;
                }

                SaveWrapper.Instance.Save();

                if (index >= 3)
                {
                    _widthMaxObject.SetActive(true);
                    _widthCostText.gameObject.SetActive(false);

                    if (_isItMaxed)
                    {
                        OpenTheTierButton();
                    }
                }
            }
        }

        public void UpgradeDepthButton()
        {
            if (_depthLevel >= 4)
            {
                return;
            }

            if (_depthCostsAmounts.UpgradeCosts[_depthLevel] > CashManager.Instance.GetMoney)
            {
                _moneyWarningPanel.SetActive(true);
                //gameObject.SetActive(false);
                return;
            }

            CashManager.Instance.DecreaseMoneyAmount(_depthCostsAmounts.UpgradeCosts[_depthLevel]);
            
            _depthLevel++;

            _depthCostText.text = _depthCostsAmounts.UpgradeCosts[_depthLevel].ToString();

            StartCoroutine(AnimateCoroutine());

            IEnumerator AnimateCoroutine()
            {
                int index = _depthLevel - 1;

                OnLevelUp(UpgradeType.Depth, _depthLevel);

                float t = 0;

                while (t < 1)
                {
                    t += 1 * Time.deltaTime;
                    _depthProgress[index].fillAmount = t;
                    yield return null;
                }

                SaveWrapper.Instance.Save();

                if (index >= 3)
                {
                    _depthMaxObject.SetActive(true);
                    _depthCostText.gameObject.SetActive(false);

                    if (_isItMaxed)
                    {
                        OpenTheTierButton();
                    }
                }
            }
        }

        public void UpgradeHeightButton()
        {

            if (_heightLevel >= 4)
            {
                return;
            }

            if (_heightCostsAmounts.UpgradeCosts[_heightLevel] > CashManager.Instance.GetMoney)
            {
                _moneyWarningPanel.SetActive(true);
                //gameObject.SetActive(false);
                return;
            }

            CashManager.Instance.DecreaseMoneyAmount(_depthCostsAmounts.UpgradeCosts[_heightLevel]);
            
            _heightLevel++;

            _heightCostText.text = _heightCostsAmounts.UpgradeCosts[_heightLevel].ToString();

            StartCoroutine(AnimateCoroutine());

            IEnumerator AnimateCoroutine()
            {
                int index = _heightLevel - 1;

                OnLevelUp(UpgradeType.Height, _heightLevel);

                float t = 0;

                while (t < 1)
                {
                    t += 1 * Time.deltaTime;
                    _heightProgress[(index)].fillAmount = t;
                    yield return null;
                }

                SaveWrapper.Instance.Save();

                if (index >= 3)
                {
                    _depthCostText.gameObject.SetActive(false);
                    _heightMaxObject.SetActive(true);

                    if (_isItMaxed)
                    {
                        OpenTheTierButton();
                    }
                }
            }
        }

        private void OpenTheTierButton()
        {
            _tierUpPanel.SetActive(true);
        }

        public void IncreaseTheTierLevel()
        {
            _tierLevel++;
            _tierLevelText.text = "Tier " + _tierLevel;
            _tierUpPanel.SetActive(false);

            ResetLevels();
            OnTierUp(_tierLevel);
            WriteUpgradeCosts();
        }

        private void WriteUpgradeCosts()
        {
            _widthCostText.gameObject.SetActive(true);
            _depthCostText.gameObject.SetActive(true);
            _heightCostText.gameObject.SetActive(true);

            _widthCostText.text = _widthCostsAmounts.UpgradeCosts[_widthLevel].ToString();
            _depthCostText.text = _depthCostsAmounts.UpgradeCosts[_depthLevel].ToString();
            _heightCostText.text = _heightCostsAmounts.UpgradeCosts[_heightLevel].ToString();
        }

        private void ResetLevels()
        {
            for (int i = 0; i < _widthProgress.Length; i++)
            {
                _widthProgress[i].fillAmount = 0;
                _depthProgress[i].fillAmount = 0;
                _heightProgress[i].fillAmount = 0;
            }
            
            _widthLevel = 0;
            _depthLevel = 0;
            _heightLevel = 0;

            _widthMaxObject.SetActive(false);
            _depthMaxObject.SetActive(false);
            _heightMaxObject.SetActive(false);
        }

        [System.Serializable]
        struct SaveData
        {
            public int widthLevel;
            public int depthLevel;
            public int heightLevel;
            public int tierLevel;

            public bool isItMaxed;
        }

        public object CaptureState()
        {
            SaveData saveData = new SaveData();
            saveData.widthLevel = _widthLevel;
            saveData.depthLevel = _depthLevel;
            saveData.heightLevel = _heightLevel;
            saveData.tierLevel = _tierLevel;
            saveData.isItMaxed = _isItMaxed;

            return saveData;
        }

        public void RestoreState(object state)
        {
            SaveData saveData = (SaveData)state;
            
            _widthLevel = saveData.widthLevel;
            _depthLevel = saveData.depthLevel;
            _heightLevel = saveData.heightLevel;
            _tierLevel = saveData.tierLevel;

            GameObject[] maxObjects = new GameObject[] { _widthMaxObject, _depthMaxObject, _heightMaxObject };

            if (_widthLevel > 0)
            {
                for (int i = 0; i < _widthLevel; i++)
                {
                    _widthProgress[i].fillAmount = 1;
                }
                
                _widthCostText.text = _widthCostsAmounts.UpgradeCosts[_widthLevel].ToString();
            }

            if (_depthLevel > 0)
            {
                for (int i = 0; i < _depthLevel; i++)
                {
                    _depthProgress[i].fillAmount = 1;
                }
                _depthCostText.text = _depthCostsAmounts.UpgradeCosts[_depthLevel].ToString();
            }

            if (_heightLevel > 0)
            {
                for (int i = 0; i < _heightLevel; i++)
                {
                    _heightProgress[i].fillAmount = 1;
                }
                _heightCostText.text = _heightCostsAmounts.UpgradeCosts[_heightLevel].ToString();
            }

            if (saveData.isItMaxed)
            {
                foreach (var maxObject in maxObjects)
                {
                    maxObject.SetActive(true);
                }

                OpenTheTierButton();
            }
        }
    }
}
