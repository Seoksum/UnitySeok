/// 작성자: 백인성
/// 작성일: 2021-04-19
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.XRST;
using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.NET
{
    /// <summary>
    /// 연결시도중인 장비를 UI에 표시 하기 위한 클래스
    /// </summary>
    public class XRST_ConnectorToggle : XRST_Connector
    {
        /// <summary>
        /// 연결하기위해 선택을 표시
        /// </summary>
        public Toggle Checker
        {
            get
            {
                if (m_toggle == null)
                    m_toggle = GetComponent<Toggle>();

                return m_toggle;
            }
        }
        /// <summary>
        /// 초기화 되어있는 상태인지 확인용
        /// </summary>
        public bool IsInit { get => DeviceName.text != XRSTConst.kNoConnect; }

        /// <summary>
        /// 장비 종류를 표시할 <see cref="Icon"/>의 종류입니다.
        /// </summary>
        public Sprite[] icons;

        private Toggle m_toggle;

        private UnityAction<Toggle, string> onCheck;
        private UnityAction<Toggle, string> onUncheck;

        /// <summary>
        /// 최초생성시 초기화를 해주기 위한 함수
        /// </summary>
        /// <param name="dataType">장비 종류</param>
        /// <param name="deviceName">장비 명</param>
        /// <param name="ip">연결 IP</param>
        /// <param name="onCheck">선택시 이벤트</param>
        /// <param name="onUncheck">선택 해제시 이벤트</param>
        public void Init(FNI_Device.DeviceType dataType, string deviceName, string ip, UnityAction<Toggle, string> onCheck, UnityAction<Toggle, string> onUncheck)
        {
            SetContent(dataType, deviceName, ip);

            Checker.onValueChanged.AddListener(Checked); 

            this.onCheck = onCheck;
            this.onUncheck = onUncheck;
        }
        /// <summary>
        /// 초기화 이후 표시되는 내용을 변결할 때 사용
        /// </summary>
        /// <param name="dataType">장비 종류</param>
        /// <param name="deviceName">장비 명</param>
        /// <param name="ip">연결 IP</param>
        public override void SetContent(FNI_Device.DeviceType dataType, string deviceName, string ip)
        {
            base.SetContent(dataType, deviceName, ip);

            SetIcon(dataType);

            Checker.SetIsOnWithoutNotify(false);
        }
        /// <summary>
        /// Icon설정, 장비 종류
        /// </summary>
        /// <param name="num"></param>
        private void SetIcon(FNI_Device.DeviceType dataType)
        {
            Icon.sprite = icons[(int)dataType];
        }

        private void Checked(bool check)
        {
            if (check) onCheck?.Invoke(Checker, DeviceName.text);
            else       onUncheck?.Invoke(Checker, DeviceName.text);
        }
        private void OnDisable()
        {
            onUncheck?.Invoke(Checker, DeviceName.text);
        }
    }
}