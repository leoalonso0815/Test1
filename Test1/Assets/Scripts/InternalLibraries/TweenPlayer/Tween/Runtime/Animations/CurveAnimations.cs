using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityExtensions.Tween
{

    [Serializable]
    public abstract class TweenCurve<TTarget> : TweenFromTo<Vector2, TTarget> where TTarget : Object
    {
        [SerializeField]
        public CustomizableInterpolator curve;

        [SerializeField]
        public float force = 100f;

        [SerializeField]
        public float random = 0f;

        public TTarget fromTarget;

        public TTarget toTarget;

        protected abstract Vector2 GetValue(TTarget target);

        protected abstract void SetValue(TTarget target, Vector2 value);

        public override Vector2 current
        {
            get => GetValue(target);
            set => SetValue(target, value);
        }

        public Vector2 From => fromTarget ? GetValue(fromTarget) : from;

        public Vector2 To => toTarget ? GetValue(toTarget) : to;

        public override void Interpolate(float factor)
        {
            var randomForce = force * Mathf.Sign(random) - force * random;
            var line = new Vector2(To.x - From.x, To.y - From.y);
            var directionNormal = new Vector2(-line.y, line.x).normalized;
            var direction = directionNormal * randomForce;

            var interpolator = curve[factor];
            var disFactor = Mathf.Sin(interpolator * Mathf.PI);
            var dis = direction * interpolator;

            var t = current;
            t.x = (To.x - From.x) * factor + From.x;
            t.y = (To.y - From.y) * factor + From.y;
            current = t + dis;
            // Debug.Log($"{factor}     {interpolator}    {current}");
        }

#if UNITY_EDITOR
        protected override void OnPropertiesGUI(TweenPlayer player, SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(curve)));
            base.OnPropertiesGUI(player, property);
            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(fromTarget)));
            if (!fromTarget)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(from)));
            }
            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(toTarget)));
            if (!toTarget)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(to)));
            }
            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(force)));
            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(random)));
        }
#endif
        
    }
    
    [Serializable, TweenAnimation("CurveAnimation/RectTransform Curve", "RectTransform Curve")]
    public class TweenRectTransformCurve : TweenCurve<RectTransform>
    {
        protected override Vector2 GetValue(RectTransform target)
        {
            return target ? target.anchoredPosition : default;
        }

        protected override void SetValue(RectTransform target, Vector2 value)
        {
            if (target) target.anchoredPosition = value;
        }
    }

    [Serializable, TweenAnimation("CurveAnimation/Transform Curve", "Transform Curve")]
    public class TweenTransformCurve : TweenCurve<Transform>
    {
        protected override Vector2 GetValue(Transform target)
        {
            return target ? target.position : default;
        }

        protected override void SetValue(Transform target, Vector2 value)
        {
            if (target) target.position = new Vector3(value.x, value.y, target.position.z);
        }
    }
}