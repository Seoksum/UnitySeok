/// 작성자: 백인성
/// 작성일: 2021-05-20
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.NET;
using FNI.NET.Utility;
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
    public class FNI_NetBase : MonoBehaviour
    {
        protected NetworkDataType dataType;

        protected virtual void Start()
        {
            switch (dataType)
            {
                case NetworkDataType.U_DeviceConnect:
                    FNI_NETProcess.onReceiveData_DeviceConnect.AddListener(Receive);
                    break;
                case NetworkDataType.U_SyncObject:
                    FNI_NETProcess.onReceiveData_SyncObject.AddListener(Receive);
                    break;
                case NetworkDataType.U_SyncCharactor:
                    FNI_NETProcess.onReceiveData_SyncCharactor.AddListener(Receive);
                    break;
                case NetworkDataType.T_DeviceFeedback:
                    FNI_NETProcess.onReceiveData_DeviceFeedback.AddListener(Receive);
                    break;
                case NetworkDataType.T_Mission:
                    FNI_NETProcess.onReceiveData_Mission.AddListener(Receive);
                    break;
                case NetworkDataType.T_Student:
                    FNI_NETProcess.onReceiveData_Student.AddListener(Receive);
                    break;
                default:
                    throw new XRSTExeption("XRST_NetBase", "Start", $"[{dataType}]은 잘못된 타입입니다.");
            }
        }
#if SERVER
        protected virtual void Send(byte[] _data, FNI_Device.DeviceType _device = FNI_Device.DeviceType.Hololens)
        {
            IPEndPoint[] connects = XRSTDeviceData.GetIP(_device);

            for (int cnt = 0; cnt < connects.Length; cnt++)
            {
                FNI_NET.SendData(connects[cnt], _data);
            }
        }
#else
        protected virtual void Send(byte[] _data)
        {
            FNI_NET.SendData(_data);
        }
#endif
        protected virtual void Receive(NetData _data)
        {

        }
    }
}