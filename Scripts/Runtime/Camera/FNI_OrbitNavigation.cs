/// 작성자: 백인성
/// 작성일: 2020-11-24
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using System;
using System.Collections;
using System.Collections.Generic;

using FNI;
using FNI.Common.Utils;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.Common.Utils.Navigation
{
    public class FNI_OrbitNavigation : MonoBehaviour
    {
        /// <summary>
        /// 마우스 버튼의 상태입니다.
        /// </summary>
        public enum ClickState
        {
            Left,
            Right,
            Middle,
        }
        /// <summary>
        /// 좌우 기울기입니다.
        /// </summary>
        private Transform Y
        {
            get
            {
                if (m_y == null && checkY == false)
                {
                    m_y = MakeTransform<Transform>(null, "Pivot CameraOrbit");
                    checkY = true;
                }

                return m_y;
            }
        }
        /// <summary>
        /// 앞뒤 기울기 입니다.
        /// </summary>
        private Transform X
        {
            get
            {
                if (m_x == null && checkX == false)
                {
                    Transform find = Y.Find("Pivot X");

                    if (find != null)
                        m_x = find;
                    else
                        m_x = MakeTransform<Transform>(Y, "Pivot X");

                    checkX = true;
                }

                return m_x;
            }
        }
        /// <summary>
        /// 줌 인 아웃을 위함입니다.
        /// </summary>
        private Camera Z
        {
            get
            {
                if (m_camera == null && checkCam == false)
                {
                    m_camera = GetComponent<Camera>();

                    if (m_camera == null)
                    {
                        Transform find = X.Find("Camera Z");
                        if (find != null)
                            m_camera = find.GetComponent<Camera>();
                        else
                            m_camera = MakeTransform<Camera>(X, "Camera Z");
                    }
                    else
                    {
                        m_camera.transform.SetParent(X, false);
                        m_camera.transform.localEulerAngles = Vector3.zero;
                    }

                    m_camera.nearClipPlane = data.clippingPlanes.Min;
                    m_camera.farClipPlane = data.clippingPlanes.Max;
                    m_camera.depth = data.depth;

                    if (data.addAudioListener)
                    {
                        AudioListener check = m_camera.GetComponent<AudioListener>();
                        if (check == null)
                            m_camera.gameObject.AddComponent<AudioListener>();
                    }
                    checkCam = true;
                }
                return m_camera;
            }
        }
        /// <summary>
        /// 피봇을 나타냅니다.
        /// </summary>
        private Transform Axis
        {
            get
            {
                if (m_axis == null && checkAxis == false)
                {
                    Transform find = Y.Find("Axis");
                    if (find != null)
                        m_axis = find;
                    else
                    {
                        if (axisDummy != null)
                        {
                            m_axis = Instantiate(axisDummy, Y, false);
                            m_axis.name = axisDummy.name;
                        }
                    }
                    checkAxis = true;
                }

                return m_axis;
            }
        }
        /// <summary>
        /// 카메라 거리에 대한 움직임의 가속값입니다. 멀면 더빠르게 가까우면 더 느리게 움직입니다.
        /// </summary>
        private float Accelator
        {
            get => Mathf.Lerp(1, 0.1f, data.limitZoom.GetPercent(m_rotvalue.z));
        }

        private Transform m_axis;

        private Transform m_y;
        private Transform m_x;
        private Camera m_camera;
        private Vector2 m_oldMousePosition;

        private bool checkAxis = false;
        private bool checkX = false;
        private bool checkY = false;
        private bool checkCam = false;

        private Vector3 m_posValue;
        private Vector3 m_rotvalue;
        
        [Header("[Mouse Input]")]
        public ClickState inputRotate = ClickState.Left;
        public ClickState inputMove = ClickState.Middle;
        public ClickState inputReset = ClickState.Right;

        [Header("[Data]")]
        public OrbitData data;
        /// <summary>
        /// Prefabs를 넣을 것
        /// </summary>
        [Header("[Aixs] - Can Empty")]
        public Transform axisDummy;
        [Header("[Controll Guide] - Can Empty")]
        public TextMeshProUGUI guide;

        private void Start()
        {
            Init();
        }
        public void Init()
        {
            Z.orthographic = data.isOrthographic;
            m_rotvalue = new Vector3(data.limitRotX.PercentToValue(data.resetPercent.x),
                                     data.limitRotY.PercentToValue(data.resetPercent.y), 
                                     data.limitZoom.PercentToValue(data.resetPercent.z));
            m_posValue = new Vector3(0, data.baseHeight, 0);
        }
        public void ChangeData(OrbitData data)
        {
            this.data = data;
            Init();
        }
        /// <summary>
        /// 화면을 클릭하면 호출합니다.
        /// EventTrigger를 사용시 같은 이름의 이벤트를 사용합니다.
        /// </summary>
        /// <param name="data">EventTrigger Event</param>
        public void Click(BaseEventData data)
        {
            if (data.currentInputModule.input.GetMouseButtonUp((int)inputReset))
                Init();
        }
        /// <summary>
        /// 드레그를 시작할 때 호출합니다.
        /// EventTrigger를 사용시 같은 이름의 이벤트를 사용합니다.
        /// </summary>
        /// <param name="data">EventTrigger Event</param>
        public void BeginDrag(BaseEventData data)
        {
            m_oldMousePosition = Input.mousePosition;
        }
        /// <summary>
        /// 드레그 중일 때 호출되어야 합니다.
        /// EventTrigger를 사용시 같은 이름의 이벤트를 사용합니다.
        /// </summary>
        /// <param name="data">EventTrigger Event</param>
        public void Drag(BaseEventData data)
        {
            Vector2 delta = data.currentInputModule.input.mousePosition - m_oldMousePosition;
            m_oldMousePosition = data.currentInputModule.input.mousePosition;

            if (data.currentInputModule.input.GetMouseButton((int)inputRotate))
                Rotating(delta);
            else if (data.currentInputModule.input.GetMouseButton((int)inputMove))
                Moving(delta);
        }

        /// <summary>
        /// 줌, 
        /// 마우스 스크롤을 때 호출되어야 합니다.
        /// EventTrigger를 사용시 같은 이름의 이벤트를 사용합니다.
        /// </summary>
        /// <param name="data">EventTrigger Event</param>
        public void Scroll(BaseEventData data)
        {
            m_rotvalue -= new Vector3(0, 0, data.currentInputModule.input.mouseScrollDelta.y * this.data.inputPower.z);
        }
        /// <summary>
        /// 회전을 담당합니다.
        /// </summary>
        /// <param name="rot">회전 값</param>
        public void Rotating(Vector2 rot)
        {
            m_rotvalue += new Vector3(-rot.y, rot.x, 0) * data.inputPower.y;
        }
        /// <summary>
        /// 이동을 담당합니다.
        /// </summary>
        /// <param name="rot">이동 값</param>
        public void Moving(Vector2 rot)
        {
            m_posValue += ((Y.forward * -rot.y * data.moveDir.y) + (Y.right * -rot.x * data.moveDir.x)) * data.inputPower.x * Accelator;
        }

        private void LateUpdate()
        {
            //한계값 지정
            m_rotvalue = new Vector3(data.useLimitX ? data.limitRotX.Clamp(m_rotvalue.x) : m_rotvalue.x,
                                     data.useLimitY ? data.limitRotY.Clamp(m_rotvalue.y) : m_rotvalue.y,
                                     data.useLimitZ ? data.limitZoom.Clamp(m_rotvalue.z) : m_rotvalue.z);
            m_posValue = new Vector3(data.limitMoveArea.ClampMin(m_posValue.x),
                                     data.baseHeight,
                                     data.limitMoveArea.ClampMax(m_posValue.z));

            //Lerp Move, Rot
            float deltaTime = Time.deltaTime;
            float zoomPercent = data.limitZoom.GetPercent(m_rotvalue.z);

            Y.localPosition = Vector3.Lerp(Y.localPosition, m_posValue, deltaTime * data.lerpPower.x);

            Y.localRotation = Quaternion.Lerp(Y.localRotation, Quaternion.Euler(new Vector3(0, m_rotvalue.y, 0)), deltaTime * data.lerpPower.y);
            X.localRotation = Quaternion.Lerp(X.localRotation, Quaternion.Euler(new Vector3(m_rotvalue.x, 0, 0)), deltaTime * data.lerpPower.y);

            if (data.isOrthographic)
            {
                Z.orthographicSize = Mathf.Lerp(Z.orthographicSize, data.limitSizeRange.PercentToValue(zoomPercent), deltaTime * data.lerpPower.z);
                Z.transform.localPosition = new Vector3(0, 0, -data.limitZoom.PercentToValue(data.resetPercent.z));
            }
            else
                Z.transform.localPosition = Vector3.Lerp(Z.transform.localPosition, new Vector3(0, 0, -m_rotvalue.z), deltaTime * data.lerpPower.z);

            //Axis Setting
            if (Axis)
            {
                Axis.localRotation = Quaternion.Inverse(Y.localRotation);
                Axis.localScale = Vector3.Lerp(Vector3.one * data.limitAxisScale.Min, Vector3.one * data.limitAxisScale.Max, data.limitZoom.GetPercent(-Z.transform.localPosition.z));
            }

            if (guide != null)
            {
                guide.text = $"Mouse Input\n\n" +
                             $"Move:   {inputMove}\n" +
                             $"Rotate: {inputRotate}\n" +
                             $"Reset:  {inputReset}";
            }
        }

        /// <summary>
        /// 게임 오브젝트를 생성합니다.
        /// </summary>
        /// <typeparam name="T">생성할 타입</typeparam>
        /// <param name="parent">하위로 들어갈 부모</param>
        /// <param name="makeName">생성시 이름</param>
        /// <returns></returns>
        private T MakeTransform<T>(Transform parent, string makeName) where T : Component
        {
            GameObject go = new GameObject(makeName);
            T make = go.GetComponent<T>();
            if (make == null)
                make = go.AddComponent<T>();
            make.transform.SetParent(parent, false);

            return make;
        }
    }
}