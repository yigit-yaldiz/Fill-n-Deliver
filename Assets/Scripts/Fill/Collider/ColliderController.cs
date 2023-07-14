using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillTheDelivery.UI;
using Saving;
using UniqueGames.Saving;

namespace FillTheDelivery.Vehicle
{
    public abstract class ColliderController : MonoBehaviour
    {
        [SerializeField] protected float _firstScaleValue; //4
        [SerializeField] protected float _startPos; //0.0122
        [SerializeField] protected float _treshhold; //0.0015
                         
        [SerializeField] protected Transform _floorCollider;
        [SerializeField] protected Transform _leftCollider;
        [SerializeField] protected Transform _rightCollider;
        [SerializeField] protected Transform _frontCollider;
        [SerializeField] protected Transform _backCollider;

        private void OnEnable()
        {
            UpgradeManager.OnLevelUp += WhenLevelUp;
        }
        private void OnDisable()
        {
            UpgradeManager.OnLevelUp -= WhenLevelUp;
        }

        public virtual void WhenLevelUp(UpgradeManager.UpgradeType type, int level)
        {
            if (UpgradeManager.UpgradeType.Width == type)
            {
                _floorCollider.localScale = new Vector3(_firstScaleValue + (0.4f * level), _floorCollider.localScale.y, _floorCollider.localScale.z);
                _frontCollider.localScale = new Vector3(_firstScaleValue + (0.5f * level), _frontCollider.localScale.y, _frontCollider.localScale.z);
                _backCollider.localScale = new Vector3(_firstScaleValue + (0.5f * level), _backCollider.localScale.y, _backCollider.localScale.z);

                _leftCollider.localPosition = new Vector3(_leftCollider.localPosition.x, _startPos + (level * _treshhold), _leftCollider.localPosition.z);
                _rightCollider.localPosition = new Vector3(_rightCollider.localPosition.x, -_startPos + (level * -_treshhold), _rightCollider.localPosition.z);
            }
            else if (UpgradeManager.UpgradeType.Depth == type)
            {
                _floorCollider.localScale = new Vector3(_floorCollider.localScale.x, _firstScaleValue + (0.5f * level), _floorCollider.localScale.z);
                _leftCollider.localScale = new Vector3(_leftCollider.localScale.x, _firstScaleValue + (0.5f * level), _leftCollider.localScale.z);
                _rightCollider.localScale = new Vector3(_rightCollider.localScale.x, _firstScaleValue + (0.5f * level), _rightCollider.localScale.z);
                
               _frontCollider.localPosition = new Vector3(-_startPos + (level * -_treshhold), _frontCollider.localPosition.y, _frontCollider.localPosition.z);
               _backCollider.localPosition = new Vector3(_startPos + (level * _treshhold), _backCollider.localPosition.y, _backCollider.localPosition.z);
            }
            else if (UpgradeManager.UpgradeType.Height == type)
            {
                //_floorCollider.localScale = new Vector3(_floorCollider.localScale.x, _floorCollider.localScale.y, _floorCollider.localScale.z);
                _leftCollider.localScale = new Vector3(_leftCollider.localScale.x, _leftCollider.localScale.y, _firstScaleValue + (0.5f * level));
                _rightCollider.localScale = new Vector3(_rightCollider.localScale.x, _rightCollider.localScale.y, _firstScaleValue + (0.5f * level));
                _frontCollider.localScale = new Vector3(_frontCollider.localScale.x, _frontCollider.localScale.y, _firstScaleValue + (0.5f * level));
                _backCollider.localScale = new Vector3(_backCollider.localScale.x, _backCollider.localScale.y, _firstScaleValue + (0.5f * level));
            }

            //SaveWrapper.Instance.Save();
        }
    }                      
}
                           
                           