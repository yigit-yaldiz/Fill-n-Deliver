using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public RagdollController Ragdoll;
    [HideInInspector]
    public float MaxSpeed = 1.5f;

    public float TravelledDistance => _distanceTravelled;
    public float FillingPos => _fillingPos;

    public bool InteractedWithPedestrian { get => _interactedWithPedestrian; private set => _interactedWithPedestrian = value; }

    private Animator _animator;

    [SerializeField] private PathCreator _pathCreator;
    [SerializeField] private float _distanceTravelled;
    [SerializeField] private float _acceleration = 4f;
    [SerializeField] private float _fillingPos;

    [SerializeField] Light _spotLight; 

    private const float _animSpeed = 0.5f;
    private float _speed = 0f;

    private bool _interactedWithPedestrian;

    #region Coroutines
    private Coroutine _positionCoroutine;
    private Coroutine _speedUpCoroutine;
    private Coroutine _speedDownCoroutine;
    private Coroutine _inputDelayCoroutine;
    #endregion

    public bool isInputAllowed = true;

    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        AccidentController.OnAccident += Respawn;
    }

    private void OnDisable()
    {
        AccidentController.OnAccident -= Respawn;
    }

    private void Start()
    {
        _spotLight = VehicleManager.Instance.GetActiveVehicle.SpotLight;
        VehicleManager.Instance.SetVehicleSpeed();
        VehicleManager.Instance.SetRagdoll();
    }

    private void Update()
    {
        if (!isInputAllowed)
        {
            ResetPlayerSpeed();
            return;
        }

        if (Input.GetMouseButtonDown(0) && GameManager.Instance.GameState == GameStates.Delivery)
        {
            if (_speedDownCoroutine != null)
            {
                StopCoroutine(_speedDownCoroutine);
            }

            if (PlacementRay.Instance.FirstTime && PlacementRay.Instance.Animator != null)
            {
                PlacementRay.Instance.Animator.ResetTrigger("hold");
                PlacementRay.Instance.TutorialFinished();
            }

            _speedUpCoroutine = StartCoroutine(SpeedUp());
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if (_speedUpCoroutine != null)
            {
                StopCoroutine(_speedUpCoroutine);
            }
            
            _speedDownCoroutine = StartCoroutine(SpeedDown());
        }

        if (Input.GetMouseButton(0) && GameManager.Instance.GameState == GameStates.Delivery)
        {
            _positionCoroutine = StartCoroutine(SetPosition());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pedestrian"))
        {
            _interactedWithPedestrian = true;
        }
    }

    private void Respawn()
    {
        if (AccidentController.Instance.State != AccidentStates.PlayerRolling)
        {
            return;
        }

        if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
        {
            _animator.SetTrigger("accidentTrig");
            //_animator.ResetTrigger("accidentTrig");
        }

        if (_positionCoroutine != null)
        {
            StopCoroutine(_positionCoroutine);
        }

        StopCoroutine(_speedUpCoroutine);
        Ragdoll.ChangeRagdollStatus(true);
        Ragdoll.Explosion();

        if (_interactedWithPedestrian)
        {
            _interactedWithPedestrian = false;
        }

        isInputAllowed = false;
        AccidentController.Instance.ChangeTheState(AccidentStates.BikeRolling);
        _spotLight.range = 2.5f;

        StartCoroutine(ResetAccidentDelay(4));
    }
    IEnumerator ResetAccidentDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetAfterAccident();
    }

    void ResetAfterAccident()
    {
        ResetSetPosition();
        isInputAllowed = true;
        _inputDelayCoroutine = null;
        ResetRagdoll();
        ResetTheAccident();
        GameManager.Instance.OnAccidentFinished();
    }

    void ResetSetPosition()
    {
        CheckpointManager.Instance.SpawnAtLastCheckPoint();

        _distanceTravelled += _speed * Time.deltaTime;
        #region Position Settings
        Vector3 pos = transform.position;
        pos.x = _pathCreator.path.GetPointAtDistance(_distanceTravelled).x;
        pos.z = _pathCreator.path.GetPointAtDistance(_distanceTravelled).z;
        pos.y = transform.position.y;
        transform.position = pos;
        #endregion
        transform.rotation = _pathCreator.path.GetRotationAtDistance(_distanceTravelled);
        transform.eulerAngles += new Vector3(0, 0, -90);
    }
    private IEnumerator SetPosition(float time = 0)
    {
        yield return new WaitForSeconds(time);

        _distanceTravelled += _speed * Time.deltaTime;
        #region Position Settings
        Vector3 pos = transform.position;
        pos.x = _pathCreator.path.GetPointAtDistance(_distanceTravelled).x;
        pos.z = _pathCreator.path.GetPointAtDistance(_distanceTravelled).z;
        pos.y = _pathCreator.path.GetPointAtDistance(_distanceTravelled).y;
        //pos.y = transform.position.y;
        #endregion
        transform.SetPositionAndRotation(pos, _pathCreator.path.GetRotationAtDistance(_distanceTravelled));
        transform.eulerAngles += new Vector3(0, 0, -90);
    }

    public void InputDelay(float time)
    {
        if (_inputDelayCoroutine != null)
        {
            return;
        }

        _inputDelayCoroutine = StartCoroutine(InputDelayCoroutine(time));

        IEnumerator InputDelayCoroutine(float time)
        {
            isInputAllowed = false;
            yield return new WaitForSeconds(time);
            isInputAllowed = true;
            _inputDelayCoroutine = null;
        }
    }

    public void GoToFillingPos()
    {
        StartCoroutine(GoingThePosAnim());
        _speed = 0;

        IEnumerator GoingThePosAnim()
        {
            _distanceTravelled %= _pathCreator.path.length;

            while (_distanceTravelled < _fillingPos)
            {
                _distanceTravelled += _animSpeed * Time.deltaTime;
                StartCoroutine(SetPosition());

                yield return null;
            }

            if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<MotorcycleColliderUpgrader>())
            {
                MotorcycleColliderUpgrader.ResetLidAnimator();
                MotorcycleColliderUpgrader.ChangeLidStatus(true);
            }
            else if (VehicleManager.Instance.GetActiveVehicle.GetComponentInChildren<TukTukColliderUpgrader>())
            {
                TukTukColliderUpgrader.ResetLidAnimator();
                TukTukColliderUpgrader.ChangeLidStatus(true);
            }

            _distanceTravelled = _fillingPos;
        }
    }

    private IEnumerator SpeedUp()
    {
        while (_speed <= MaxSpeed)
        {
            _speed += Time.deltaTime * _acceleration;
            SetAnimationSpeed(_speed);

            if (_spotLight.gameObject.activeSelf)
            {
                _spotLight.range += Time.deltaTime * 25;
            }

            yield return null;
        }
    }

    private IEnumerator SpeedDown()
    {
        while (_speed > 0)
        {
            _speed -= Time.deltaTime * _acceleration;
            SetAnimationSpeed(_speed);
            StartCoroutine(SetPosition());

            if (_spotLight.gameObject.activeSelf)
            {
                _spotLight.range -= Time.deltaTime * 25;
            }

            yield return null;
        }

        if (_spotLight.gameObject.activeSelf)
        {
            _spotLight.range = 2.5f;
        }

        SetAnimationSpeed(_speed);
    }

    private void ResetRagdoll()
    {
        Ragdoll.ChangeRagdollStatus(ragdollStatus: false);
        Ragdoll.ResetArmature();
        Ragdoll.ResetRagdollColliderPosition();
    }

    private void SetAnimationSpeed(float speed)
    {
        VehicleManager.Instance.GetActiveVehicle.SetAnimatorSpeed(speed);
    }

    public void ResetPlayerSpeed()
    {
        _speed = 0;
        SetAnimationSpeed(_speed);
    }

    public void DisableLayerIgnore()
    {
        Physics.IgnoreLayerCollision(14, 15, false); 
        Physics.IgnoreLayerCollision(8, 8, false);
        Physics.IgnoreLayerCollision(11, 8, false);
        Physics.IgnoreLayerCollision(13, 3, false);
        Physics.IgnoreLayerCollision(14, 3, false);
        Physics.IgnoreLayerCollision(11, 15, false);
        Physics.IgnoreLayerCollision(6, 15, false);
        Physics.IgnoreLayerCollision(13, 15, false); 
        Physics.IgnoreLayerCollision(13, 16, false); //customer - bike
    }

    private void ResetTheAccident()
    {
        DisableLayerIgnore();
        //_pedestrian.DidItInteract = false;
        AccidentController.Instance.ResetCameraEffect();
    }

    public void ChangeDistanceTravelled(float distanceTravelled)
    {
        _distanceTravelled = distanceTravelled;
    }
}
