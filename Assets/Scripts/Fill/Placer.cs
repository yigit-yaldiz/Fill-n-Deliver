using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Placer : MonoBehaviour
{
    public enum State
    {
        Placed,
        NonPlaced
    }

    [SerializeField] private GameObject _placed;
    [SerializeField] private Ghost _ghost;
    [SerializeField] private Rigidbody _placedRigidbody;
    private Vector3 _placementPos => PlacementRay.Instance.GetPlacePos;

    private Coroutine _startedCoroutine;
    private Coroutine _ghostCoroutine;

    private Case _myCase;
    private Animator _animator;
    private Product _product;
    AudioSource _popSound;

    #region Positions & Rotations
    private Vector3 _firstPos;
    private Vector3 _placedFirstPos;
    private Vector3 _ghostFirstPos;

    private Quaternion _firstRot;
    private Quaternion _placedFirstRot;
    private Quaternion _ghostFirstRot;
    
    private Vector3 _placedWorldPos;
    #endregion

    private State _state = State.NonPlaced;
    private float _roundDigidAmount = 1f;

    public State PlacerState => _state;
    public Case MyCase => _myCase;

    private void Awake()
    {
        _ghost = GetComponentInChildren<Ghost>(true);
        _animator = GetComponent<Animator>();
        #region Positions & Rotations
        _firstPos = transform.parent.localPosition;
        _firstRot = transform.parent.localRotation;
        _placedFirstPos = _placed.transform.localPosition;
        _placedWorldPos = _placed.transform.position;
        _placedFirstRot = _placed.transform.localRotation;
        _ghostFirstPos = _ghost.transform.localPosition;
        _ghostFirstRot = _ghost.transform.localRotation;
        #endregion
        _product = GetComponent<Product>();
        _popSound = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += ShakingAnimation;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= ShakingAnimation;
        GetComponent<Product>().IsItAtPlacingStage = false;
    }

    public void Starting()
    {
        GetComponent<Product>().IsItAtPlacingStage = true;
        _startedCoroutine = StartCoroutine(Started());
    }

    private IEnumerator Started()
    {
        bool checkingOccupation = false;

        _placed.transform.localPosition = _placedFirstPos;
        _placedWorldPos = _placed.transform.position;

        while (true)
        {
            yield return null;
            
            _ghost.gameObject.SetActive(true);
            transform.parent.position = _placementPos;
            transform.parent.localPosition = RoundThePoint(transform.parent.localPosition, _roundDigidAmount); //parent round ediyoruz
            _placed.transform.position = _placedWorldPos;

            if (!checkingOccupation)
            {
                checkingOccupation = true;
                _ghostCoroutine = StartCoroutine(GhostOccupation());
            }
        }

        IEnumerator GhostOccupation()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            //yield return new WaitForFixedUpdate();
            //yield return new WaitForFixedUpdate();

            while (true)
            {
                if (!_ghost.GetIsOccupied)
                {
                    PlaceIt();
                    StopCoroutine(_startedCoroutine);
                    _myCase.RemovePreviousPlacer();

                    break;
                }

                yield return null;
            }

            if (PlacementRay.Instance.FirstTime && PlacementRay.Instance.Animator != null)
            {
                PlacementRay.Instance.Animator.SetTrigger("placed");
            }
        }
    }

    private void PlaceIt()
    {
        _placed.transform.localPosition = Vector3.up * 3;
        _placedRigidbody.isKinematic = false;

        _popSound.Play();

        _ghost.gameObject.SetActive(false);
        transform.parent.SetParent(VehicleManager.Instance.GetActiveVehicle.Basket.transform); //basket olmasi gerek
        _state = State.Placed;
        ProductListManager.AddToPlacedProduct(_product);
        DefrostRotationOfProduct();
        //_itCanReversible = true;
    }

    public void ResetToCase()
    {
        //if (_state == State.Placed && GameManager.Instance.GameState != GameManager.GameStates.Reverse) //nonplaced or placed
        //{
        //    return;
        //}

        _placedRigidbody.isKinematic = true;
        _animator.SetTrigger("idleTrig");

        GetComponentInChildren<MeshRenderer>(true).enabled = true;

        _ghost.ResetIncount();

        #region transform and parent controls
        transform.parent.SetParent(_myCase.transform);
        transform.parent.localPosition = _firstPos;
        transform.parent.localRotation = _firstRot;
        _ghost.transform.localPosition = _ghostFirstPos;
        _ghost.transform.localRotation = _ghostFirstRot;
        _ghost.gameObject.SetActive(false);
        _placed.transform.localPosition = _placedFirstPos;
        _placed.transform.localRotation = _placedFirstRot;
        _placed.transform.position = _placedWorldPos;
        _placed.SetActive(true);
        #endregion

        if (_startedCoroutine != null)
        {
            StopCoroutine(_startedCoroutine);
        }

        if (_ghostCoroutine != null)
        {
            StopCoroutine(_ghostCoroutine);
        }

        ResetPlaced();
        _state = State.NonPlaced;
        
        _myCase.AddReturnedPlacer(gameObject);

        ProductListManager.RemoveFromPlacedProduct(_product);

        this.enabled = false;
    }

    public void SetMyCase(Case myCase)
    {
        _myCase = myCase;
    }

    public void SetTheState(State state)
    {
        _state = state;
    }

    private void ShakingAnimation()
    {
        if (GameManager.Instance.GameState == GameStates.Reverse)
        {
            _animator.ResetTrigger("idleTrig");
            _animator.SetTrigger("reverseTrig");
        }
        else
        {
            _animator.ResetTrigger("reverseTrig");
            _animator.SetTrigger("idleTrig");
        }
    }

    private Vector3 RoundThePoint(Vector3 position, float digitAmount)
    {
        Vector3 pos;
        pos.x = (float)Mathf.Round(position.x * digitAmount) / digitAmount;
        pos.y = (float)Mathf.Round(position.y * digitAmount) / digitAmount;
        pos.z = (float)Mathf.Round(position.z * digitAmount) / digitAmount;
        position = pos;

        return position;
    }

    public void ResetPlaced()
    {
        _placed.transform.localPosition = _placedFirstPos;
        _placedWorldPos = _placed.transform.position;
        FreezeRotationOfProduct();
    }

    public void FreezeRotationOfProduct()
    {
        _placedRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void DefrostRotationOfProduct()
    {
        _placedRigidbody.constraints = RigidbodyConstraints.None;
    }
}
