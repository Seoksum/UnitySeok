/// 작성자: 백인성
/// 작성일: 2020-11-26
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.Common.Utils.Navigation
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Orbit Data", menuName = "FNI/Orbit Data")]
    public class OrbitData : ScriptableObject
    {
        [Header("Camera")]
        public float baseHeight = 0;
        public MinMax clippingPlanes = new MinMax(0.1f, 100);
        public int depth = -1;
        public bool isOrthographic;
        public MinMax limitSizeRange;
        public bool addAudioListener = false;

        [Header("Option")]
        public Vector2 moveDir = new Vector2(1, 1);
        /// <summary>
        /// [x] = Move, [y] = Drag, [z] = Zoom
        /// </summary>
        [Header("[x] = 상하, [y] = 좌우, [z] = Zoom")]
        public Vector3 resetPercent = new Vector3(0.7f, 0.5f, 0.5f);
        /// <summary>
        /// [x] = Move, [y] = Drag, [z] = Zoom
        /// </summary>
        [Header("[x] = Move, [y] = Rotation, [z] = Zoom")]
        public Vector3 inputPower = new Vector3(0.1f, 0.25f, 0.5f);
        /// <summary>
        /// [x] = Move, [y] = Drag, [z] = Zoom
        /// </summary>
        [Header("[1~20][x] = Move, [y] = Rotation, [z] = Zoom")]
        public Vector3 lerpPower = new Vector3(5, 5, 5);

        [Header("Limit")]
        public bool useLimitX = true;
        public bool useLimitY = true;
        public bool useLimitZ = true;

        [Space]
        public MinMax limitRotX = new MinMax(10, 80);
        public MinMax limitRotY = new MinMax(-80, 80);
        public MinMax limitZoom = new MinMax(-1f, 0);
        public MinMax limitMoveArea = new MinMax(1, 0.45f);
        public MinMax limitAxisScale = new MinMax(0.001f, 0.0001f);
    }

    /// <summary>
    /// 최소값 최대값, 라이트 버전임
    /// </summary>
    [System.Serializable]
    [SerializeField]
    public struct MinMax
    {
        /// <summary>
        /// percent가 적용된 최소값을 가져오고 설정은 m_min값만 설정
        /// </summary>
        public float Min { get { return m_min; } }
        /// <summary>
        /// percent가 적용된 최대값을 가져오고 설정은 m_max값만 설정
        /// </summary>
        public float Max { get { return m_max; } }

        [SerializeField]
        private float m_min;
        [SerializeField]
        private float m_max;

        /// <summary>
        /// 최소 값과 최대값을 설정합니다.
        /// Percent의 값을 기준으로 값을 반환합니다.
        /// </summary>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        public MinMax(float min = 0, float max = 0)
        {
            m_min = min;
            m_max = max;
        }
        /// <summary>
        /// value가 Min Max사이 어디 쯤 있는지 %로 계산함
        /// </summary>
        /// <param name="value">확인할 값</param>
        /// <returns></returns>
        public float GetPercent(float value)
        {
            float percent = (value - Min) / (Max - Min);

            return (float)Math.Round(percent, 4);
        }
        /// <summary>
        /// 0~1의 값을 입력하면 해당되는 값을 반환합니다.
        /// </summary>
        /// <param name="percent">0~1의 값</param>
        /// <returns></returns>
        public float PercentToValue(float percent)
        {
            return Mathf.Lerp(Min, Max, percent);
        }
        /// <summary>
        /// 입력되는 값을 min, max에 맞춰잘라낸 값을 반환합니다.
        /// </summary>
        /// <param name="value">자를 값</param>
        /// <returns></returns>
        public float Clamp(float value)
        {
            if (value < m_min)
                return m_min;
            else if (m_max < value)
                return m_max;
            else
                return value;
        }
        /// <summary>
        /// 입력되는 값을 min에 맞춰잘라낸 값을 반환합니다.
        /// </summary>
        /// <param name="value">자를 값</param>
        /// <returns></returns>
        public float ClampMin(float value)
        {
            if (value < -m_min)
                return -m_min;
            else if (m_min < value)
                return m_min;
            else
                return value;
        }
        /// <summary>
        /// 입력되는 값을 max에 맞춰잘라낸 값을 반환합니다.
        /// </summary>
        /// <param name="value">자를 값</param>
        /// <returns></returns>
        public float ClampMax(float value)
        {
            if (value < -m_max)
                return -m_max;
            else if (m_max < value)
                return m_max;
            else
                return value;
        }
    }
}