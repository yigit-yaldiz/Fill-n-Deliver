using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillTheDelivery.Vehicle;
using FillTheDelivery.UI;
using Saving;
using UniqueGames.Saving;

public class MotorcycleColliderUpgrader : ColliderController, ISaveable
{
    [SerializeField] protected Transform _lid;    
    private static Animator _lidAnimator;

    public MotorcycleColliderUpgrader Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        _lidAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Start()
    {
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
            _backCollider.localScale = new Vector3(_firstScaleValue + (10f * level), _backCollider.localScale.y, _backCollider.localScale.z);

            _leftCollider.localPosition = new Vector3(_leftCollider.localPosition.x, _leftCollider.localPosition.y, -_startPos + (level * -_treshhold));
            _rightCollider.localPosition = new Vector3(_rightCollider.localPosition.x, _rightCollider.localPosition.y, _startPos + (level * _treshhold));
        }
        else if (UpgradeManager.UpgradeType.Depth == type)
        {
            _floorCollider.localScale = new Vector3(_floorCollider.localScale.x, _firstScaleValue + (30f * level), _floorCollider.localScale.z);
            _floorCollider.localPosition = new Vector3(level * _treshhold, _floorCollider.localPosition.y, _floorCollider.localPosition.z);
            
            _leftCollider.localScale = new Vector3(_leftCollider.localScale.x, _firstScaleValue + (30f * level), _leftCollider.localScale.z);
            _leftCollider.localPosition = new Vector3(level * _treshhold, _leftCollider.localPosition.y, _leftCollider.localPosition.z);
            
            _rightCollider.localScale = new Vector3(_rightCollider.localScale.x, _firstScaleValue + (30f * level), _rightCollider.localScale.z);
            _rightCollider.localPosition = new Vector3(level * _treshhold, _rightCollider.localPosition.y, _rightCollider.localPosition.z);

            //_frontCollider.localPosition = new Vector3(-_startPos + (level * -_treshhold), _frontCollider.localPosition.y, _frontCollider.localPosition.z);
            _backCollider.localPosition = new Vector3(_startPos + (level * (_treshhold * 2)), _backCollider.localPosition.y, _backCollider.localPosition.z);
        }
        else if (UpgradeManager.UpgradeType.Height == type)
        {
            //_floorCollider.localScale = new Vector3(_floorCollider.localScale.x, _floorCollider.localScale.y, _floorCollider.localScale.z);
            _leftCollider.localScale = new Vector3(_leftCollider.localScale.x, _leftCollider.localScale.y, _firstScaleValue + (100f * level));
            _rightCollider.localScale = new Vector3(_rightCollider.localScale.x, _rightCollider.localScale.y, _firstScaleValue + (100f * level));
            _frontCollider.localScale = new Vector3(_frontCollider.localScale.x, _frontCollider.localScale.y, _firstScaleValue + (100f * level));
            _backCollider.localScale = new Vector3(_backCollider.localScale.x, _backCollider.localScale.y, _firstScaleValue + (100f * level));
            _lid.transform.localPosition = new Vector3(_lid.localPosition.x, 1.4f + (level * _treshhold), _lid.localPosition.z); 
        }
    }

    [System.Serializable]
    public struct SaveData
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
    }
}
