using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillTheDelivery.Vehicle;

[System.Serializable]
public class Vehicle : MonoBehaviour
{
    [SerializeField] private Animator[] _animators;
    [SerializeField] private float _vehicleSpeed;

    [SerializeField] Light _spotLight;

    private ColliderController _basket;
    public ColliderController Basket => _basket;
    public float VehicleSpeed => _vehicleSpeed;

    public Light SpotLight => _spotLight;

    private void Awake()
    {
        _basket = GetComponentInChildren<ColliderController>();
        _spotLight = GetComponentInChildren<Light>(true);
    }

    private void OnEnable()
    {
        AccidentController.OnAccident += DropTheVehicle;
    }

    private void OnDisable()
    {
        AccidentController.OnAccident -= DropTheVehicle;
    }

    public void SetAnimatorSpeed(float speed)
    {
        foreach (Animator item in _animators)
        {
            item.SetFloat("Speed", speed);
        }
    }

    public void DropTheVehicle()
    {
        if (AccidentController.Instance.State != AccidentStates.BikeRolling)
        {
            return;
        }

        if (!VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
        {
            ChangeRigidbodyStatus(false);
            ResetBody(4f);
        }

        AccidentController.Instance.ChangeTheState(AccidentStates.ProductExplosion);
    }

    public void ResetBody(float time)
    {
        StartCoroutine(Reset(time));

        IEnumerator Reset(float time)
        {
            yield return new WaitForSeconds(time);

            ChangeRigidbodyStatus(true);

            if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
            {
                transform.GetChild(1).localPosition = Vector3.zero;
                transform.GetChild(1).localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
            }
            else
            {
                transform.GetChild(1).localPosition = Vector3.zero;
                transform.GetChild(1).localRotation = Quaternion.Euler(Vector3.zero);
                //transform.GetChild(1) ("body") transfrom which is in the bike cannot replace in inspector panel
            }
        }
    }

    private void ChangeRigidbodyStatus(bool isKinematic)
    {
        Rigidbody[] rigidbodies = transform.GetChild(1).GetComponents<Rigidbody>();
        //transform.GetChild(1) ("body") transfrom which is in the bike cannot replace in inspector panel

        foreach (Rigidbody item in rigidbodies)
        {
            item.isKinematic = isKinematic;
        }
    }
}
