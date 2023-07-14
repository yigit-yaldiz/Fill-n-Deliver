using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FillTheDelivery.UI;
using UniqueGames.Saving;
using Saving;
using System;

namespace FillTheDelivery.Animation
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public abstract class ContainerAnimationController : MonoBehaviour
    {
        public AnimationCurve Curve => _curve;

        [SerializeField] protected AnimationCurve _curve;
        protected SkinnedMeshRenderer _skinned;

        private Coroutine _coroutine;

        private UpgradeManager.UpgradeType _lastType;

        internal void WhenLevelUp()
        {
            throw new NotImplementedException();
        }

        private void Awake()
        {
            _skinned = GetComponent<SkinnedMeshRenderer>();
        }

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
            if (_coroutine != null && _lastType == type)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(Animate());

            _lastType = type;

            IEnumerator Animate()
            {
                float t = 0;
                while (t < 1)
                {
                    yield return null;
                    t += 1 * Time.deltaTime;
                    _skinned.SetBlendShapeWeight((int)type, 25 * (level-1) + 25 * (_curve.Evaluate(t)));
                }

                //SaveWrapper.Instance.Save();
            }
        }
    }

    
}