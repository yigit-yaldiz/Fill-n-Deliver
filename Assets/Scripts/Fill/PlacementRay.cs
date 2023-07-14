using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;
using Saving;

public class PlacementRay : MonoBehaviour, ISaveable
{
    #region Layer Masks
    [SerializeField] private LayerMask _floorLayerMask;
    [SerializeField] private LayerMask _caseLayerMask;
    [SerializeField] private LayerMask _placedMask;
    #endregion
    
    public Case _case;

    private Camera _cam;
    private Ray _ray;
    private RaycastHit _hit;
    private AudioSource _getBackSound;

    private Vector3 _placementPosition;
    public Vector3 GetPlacePos => _placementPosition;
    public Placer ActivePlacer { get; private set; }

    private Vector3 _placingOffset = 0.1f * Vector3.forward;

    #region Tutorial Variables
    private bool _isThisFirstTime = true;
    [SerializeField] Animator _animator;
    public bool FirstTime => _isThisFirstTime;
    public Animator Animator => _animator;
    #endregion

    #region Singleton Instance
    public static PlacementRay Instance { get; private set; }
    #endregion

    private void Awake()
    {
        #region Singleton Pattern
        Instance = this;
        #endregion

        _cam = GetComponent<Camera>();
        _getBackSound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (_isThisFirstTime && _animator != null)
        {
            _animator.SetTrigger("firstTime");
        }
        else if(_animator != null)
        {
            _animator.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            _ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (GameManager.Instance.GameState == GameStates.Placement)
            {
                SelectPlacementPlace();
            }
            else if (GameManager.Instance.GameState == GameStates.Reverse)
            {
                SelectAPlaced();
                
                if (_isThisFirstTime && _animator != null)
                {
                    _animator.SetTrigger("reverseCompleted");
                }
            }
        }
        else
        {
            ResetActivePlacerToCase();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (GameManager.Instance.GameState == GameStates.Placement)
            {
                SelectACase();
                
                if (_isThisFirstTime && _animator != null)
                {
                    _animator.SetTrigger("caseSelected");
                }
            }
        }
    }

    public void SelectACase()
    {
        if (Physics.Raycast(_ray, out _hit, 100, _caseLayerMask))
        {
            Case selection = _hit.collider.GetComponent<Case>();

            if (_case == selection)
            {
                return;
            }

            selection.SetTrigger();
            _case = selection;
        }
    }

    public void ResetActivePlacerToCase()
    {
        if (ActivePlacer == null || ActivePlacer.PlacerState == Placer.State.Placed)
        {
            return;
        }
            
        ActivePlacer.ResetToCase();
    }

    public void SelectAPlaced()
    {
        if (Physics.Raycast(_ray, out _hit, 100, _placedMask))
        {
            Placer placer = _hit.transform.GetComponentInParent<Placer>();

            placer.gameObject.GetComponentInChildren<Ghost>(true).enabled = false;
            
            if (GameManager.Instance.GameState == GameStates.Reverse && placer.PlacerState == Placer.State.Placed)
            {
                _getBackSound.Play();
                placer.ResetToCase();
                Reverse.HasItDoneReversed = true;
            }
        }
    }

    private void SelectPlacementPlace()
    {
        if (Physics.Raycast(_ray, out _hit, 100, _floorLayerMask))
        {
            _placementPosition = _hit.point /*+ _placingOffset*/;

            if (_case == null)
            {
                Debug.Log("Choose a case");
                return;
            }

            GameObject nextPlacer = _case.ChooseTheNextPlacer();

            if (nextPlacer == null || !nextPlacer.transform.parent.gameObject.activeSelf)
            {
                _case.RemoveSpecificPlacer(nextPlacer);
                return;
            }

            Placer placer = nextPlacer.GetComponent<Placer>();

            if (!placer.enabled)
            {
                placer.enabled = true;
                placer.Starting();
                ActivePlacer = placer;
            }
        }
    }

    public void TutorialFinished()
    {
        _animator.gameObject.SetActive(false);
        _isThisFirstTime = false;
        SaveWrapper.Instance.Save();
    }

    [System.Serializable]
    struct SaveData
    {
        public bool isThisFirstTime;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();
        saveData.isThisFirstTime = _isThisFirstTime;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;
        _isThisFirstTime = saveData.isThisFirstTime;
    }
}
