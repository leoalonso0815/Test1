#pragma warning disable CS0414

using System;
using UnityEngine;

namespace UnityExtensions.Tween
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TweenAnimationAttribute : Attribute
    {
        public readonly string menu;
        public readonly string name;

        public TweenAnimationAttribute(string menu, string name)
        {
            this.menu = menu;
            this.name = name;
        }
    }

    [Serializable]
    public abstract partial class TweenAnimation
    {
        [HideInInspector]
        public bool enabled = true;

        [SerializeField][HideInInspector]
        float _minNormalizedTime = 0f;

        [SerializeField][HideInInspector]
        float _maxNormalizedTime = 1f;

        [SerializeField][HideInInspector]
        bool _holdBeforeStart = false;

        [SerializeField][HideInInspector]
        bool _holdAfterEnd = false;

        [SerializeField]
        public CustomizableInterpolator _interpolator = default;

        [SerializeField][HideInInspector]
        bool _foldout = true;   // Editor Only

        [SerializeField][HideInInspector]
        string _comment = null; // Editor Only

        public float minNormalizedTime
        {
            get { return _minNormalizedTime; }
            set
            {
                _minNormalizedTime = Mathf.Clamp01(value);
                _maxNormalizedTime = Mathf.Clamp(_maxNormalizedTime, _minNormalizedTime, 1f);
            }
        }


        public float maxNormalizedTime
        {
            get { return _maxNormalizedTime; }
            set
            {
                _maxNormalizedTime = Mathf.Clamp01(value);
                _minNormalizedTime = Mathf.Clamp(_minNormalizedTime, 0f, _maxNormalizedTime);
            }
        }


        public bool holdBeforeStart
        {
            get => _holdBeforeStart;
            set => _holdBeforeStart = value;
        }


        public bool holdAfterEnd
        {
            get => _holdAfterEnd;
            set => _holdAfterEnd = value;
        }

        [NonSerialized]
        public bool isFinish;

        public void Sample(float normalizedTime, PlayDirection direction)
        {
            holdBeforeStart = false;
            holdAfterEnd = false;
            if (isFinish)
            {
                return;
            }
            if (normalizedTime < _minNormalizedTime)
            {
                if (direction == PlayDirection.Back)
                {
                    normalizedTime = 0f;
                    isFinish = true;
                }
                else
                {
                    return;
                }
            }
            else if (normalizedTime > _maxNormalizedTime)
            {
                if (direction == PlayDirection.Forward)
                {
                    normalizedTime = 1f;
                    isFinish = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (_maxNormalizedTime == _minNormalizedTime) normalizedTime = 1f;
                else normalizedTime = (normalizedTime - _minNormalizedTime) / (_maxNormalizedTime - _minNormalizedTime);
            }

            Interpolate(_interpolator[normalizedTime]);
        }


        public abstract void Interpolate(float factor);

    } // class TweenAnimation

} // UnityExtensions.Tween
