/// 작성자: 백인성
/// 작성일: 2021-04-22
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;
using FNI.XRST;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.NET.Utility
{
    /// <summary>
    /// 수신된 데이터를 처리 하는 클래스 입니다.
    /// </summary>
    public class FNI_NETProcess : FNI_NETQueue
    {
        /// <summary>
        /// <see cref="NetworkDataType.T_DeviceFeedback"/>이 들어오면 호출됩니다.
        /// </summary>
        [HideInInspector] public static NetDataEventHandler onReceiveData_DeviceFeedback = new NetDataEventHandler();
        /// <summary>
        /// <see cref="NetworkDataType.T_Mission"/>이 들어오면 호출됩니다.
        /// </summary>
        [HideInInspector] public static NetDataEventHandler onReceiveData_Mission = new NetDataEventHandler();
        /// <summary>
        /// <see cref="NetworkDataType.T_Student"/>이 들어오면 호출됩니다.
        /// </summary>
        [HideInInspector] public static NetDataEventHandler onReceiveData_Student = new NetDataEventHandler();

        /// <summary>
        /// <see cref="NetworkDataType.U_DeviceConnect"/>이 들어오면 호출됩니다.
        /// </summary>
        [HideInInspector] public static NetDataEventHandler onReceiveData_DeviceConnect = new NetDataEventHandler();
        /// <summary>
        /// <see cref="NetworkDataType.U_SyncObject"/>이 들어오면 호출됩니다.
        /// </summary>
        [HideInInspector] public static NetDataEventHandler onReceiveData_SyncObject = new NetDataEventHandler();
        /// <summary>
        /// <see cref="NetworkDataType.U_SyncCharactor"/>이 들어오면 호출됩니다.
        /// </summary>
        [HideInInspector] public static NetDataEventHandler onReceiveData_SyncCharactor = new NetDataEventHandler();

        public void Init()
        {
            InitNetwork();
#if SERVER
            StartTCP();
#endif
        }

        private void Update()
        {
            if (tcp_DataList.Count != 0)
            {
                TCP_Dequeue();
            }
            if (udp_DataList.Count != 0)
            {
                UDP_Dequeue();
            }
        }
        /// <summary>
        /// <see cref="tcp_DataList"/>에 쌓인 데이터를 처리 합니다.
        /// </summary>
        private void TCP_Dequeue()
        {
            for (int cnt = 0; cnt < tcp_DataList.Count; cnt++)
            {
                NetData data = tcp_DataList.Dequeue();

                switch (data.Type)
                {
                    case NetworkDataType.T_DeviceFeedback: onReceiveData_DeviceFeedback?.Invoke(data); break;
                    case NetworkDataType.T_Mission: onReceiveData_Mission?.Invoke(data); break;
                    case NetworkDataType.T_Student: onReceiveData_Student?.Invoke(data); break;
                }
            }
        }
        /// <summary>
        /// <see cref="udp_DataList"/>에 쌓인 데이터를 처리 합니다.
        /// </summary>
        private void UDP_Dequeue()
        {
            for (int cnt = 0; cnt < udp_DataList.Count; cnt++)
            {
                NetData data = udp_DataList.Dequeue();

                switch (data.Type)
                {
                    case NetworkDataType.U_DeviceConnect: onReceiveData_DeviceConnect?.Invoke(data); break;
                    case NetworkDataType.U_SyncObject: onReceiveData_SyncObject?.Invoke(data); break;
                    case NetworkDataType.U_SyncCharactor: onReceiveData_SyncCharactor?.Invoke(data); break;
                }
            }
        }
    }
}