using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FillTheDelivery.Vehicle;
using FillTheDelivery.UI;
using FillTheDelivery.Animation;
using Cinemachine;
using Saving;
using UniqueGames.Saving;

public class TukTukColliderUpgrader : ColliderController, ISaveable
{
    [SerializeField] private Transform _lid;
    [SerializeField] private CinemachineVirtualCamera[] _cameras;
    private static Animator _lidAnimator;
    private ContainerAnimationController _animationController;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    private float _rightColliderStartPos = 0.365f;

    private int _activeCameraIndex;


    #region Coroutines
    Coroutine _lidLerp;
    Coroutine _casesLerp;
    #endregion
    float _lidWidthBlendShape;
    float _lidHeightBlendShape;

    public static TukTukColliderUpgrader Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _lidAnimator = transform.GetChild(0).GetComponent<Animator>();
        _animationController = GetComponentInChildren<ContainerAnimationController>(true);
        _skinnedMeshRenderer = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
    }

    private void OnEnable()
    {
        UpgradeManager.OnLevelUp += CameraAdjustment;
        UpgradeManager.OnLevelUp += LerpAdjustment;
        UpgradeManager.OnLevelUp += WhenLevelUp;
    }
    private void OnDisable()
    {
        UpgradeManager.OnLevelUp -= CameraAdjustment;
        UpgradeManager.OnLevelUp -= LerpAdjustment;
        UpgradeManager.OnLevelUp -= WhenLevelUp;
    }

    private void Start()
    {
        foreach (var item in _cameras)
        {
            item.gameObject.SetActive(false);
        }

        _cameras[_activeCameraIndex].gameObject.SetActive(true);

        if (_skinnedMeshRenderer.GetBlendShapeWeight(0) == 0)
        {
            _skinnedMeshRenderer.SetBlendShapeWeight(0, _lidWidthBlendShape);
        }

        if (_skinnedMeshRenderer.GetBlendShapeWeight(2) == 0)
        {
            _skinnedMeshRenderer.SetBlendShapeWeight(2, _lidHeightBlendShape);
        }

        ChangeLidStatus(true);
    }

    public static void ChangeLidStatus(bool status)
    {
        if (status == true)
        {
            _lidAnimator.SetTrigger("openTrig");
        }
        else
        {
            _lidAnimator.SetTrigger("closeTrig");
        }
    }

    public static void ResetLidAnimator()
    {
        _lidAnimator.ResetTrigger("openTrig");
        _lidAnimator.ResetTrigger("closeTrig");
    }

    public override void WhenLevelUp(UpgradeManager.UpgradeType type, int level)
    {
        if (UpgradeManager.UpgradeType.Width == type)
        {
            _floorCollider.localScale = new Vector3(_firstScaleValue + (10f * level), _floorCollider.localScale.y, _floorCollider.localScale.z);
            _frontCollider.localScale = new Vector3(_firstScaleValue + (10f * level), _frontCollider.localScale.y, _frontCollider.localScale.z);
            _backCollider.localScale = new Vector3(_firstScaleValue / 100 + (0.07f * level), _backCollider.localScale.y, _backCollider.localScale.z);

            _leftCollider.localPosition = new Vector3(_leftCollider.localPosition.x, _leftCollider.localPosition.y, -_startPos + (level * -_treshhold));
            _rightCollider.localPosition = new Vector3(_rightCollider.localPosition.x, _rightCollider.localPosition.y, _rightColliderStartPos + (level * _treshhold));
        }
        else if (UpgradeManager.UpgradeType.Depth == type)
        {
            _floorCollider.localScale = new Vector3(_floorCollider.localScale.x, _firstScaleValue + (7.5f * level), _floorCollider.localScale.z);
            _floorCollider.localPosition = new Vector3(level * (_treshhold * 1.85f), _floorCollider.localPosition.y, _floorCollider.localPosition.z);

            _leftCollider.localScale = new Vector3(_leftCollider.localScale.x, _firstScaleValue + (7.5f * level), _leftCollider.localScale.z);
            _leftCollider.localPosition = new Vector3(0.09f + (level * _treshhold), _leftCollider.localPosition.y, _leftCollider.localPosition.z);

            _rightCollider.localScale = new Vector3(_rightCollider.localScale.x, _firstScaleValue + (7.5f * level), _rightCollider.localScale.z);
            _rightCollider.localPosition = new Vector3(0.09f + (level * _treshhold), _rightCollider.localPosition.y, _rightCollider.localPosition.z);

            //_frontCollider.localPosition = new Vector3(-_startPos + (level * -_treshhold), _frontCollider.localPosition.y, _frontCollider.localPosition.z);
            //_backCollider.localPosition = new Vector3(_startPos / 10000 + (level * _treshhold / 48), _backCollider.localPosition.y, _backCollider.localPosition.z);
            //_lid.transform.localPosition = new Vector3(0.563919f + (level * 0.12f), _lid.localPosition.y, _lid.localPosition.z);
        }
        else if (UpgradeManager.UpgradeType.Height == type)
        {
            //_floorCollider.localScale = new Vector3(_floorCollider.localScale.x, _floorCollider.localScale.y, _floorCollider.localScale.z);
            _leftCollider.localScale = new Vector3(_leftCollider.localScale.x, _leftCollider.localScale.y, _firstScaleValue + (10f * level));
            _rightCollider.localScale = new Vector3(_rightCollider.localScale.x, _rightCollider.localScale.y, _firstScaleValue + (10f * level));
            _frontCollider.localScale = new Vector3(_frontCollider.localScale.x, _frontCollider.localScale.y, _firstScaleValue + (10f * level));

            _backCollider.localScale = new Vector3(_backCollider.localScale.x, _backCollider.localScale.y, _firstScaleValue / 100 + (0.09f * level));
            _backCollider.localPosition = new Vector3(_backCollider.localPosition.x, _backCollider.localPosition.y, 0.00142f + (level * _treshhold / 250));
        }
    }

    void CameraAdjustment(UpgradeManager.UpgradeType type, int level)
    {
        int widthLevel = UpgradeManager.Instance.WidthLevel;
        int dephtLevel = UpgradeManager.Instance.DepthLevel;
        int heightLevel = UpgradeManager.Instance.HeightLevel;

        int[] Levels = new int[] { widthLevel, dephtLevel, heightLevel };

        int max = Levels.Max();

        foreach (var camera in _cameras)
        {
            camera.gameObject.SetActive(false);
        }

        _cameras[max - 1].gameObject.SetActive(true);
        _activeCameraIndex = max - 1;
    }

    void LerpAdjustment(UpgradeManager.UpgradeType type, int level)
    {
        //if (_lidLerp != null)
        //{
        //    StopCoroutine(_lidLerp);
        //}
        //if (_casesLerp != null)
        //{
        //    StopCoroutine(_casesLerp);
        //}

        _lidLerp = StartCoroutine(LidLerp(type, level));
        _casesLerp = StartCoroutine(CasesLerp(type, level));
    }

    IEnumerator LidLerp(UpgradeManager.UpgradeType type, int level)
    {
        float t = 0;
        while (t < 1)
        {
            t += 1 * Time.deltaTime;
            Vector3 lidLocalPos = _lid.transform.localPosition;

            if (type == UpgradeManager.UpgradeType.Depth)
            {
                lidLocalPos.x = Mathf.Lerp(_lid.transform.localPosition.x, 0.563919f + (level * 0.12f), _animationController.Curve.Evaluate(t) / 10f);
                _lid.localPosition = lidLocalPos;
            }
            else if (type == UpgradeManager.UpgradeType.Width)
            {
                _skinnedMeshRenderer.SetBlendShapeWeight(((int)type), 25 * (level - 1) + 25 * (_animationController.Curve.Evaluate(t)));
                _lidWidthBlendShape = 25 * (level - 1) + 25 * _animationController.Curve.Evaluate(t);
            }
            else if (type == UpgradeManager.UpgradeType.Height)
            {
                _skinnedMeshRenderer.SetBlendShapeWeight(((int)type), 25 * (level - 1) + 25 * (_animationController.Curve.Evaluate(t)));
                _lidHeightBlendShape = _lidWidthBlendShape = 25 * (level - 1) + 25 * _animationController.Curve.Evaluate(t);
            }

            yield return null;
        }

        SaveWrapper.Instance.Save();
    }

    IEnumerator CasesLerp(UpgradeManager.UpgradeType type, int level)
    {
        float t = 0;
        while (t < 1)
        {
            t += 1 * Time.deltaTime;
            Vector3 casesLocalPos = VehicleManager.Instance.Cases.transform.localPosition;
            if (type == UpgradeManager.UpgradeType.Depth)
            {
                casesLocalPos.z = Mathf.Lerp(VehicleManager.Instance.Cases.transform.localPosition.z, -2.661f + (level * -0.3f), _animationController.Curve.Evaluate(t) / 10f);
            }
            VehicleManager.Instance.Cases.transform.localPosition = casesLocalPos;
            yield return null;
        }
        SaveWrapper.Instance.Save();
    }

    [System.Serializable]
    struct SaveData
    {
        public SerializableVector3 floorLocalPos;
        public SerializableVector3 leftLocalPos;
        public SerializableVector3 rightLocalPos;
        public SerializableVector3 frontLocalPos;
        public SerializableVector3 backLocalPos;
        public SerializableVector3 lidLocalPos;

        public SerializableVector3 floorLocalScale;
        public SerializableVector3 leftLocalScale;
        public SerializableVector3 rightLocalScale;
        public SerializableVector3 frontLocalScale;
        public SerializableVector3 backLocalScale;
        public SerializableVector3 lidLocalScale;

        public float lidWidthBlendShape;
        public float lidHeightBlendShape;

        public int activeCameraIndex;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();

        saveData.floorLocalPos = new SerializableVector3(_floorCollider.localPosition);
        saveData.leftLocalPos = new SerializableVector3(_leftCollider.localPosition);
        saveData.rightLocalPos = new SerializableVector3(_rightCollider.localPosition);
        saveData.frontLocalPos = new SerializableVector3(_frontCollider.localPosition);
        saveData.backLocalPos = new SerializableVector3(_backCollider.localPosition);
        saveData.lidLocalPos = new SerializableVector3(_lid.localPosition);

        saveData.floorLocalScale = new SerializableVector3(_floorCollider.localScale);
        saveData.leftLocalScale = new SerializableVector3(_leftCollider.localScale);
        saveData.rightLocalScale = new SerializableVector3(_rightCollider.localScale);
        saveData.frontLocalScale = new SerializableVector3(_frontCollider.localScale);
        saveData.backLocalScale = new SerializableVector3(_backCollider.localScale);
        saveData.lidLocalScale = new SerializableVector3(_lid.localScale);

        saveData.lidWidthBlendShape = _lidWidthBlendShape;
        saveData.lidHeightBlendShape = _lidHeightBlendShape;

        saveData.activeCameraIndex = _activeCameraIndex;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;

        _floorCollider.localPosition = saveData.floorLocalPos.ToVector();
        _leftCollider.localPosition = saveData.leftLocalPos.ToVector();
        _rightCollider.localPosition = saveData.rightLocalPos.ToVector();
        _frontCollider.localPosition = saveData.frontLocalPos.ToVector();
        _backCollider.localPosition = saveData.backLocalPos.ToVector();
        _lid.localPosition = saveData.lidLocalPos.ToVector();

        _floorCollider.localScale = saveData.floorLocalScale.ToVector();
        _leftCollider.localScale = saveData.leftLocalScale.ToVector();
        _rightCollider.localScale = saveData.rightLocalScale.ToVector();
        _frontCollider.localScale = saveData.frontLocalScale.ToVector();
        _backCollider.localScale = saveData.backLocalScale.ToVector();
        _lid.localScale = saveData.lidLocalScale.ToVector();

        _lidWidthBlendShape = saveData.lidWidthBlendShape;
        _lidHeightBlendShape = saveData.lidHeightBlendShape;

        _activeCameraIndex = saveData.activeCameraIndex;
    }
}
