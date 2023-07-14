using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Store : MonoBehaviour
{
    [SerializeField] private GameObject _casePanel;
    [SerializeField] private CinemachineVirtualCamera _deliveryVirtualCam;
    [SerializeField] private CinemachineVirtualCamera _caseVirtualCam;
    [SerializeField] private GameObject _cases;

    public static Store Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            player.GoToFillingPos();
            player.ResetPlayerSpeed();
            player.isInputAllowed = false;

            StartCoroutine(CasesAppearDelay(0.55f));
            Filling();
            CheckpointManager.Instance.ClearTheCheckpointList();
        }
    }
    private void Filling()
    {
        GameManager.Instance.ChangeGameState(GameStates.Placement);

        _casePanel.SetActive(true);
        _deliveryVirtualCam.gameObject.SetActive(false);
        _caseVirtualCam.gameObject.SetActive(true);
    }

    public void Delivering()
    {
        _cases = CasePanelInput.Instance.Cases;

        GameManager.Instance.ChangeGameState(GameStates.Delivery);

        //if (_cases.activeSelf || _cases.activeInHierarchy)
        //{
        //    _cases.SetActive(false);
        //}

        _casePanel.SetActive(false);
        _deliveryVirtualCam.gameObject.SetActive(true);
        _caseVirtualCam.gameObject.SetActive(false);
    }

    private IEnumerator CasesAppearDelay(float time)
    {
        yield return new WaitForSeconds(time);
        _cases = CasePanelInput.Instance.Cases;
        _cases.SetActive(true);
    }

    public void OpenTheLid()
    {
        if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<BikeColliderUpgrader>())
        {
            return;
        }
        else if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<MotorcycleColliderUpgrader>())
        {
            MotorcycleColliderUpgrader.ResetLidAnimator();
            MotorcycleColliderUpgrader.ChangeLidStatus(true);
        }
        else if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
        {
            TukTukColliderUpgrader.ResetLidAnimator();
            TukTukColliderUpgrader.ChangeLidStatus(true);
        }
    }

    public void CloseTheLid()
    {
        if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<BikeColliderUpgrader>())
        {
            return;
        }
        else if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<MotorcycleColliderUpgrader>())
        {
            MotorcycleColliderUpgrader.ResetLidAnimator();
            MotorcycleColliderUpgrader.ChangeLidStatus(false);
        }
        else if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
        {
            TukTukColliderUpgrader.ResetLidAnimator();
            TukTukColliderUpgrader.ChangeLidStatus(false);
        }
    }
}
