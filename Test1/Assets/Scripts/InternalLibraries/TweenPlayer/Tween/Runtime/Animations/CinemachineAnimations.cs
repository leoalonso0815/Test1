using System;
using Unity.Cinemachine;

namespace UnityExtensions.Tween
{
    [Serializable, TweenAnimation("Cinemachine/OrthographicSize", "CinemachineVirtualCamera OrthographicSize")]
    public class CinemachineAnimations : TweenFloat<CinemachineVirtualCamera>
    {
        public override float current
        {
            get => target ? target.m_Lens.OrthographicSize : default;
            set
            {
                if (target && target.m_Lens.OrthographicSize > 0)
                {
                    target.m_Lens.OrthographicSize = value;
                }
            }
        }
    }
}