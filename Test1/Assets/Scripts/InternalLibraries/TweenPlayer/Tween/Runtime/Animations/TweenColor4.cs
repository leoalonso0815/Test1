using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;

#endif

namespace UnityExtensions.Tween
{
    public abstract class TweenColor4<TTarget> : TweenFromTo<Color4, TTarget> where TTarget : Object
    {
        private bool4 togC4 = new bool4(true);

        public override void Interpolate(float factor)
        {
            Color4 color4 = current;
            if (togC4.x)
            {
                color4.c1 = @from.c1 + (to.c1 - @from.c1) * factor;
            }
            if (togC4.y)
            {
                color4.c2 = @from.c2 + (to.c2 - @from.c2) * factor;
            }
            if (togC4.z)
            {
                color4.c3 = @from.c3 + (to.c3 - @from.c3) * factor;
            }
            if (togC4.w)
            {
                color4.c4 = @from.c4 + (to.c4 - @from.c4) * factor;
            }
            current = color4;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }
#endif
        }

#if UNITY_EDITOR

        public override void Reset(TweenPlayer player)
        {
            base.Reset(player);
            @from = Color4.White;
            current = @from;
        }
        
        protected virtual bool hdr => false;
        
        protected override void OnPropertiesGUI(TweenPlayer player, SerializedProperty property)
        {
            base.OnPropertiesGUI(player, property);
            this.DrawColorProperty(ref togC4.x, ref @from.c1, ref to.c1);
            this.DrawColorProperty(ref togC4.y, ref @from.c2, ref to.c2);
            this.DrawColorProperty(ref togC4.z, ref @from.c3, ref to.c3);
            this.DrawColorProperty(ref togC4.w, ref @from.c4, ref to.c4);
        }

        private void DrawColorProperty(ref bool b, ref Color fc, ref Color tc)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            var rect = EditorGUILayout.GetControlRect();
            var fromRect = new Rect(rect.x + labelWidth, rect.y, (rect.width - labelWidth - 8) / 2, rect.height);
            var toRect = new Rect(rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);
            rect.width = labelWidth - 8;
            b = EditorGUI.ToggleLeft(rect, "RGBA", b);
            using (DisabledScope.New(!b))
            {
                using (LabelWidthScope.New(12))
                {
                    
                    fc = EditorGUI.ColorField(fromRect, EditorGUIUtilities.TempContent("F"), fc, false, true, hdr);
                    tc = EditorGUI.ColorField(toRect, EditorGUIUtilities.TempContent("T"), tc, false, true, hdr);
                }
            }
        }
#endif
        
    }
}