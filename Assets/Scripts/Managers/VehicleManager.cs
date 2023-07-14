using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillTheDelivery.UI;
using UniqueGames.Saving;
using Saving;

public class VehicleManager : MonoBehaviour, ISaveable
{
    [SerializeField] private Cases _cases;
    private Vehicle _activeVehicle;
    private Spine _activeSpine;
    private int _activeVehicleIndex;
    private int _activeSpineIndex;

    public Cases Cases => _cases;
    public Vehicle GetActiveVehicle => _activeVehicle;

    [Tooltip("All vehicle tiers should add to this array")]
    [SerializeField] private Vehicle[] _vehicles;
    [SerializeField] private Spine[] _spines;

    public static VehicleManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        UpgradeManager.OnTierUp += UpgradeTheVehicle;
    }

    private void OnDisable()
    {
        UpgradeManager.OnTierUp -= UpgradeTheVehicle;
    }

    private void Start()
    {
        _activeVehicle = _vehicles[_activeVehicleIndex];
        _activeSpine = _spines[_activeSpineIndex];

        foreach (var vehicle in _vehicles)
        {
            vehicle.gameObject.SetActive(false);
        }

        _activeVehicle.gameObject.SetActive(true);
        LookAtCharacterFollower.Instance.SetFollowTransfrom(_activeSpine.transform);

        CasesObjectInitialSettings();

        if (_activeVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
        {
            _cases.transform.localPosition += Vector3.back / 2;
        }

        _cases = _activeVehicle.GetComponentInChildren<Cases>();
    }

    private void CasesObjectInitialSettings()
    {
        CasePanelInput.Instance.Vehicle = _activeVehicle;
        _cases.transform.SetParent(_activeVehicle.transform);

        if (_activeVehicle.GetComponentInChildren<MotorcycleColliderUpgrader>())
        {
            _cases.transform.localPosition += Vector3.back * 1.15f;
        }
        else if (_activeVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
        {
            _cases.transform.localPosition += Vector3.back * 1.8f;
        }
    }

    private void UpgradeTheVehicle(int tierLevel)
    {
        ProductListManager.GetKeys();

        foreach (var key in ProductListManager._placedProductsKeys)
        {
            int count = ProductListManager.GetProductList(key).Count;

            for (int i = 0; i < count; i++)
            {
                Placer placer = ProductListManager.GetProductList(key)[count - (i + 1)].GetComponent<Placer>();

                if (placer.PlacerState == Placer.State.Placed)
                {
                    ProductListManager.GetProductList(key)[count - (i + 1)].transform.parent.SetParent(_cases.transform);
                    placer.ResetPlaced();
                    placer.ResetToCase();
                }
            }
        }

        //ProductListManager.ResetDictionary();

        GameObject newVehicle = _vehicles[tierLevel - 1].gameObject;
        _activeVehicle = newVehicle.GetComponent<Vehicle>();
        _vehicles[tierLevel - 2].gameObject.SetActive(false);

        #region Cases Object Settings
        CasePanelInput.Instance.Vehicle = _activeVehicle;
        _cases.transform.SetParent(_activeVehicle.transform);

        if (_activeVehicle.GetComponentInChildren<MotorcycleColliderUpgrader>())
        {
            _cases.transform.localPosition += Vector3.back * 1.15f;
        }
        else if (_activeVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
        {
            _cases.transform.localPosition += Vector3.back * 0.75f;
        }
        #endregion

        CasePanelInput.Instance.Cases = _activeVehicle.GetComponentInChildren<Cases>().gameObject;
        PlacementRay.Instance._case = _activeVehicle.GetComponentInChildren<Case>();

        SetVehicleSpeed();
        SetRagdoll();

        newVehicle.SetActive(true);

        _activeVehicleIndex++;

        SetCurrentDriver();

        GameManager.Instance.ChangeGameState(GameStates.Placement);

        SaveWrapper.Instance.Save();
    }

    private void SetCurrentDriver()
    {
        _activeSpineIndex++;

        _activeSpine = _spines[_activeSpineIndex];

        LookAtCharacterFollower.Instance.SetFollowTransfrom(_activeSpine.transform);
    }

    public void SetRagdoll()
    {
        PlayerController.Instance.Ragdoll = _activeVehicle.GetComponentInChildren<RagdollController>();
    }

    public void SetVehicleSpeed()
    {
        PlayerController.Instance.MaxSpeed = _activeVehicle.VehicleSpeed;
    }

    [System.Serializable]
    struct SaveData
    {
        public int activeVehicleIndex;
        public int activeSpineIndex;
        //public SerializableVector3 casesLocalPos;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();
        saveData.activeVehicleIndex = _activeVehicleIndex;
        saveData.activeSpineIndex = _activeSpineIndex;
        //saveData.casesLocalPos = new SerializableVector3(_cases.transform.localPosition);

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;

        _activeVehicleIndex = saveData.activeVehicleIndex;
        _activeSpineIndex = saveData.activeSpineIndex;
        //_cases.transform.localPosition = saveData.casesLocalPos.ToVector();
    }
}
