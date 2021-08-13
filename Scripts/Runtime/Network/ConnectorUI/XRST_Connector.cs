/// 작성자: 백인성
/// 작성일: 2021-04-20
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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.NET
{
    /// <summary>
    /// 연결 장비 표시 기본 UI 컨트롤러
    /// </summary>
    public class XRST_Connector : MonoBehaviour
    {
        /// <summary>
        /// 사용중인지 확인
        /// </summary>
        public bool IsUse { get => IPText.text != XRSTConst.kNoIP; }
        /// <summary>
        /// 장비 이름을 적을 <see cref="TextMeshProUGUI"/>입니다.
        /// </summary>
        public TextMeshProUGUI DeviceName
        {
            get
            {
                if (m_textDevicName == null)
                    m_textDevicName = transform.Find("DevicesName").GetComponent<TextMeshProUGUI>();

                return m_textDevicName;
            }
        }
        /// <summary>
        /// IP를 적을 <see cref="TextMeshProUGUI"/>입니다.
        /// </summary>
        public TextMeshProUGUI IPText
        {
            get
            {
                if (m_textIP == null)
                    m_textIP = transform.Find("IP").GetComponent<TextMeshProUGUI>();

                return m_textIP;
            }
        }
        /// <summary>
        /// 장비 종류를 표시할 <see cref="Image"/>입니다.
        /// </summary>
        public Image Icon
        {
            get
            {
                if (m_icon == null)
                    m_icon = transform.Find("Icon").GetComponent<Image>();

                return m_icon;
            }
        }

        private TextMeshProUGUI m_textDevicName;
        private TextMeshProUGUI m_textIP;
        private Image m_icon;

        /// <summary>
        /// UI 내용을 설정합니다.
        /// </summary>
        /// <param name="dataType">장비 종류</param>
        /// <param name="deviceName">장비명</param>
        /// <param name="ip">접속 IP</param>
        public virtual void SetContent(FNI_Device.DeviceType dataType, string deviceName, string ip)
        {
            DeviceName.text = deviceName;
            IPText.text = ip;

            SetActive(true);
        }
        /// <summary>
        /// 오브젝트의 활성화 및 비활성화 설정
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}