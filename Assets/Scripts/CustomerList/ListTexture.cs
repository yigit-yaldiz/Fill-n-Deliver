using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniqueGames.Saving;

public class ListTexture : MonoBehaviour, ISaveable
{
    [SerializeField] Camera _mainCam;
    public List<TextMeshProUGUI> RequestCountsTexts = new List<TextMeshProUGUI>();

    public List<Animator> Animators => _animators;

    [SerializeField] private List<Animator> _animators;
    private bool _isItDeactivated;

    private void Awake()
    {
        foreach (var item in GetComponentsInChildren<Animator>())
        {
            _animators.Add(item);
        }

        if (_mainCam == null)
        {
            _mainCam = FindObjectOfType<Camera>();
        }

        foreach (var item in GetComponentsInChildren<TextMeshProUGUI>())
        {
            RequestCountsTexts.Add(item);
        }
    }

    private void Start()
    {
        if (_isItDeactivated)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        transform.LookAt(_mainCam.transform);
        transform.eulerAngles += new Vector3(0, 180, 0);
        transform.localRotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y, 0));
        
        //_lockedRot = new Vector3(0, transform.rotation.y, 0);
        //transform.rotation = Quaternion.Euler(_lockedRot);
    }

    [System.Serializable]
    struct SaveData
    {
        public bool isItDeactivated;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();
        saveData.isItDeactivated = _isItDeactivated;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData) state;
        _isItDeactivated = saveData.isItDeactivated;
    }
}
