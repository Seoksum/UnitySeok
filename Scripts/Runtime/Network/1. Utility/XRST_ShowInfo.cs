/// 작성자: 백인성
/// 작성일: 2021-04-20
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;
using FNI.NET.Utility;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class XRST_ShowInfo : MonoBehaviour
    {
        public TextMeshProUGUI deviceType;
        public TextMeshProUGUI deviceName;
        public TextMeshProUGUI deviceIP;

        private void Start()
        {
            deviceType.text = FNI_Device.Type.ToString();
            deviceName.text = FNI_Device.GetDeviceName;
            deviceIP.text   = FNI_Device.IP.My.ToString();
        }
    }
}