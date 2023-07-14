using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillTheDelivery.Animation;
using UniqueGames.Saving;
using FillTheDelivery.UI;

public class TukTukContainerController : ContainerAnimationController, ISaveable
{
    public override void WhenLevelUp(UpgradeManager.UpgradeType type, int level)
    {
        base.WhenLevelUp(type, level);
    }

    [System.Serializable]
    struct SaveData
    {
        public float widthBlendShapeLevel;
        public float depthBlendShapeLevel;
        public float heightBlendShapeLevel;
    }

    public object CaptureState()
    {
        _skinned = GetComponent<SkinnedMeshRenderer>();

        SaveData saveData = new SaveData();
        saveData.widthBlendShapeLevel = _skinned.GetBlendShapeWeight((int)UpgradeManager.UpgradeType.Width);
        saveData.depthBlendShapeLevel = _skinned.GetBlendShapeWeight((int)UpgradeManager.UpgradeType.Depth);
        saveData.heightBlendShapeLevel = _skinned.GetBlendShapeWeight((int)UpgradeManager.UpgradeType.Height);

        return saveData;
    }

    public void RestoreState(object state)
    {
        _skinned = GetComponent<SkinnedMeshRenderer>();

        SaveData saveData = (SaveData)state;

        _skinned.SetBlendShapeWeight((int)UpgradeManager.UpgradeType.Width, saveData.widthBlendShapeLevel);
        _skinned.SetBlendShapeWeight((int)UpgradeManager.UpgradeType.Depth, saveData.depthBlendShapeLevel);
        _skinned.SetBlendShapeWeight((int)UpgradeManager.UpgradeType.Height, saveData.heightBlendShapeLevel);
    }
}
