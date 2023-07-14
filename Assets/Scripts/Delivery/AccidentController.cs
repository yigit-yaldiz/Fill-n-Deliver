using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AccidentStates
{
    Start,
    PlayerRolling,
    BikeRolling,
    ProductExplosion,
    PedestrianRolling,
}

public class AccidentController : MonoBehaviour
{
    [SerializeField] GameObject _accidentCam;
    [SerializeField] GameObject _deliveryCam;
    public static AccidentController Instance { get; private set; }

    public AccidentStates State;

    const float c_slowMoAcceleration=2;
    const float c_reverseSlowMoAcceleration=4;
    const float c_slowMoAmount = 0.5f;

    public delegate void AccidentAction();
    public static event AccidentAction OnAccident;

    private void Awake()
    {
        Instance = this;
        State = AccidentStates.Start;
    }

    private void OnEnable()
    {
        OnAccident += DecreaseProductAmount;
    }

    private void OnDisable()
    {
        OnAccident -= DecreaseProductAmount;
    }

    private void DecreaseProductAmount()
    {
        if (State == AccidentStates.ProductExplosion)
        {
            CameraEffect();

            if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<MotorcycleColliderUpgrader>())
            {
                MotorcycleColliderUpgrader.ResetLidAnimator();
                MotorcycleColliderUpgrader.ChangeLidStatus(false);
            }
            else if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
            {
                TukTukColliderUpgrader.ResetLidAnimator();
                TukTukColliderUpgrader.ChangeLidStatus(false);
            }
            
            ProductListWrapper.Instance.DecreaseProductAmount();
            ChangeTheState(AccidentStates.Start);
            
            if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<MotorcycleColliderUpgrader>())
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
    void CameraEffect()
    {
        _deliveryCam.SetActive(false);
        _accidentCam.SetActive(true);
        StartCoroutine(SlowMotion());

        IEnumerator SlowMotion()
        {
            float t = 0;
            while(t<1)
            {
                t += c_slowMoAcceleration * Time.deltaTime;
                Time.timeScale=Mathf.Lerp(1, c_slowMoAmount, t);
                yield return null;
            }
        }
    }
    public void ResetCameraEffect()
    {
        _deliveryCam.SetActive(true);
        _accidentCam.SetActive(false);
        StartCoroutine(ReverseSlowMotion());

        IEnumerator ReverseSlowMotion()
        {
            float t = 0;
            float startAmount = Time.timeScale;
            while(t<1)
            {
                t += c_reverseSlowMoAcceleration * Time.deltaTime;
                Time.timeScale=Mathf.Lerp(startAmount, 1, t);
                yield return null;
            }
        }
    }

    public void ChangeTheState(AccidentStates state)
    {
        State = state;
        OnAccident();
    }
}
