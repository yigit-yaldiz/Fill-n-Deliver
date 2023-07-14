using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CasePanelInput : MonoBehaviour
{
    public static CasePanelInput Instance { get; private set; }
    public Image CompletedImage { get => _completedImage; set => _completedImage = value; }

    public Vehicle Vehicle;
    [SerializeField] private Store _store;
    [SerializeField] private PlayerController _player;

    #region Swipe Control Variables
    public GameObject Cases;
    [SerializeField] private float _speed = 2f;
    #endregion

    [SerializeField] private Image _completedImage;
    
    [Header("Case's Swipe Clamp Values")]
    [SerializeField] private float _maxClamp;
    [SerializeField] private float _minClamp;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        SwipeController.OnSwipe += WhenSwipe;
    }
    private void OnDisable()
    {
        SwipeController.OnSwipe -= WhenSwipe;
    }

    private void Start()
    {
        Vehicle = VehicleManager.Instance.GetActiveVehicle;
        Cases = VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<Cases>().gameObject;
    }

    private void WhenSwipe(float delta)
    {
        bool isItPlacement = GameManager.Instance.GameState == GameStates.Placement;
        
        if (isItPlacement)
        {
            Vector3 pos = Cases.transform.localPosition;
            pos.x -= delta * -_speed;
            pos.x = Mathf.Clamp(pos.x, _minClamp, _maxClamp);
            Cases.transform.localPosition = pos;
        }
    }

    public void GoButton()
    {
        if (PlacementRay.Instance.FirstTime && PlacementRay.Instance.Animator != null)
        {
            PlacementRay.Instance.Animator.SetTrigger("hold");
        }

        _store.Delivering();
        _player.InputDelay(1f);

        Store.Instance.CloseTheLid();
        ProductListFreezer.Instance.FreezeAllProductsInBasket();

        Cases.SetActive(false);
    }
}
