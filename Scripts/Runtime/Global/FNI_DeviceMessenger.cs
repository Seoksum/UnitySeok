/// 작성자: 백인성
/// 작성일: 2021-04-23
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.XRST;
using FNI.NET;
using FNI.NET.Utility;
using FNI.Common.Utils;

using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// 
/// SendData Format Definition
/// byte[] = [0]DeviceControllerState,      [1]InitType,CharactorType,ControllerType
///          [0]DeviceControllerState.Data, [1]DataLenth, [2~]Data
/// 

namespace FNI
{
    public class FNI_DeviceMessenger : MonoBehaviour
    {

#if SERVER
        /// <summary>
        /// 초기화 완료 메시지 수신
        /// </summary>
        public UnityEvent onInitializationDone;
        /// <summary>
        /// [OptiTrack => Server] 캐릭터 변경 완료 메시지 수신
        /// </summary>
        public UnityEvent onCharactorDone;
#else
        /// <summary>
        /// 초기화 명령 수신
        /// </summary>
        public UnityEvent onInitialization;
        /// <summary>
        /// 남자 캐릭터 변경 수신
        /// </summary>
        public UnityEvent onCharactor_M;
        /// <summary>
        /// 여자 캐릭터 변경 수신
        /// </summary>
        public UnityEvent onCharactor_F;
        /// <summary>
        /// 동기화 시작 수신
        /// </summary>
        public UnityEvent onSyncStart;
        /// <summary>
        /// 동기화 일시정지 수신
        /// </summary>
        public UnityEvent onSyncPause;
        /// <summary>
        /// 동기화 재시작 수신
        /// </summary>
        public UnityEvent onSyncResume;
        /// <summary>
        /// 동기화 종료 수신
        /// </summary>
        public UnityEvent onSyncStop;
#endif
        /// <summary>
        /// 기타 메시지
        /// </summary>
        public NetDataEventHandler onData;

        private void Start()
        {
            FNI_NETProcess.onReceiveData_DeviceFeedback.AddListener(Recieve);
        }
#if SERVER
        public void Recieve(NetData data)
        {
            DeviceCommandType state = (DeviceCommandType)data[1];
            switch (state)
            {
                case DeviceCommandType.ChangeCharacter:
                    CharactorType _cType = (CharactorType)data[2];
                    if (_cType == CharactorType.Done)
                        onCharactorDone?.Invoke();   
                    break;
                case DeviceCommandType.Data:          
                    onData?.Invoke(data);      
                    break;
                case DeviceCommandType.Controll:
                    ReceiveControll(data);
                    break;
            }
        }
        protected void Send(byte[] datas, params IPEndPoint[] ip)
        {
            byte[] makeData = MakeData(datas);

            for (int cnt = 0; cnt < ip.Length; cnt++)
                FNI_NET.SendData(ip[cnt], makeData);
        }
        protected virtual void ReceiveControll(NetData data)
        { 
        
        }
#else
        public void Recieve(NetData data)
        {
            DeviceCommandType state = (DeviceCommandType)data.data[1];
            switch (state)
            {
                case DeviceCommandType.ChangeCharacter:
                    CharactorType _state = (CharactorType)data.data[2];
                    if ((_state & CharactorType.Male) == CharactorType.Male)
                        onCharactor_M?.Invoke();
                    if ((_state & CharactorType.Female) == CharactorType.Female)
                        onCharactor_F?.Invoke();

                    break;
                case DeviceCommandType.Controll:
                    DeviceControllType __state = (DeviceControllType)data.data[2];
                    switch (__state)
                    {
                        case DeviceControllType.Initialization:  onSyncStart?.Invoke();  break;
                        case DeviceControllType.Start:  onSyncStart?.Invoke();  break;
                        case DeviceControllType.Pause:  onSyncPause?.Invoke();  break;
                        case DeviceControllType.Resume: onSyncResume?.Invoke(); break;
                        case DeviceControllType.Stop:   onSyncStop?.Invoke();   break;
                    }
                    break;
                case DeviceCommandType.Data:        
                    onData?.Invoke(data);      
                    break;
            }
        }
        protected void Send(params byte[] datas)
        {
            FNI_NET.SendData(MakeData(datas));
        }
        public void Send(DeviceControllType state)
        {
            Send((byte)state);
        }
#endif
        private byte[] MakeData(params byte[] addData)
        {
            byte[] data = new byte[addData.Length + 1];
            data[0] = (byte)NetworkDataType.T_DeviceFeedback;

            for (int cnt = 0; cnt < addData.Length; cnt++)
            {
                data[cnt + 1] = addData[cnt];
            }

            return data;
        }
    }
}