using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillTheDelivery.Vehicle;
using UniqueGames.Saving;

public class BikeColliderUpgrader : ColliderController, ISaveable
{
    [System.Serializable]
    struct SaveData
    {
        public SerializableVector3 floorLocalPos;
        public SerializableVector3 leftLocalPos;
        public SerializableVector3 rightLocalPos;
        public SerializableVector3 frontLocalPos;
        public SerializableVector3 backLocalPos;

        public SerializableVector3 floorLocalScale;
        public SerializableVector3 leftLocalScale;
        public SerializableVector3 rightLocalScale;
        public SerializableVector3 frontLocalScale;
        public SerializableVector3 backLocalScale;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();

        saveData.floorLocalPos = new SerializableVector3(_floorCollider.localPosition);
        saveData.leftLocalPos = new SerializableVector3(_leftCollider.localPosition);
        saveData.rightLocalPos = new SerializableVector3(_rightCollider.localPosition);
        saveData.frontLocalPos = new SerializableVector3(_frontCollider.localPosition);
        saveData.backLocalPos = new SerializableVector3(_backCollider.localPosition);

        saveData.floorLocalScale = new SerializableVector3(_floorCollider.localScale);
        saveData.leftLocalScale = new SerializableVector3(_leftCollider.localScale);
        saveData.rightLocalScale = new SerializableVector3(_rightCollider.localScale);
        saveData.frontLocalScale = new SerializableVector3(_frontCollider.localScale);
        saveData.backLocalScale = new SerializableVector3(_backCollider.localScale);

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

        _floorCollider.localScale = saveData.floorLocalScale.ToVector();
        _leftCollider.localScale = saveData.leftLocalScale.ToVector();
        _rightCollider.localScale = saveData.rightLocalScale.ToVector();
        _frontCollider.localScale = saveData.frontLocalScale.ToVector();
        _backCollider.localScale = saveData.backLocalScale.ToVector();
    }
}
