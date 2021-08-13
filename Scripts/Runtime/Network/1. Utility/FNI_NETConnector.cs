/// 작성자: 백인성
/// 작성일: 2021-04-14
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.XRST;
using FNI.Common.Utils;

using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.NET.Utility
{
    /// <summary>
    /// Server와 Client간의 연결을 자동으로 해주기 위해 사용합니다.
    /// </summary>
    [RequireComponent(typeof(FNI_NETProcess))]
    public class FNI_NETConnector : FNI_NetBase
    {
        /// <summary>
        /// 장치 연결시 사용
        /// </summary>
        protected enum State
        {
            /// <summary>
            /// Client에서 접속을 시도합니다.
            /// </summary>
            Broadcast,
            /// <summary>
            /// Client에서 접속을 종료 합니다.
            /// </summary>
            Stop,
            /// <summary>
            /// Server에서 접속을 허가합니다.
            /// </summary>
            Select
        }
        /// <summary>
        /// 연결을 시도 중인 장비 입니다.
        /// </summary>
        [Serializable]
        public class TryConnectDevice
        {
            /// <summary>
            /// 연결시도중인 장비의 IP
            /// </summary>
            public IPEndPoint IP { get => data.ip; }
            /// <summary>
            /// 연결시도중인 장비의 타입
            /// </summary>
            public FNI_Device.DeviceType type;
            /// <summary>
            /// 연결 시도중인 장비의 명칭
            /// </summary>
            public string deviceName;

            /// <summary>
            /// 연결시도중인 장비가 보낸 데이터, 장비명 장비 타입 저장
            /// </summary>
            public NetData data;

            /// <summary>
            /// 선택된 데이터인지 체크되어 있어야 <see cref="Connected"/>에 저장됨
            /// </summary>
            public bool selectThis = false;

            public TryConnectDevice(NetData data)
            {
                this.data = data;

                _ = data[0];//NetworkDataType
                _ = data[1];//broadcast state

                string deviceInfo = data.data.ByteToString(out _, 2);

                Debug.Log($"Incoming: {data.ip}, {(NetworkDataType)data[0]}, {(State)data[1]}, {deviceInfo}");

                string[] split = deviceInfo.Split('/');
                type = (FNI_Device.DeviceType)Enum.Parse(typeof(FNI_Device.DeviceType), split[0]);
                deviceName = split[1];
            }
        }

#if SERVER
        /// <summary>
        /// 외부에 보여주기위한 연결목록
        /// </summary>
        public List<TryConnectDevice> Connectings { get => m_connectingList; }

        /// <summary>
        /// 내부에 저장하기 위한 연결 목록
        /// </summary>
        private List<TryConnectDevice> m_connectingList = new List<TryConnectDevice>();

        public virtual void StepInit()
        {
            FNI_NETProcess.onReceiveData_DeviceConnect.AddListener(RecieveData);
            step.Next();
        }

        public void RecieveData(NetData netData)
        {
            State connectState = (State)netData.data[1];
            switch (connectState)
            {
                case State.Broadcast: OnIncomingAction(netData); break;
                case State.Stop: OnOutgoingAction(netData); break;
            }
        }
        /// <summary>
        /// 데이터를 생성합니다
        /// </summary>
        /// <param name="dataType">생성할 데이터 타입</param>
        /// <returns></returns>
        public byte[] CreatData(NetworkDataType dataType)
        {
            return new byte[] { (byte)NetworkDataType.U_DeviceConnect, (byte)dataType };
        }
        /// <summary>
        /// Server에 장비가 접속하면 호출됩니다.
        /// </summary>
        /// <param name="netData">장비 정보</param>
        private void OnIncomingAction(NetData netData)
        {
            TryConnectDevice find = m_connectingList.Find(x => x.data.GetString == netData.GetString);

            if (find == null)
            {
                Debug.Log($"IP: {netData.ip.Address}");

                TryConnectDevice conect = new TryConnectDevice(netData);
                m_connectingList.Add(conect);

                Incoming(conect);
            }
        }
        /// <summary>
        /// Server에 장비가 접속 해제하면 호출됩니다.
        /// </summary>
        /// <param name="netData">장비 정보</param>
        private void OnOutgoingAction(NetData netData)
        {
            TryConnectDevice find = m_connectingList.Find(x => x.data.GetString == netData.GetString);

            if (find != null)
            {
                m_connectingList.Remove(find);

                Outgoing(find);
            }
        }
        /// <summary>
        /// 장비 접속시 호출합니다.
        /// </summary>
        /// <param name="netData">장비 데이터</param>
        protected virtual void Incoming(TryConnectDevice netData) { }
        /// <summary>
        /// 장비 접속해제시 호출합니다.
        /// </summary>
        /// <param name="netData">장비 데이터</param>
        protected virtual void Outgoing(TryConnectDevice netData) { }

        public virtual void SelectedIP()
        {
            step.Ended();
        }
#else
        /// <summary>
        /// 데이터를 보내고 받기 위해 사용
        /// </summary>
        protected FNI_NETProcess Network
        {
            get
            {
                if (m_network == null)
                    m_network = GetComponent<FNI_NETProcess>();

                return m_network;
            }
        }
        /// <summary>
        /// <see cref="FNI_NETProcess"/>의 Class저장용
        /// </summary>
        private FNI_NETProcess m_network;

        public XRST_ProjectSetting projectSetting;

        [Header("[Broadcast Option]")]
        /// <summary>
        /// 메시지를 반복적으로 보낼 시간(s)
        /// </summary>
        public float broadcastTiming = 1;
        public bool isBroadcast = false;
        [Space]
        /// <summary>
        /// 외부에 Debuging하기 위한 Event입니다.
        /// </summary>
        public StringEventHandler debug;

        /// <summary>
        /// 코루틴문 사용을 위한 변수
        /// </summary>
        private IEnumerator m_brodcastIP_Routine;

        protected override void Start()
        {
            dataType = NetworkDataType.U_DeviceConnect;
            FNI_Device.Type = projectSetting.deviceType;

            base.Start();
        }
        public void StepInit()
        {
            StartBroadcastIp();
        }

        /// <summary>
        /// 데이터를 생성합니다
        /// </summary>
        /// <param name="dataType">생성할 데이터 타입</param>
        /// <returns></returns>
        protected byte[] CreatData(State dataType)
        {
            List<byte> brodcastIP = new List<byte>
            {
                (byte)base.dataType,
                (byte)dataType
            };
            brodcastIP.AddRange(FNI_Device.GetTypeAndName.ToByte());

            return brodcastIP.ToArray();
        }
        protected override void Receive(NetData _data)
        {
            State state = (State)_data[1];
            if (state == State.Select)
            {
                FNI_Device.IP.ServerAddress = _data.ip.Address;
                Network.StartTCP();
                Network.onTCPConnected.AddListener(ConnectTCP_Success);
                Network.onTCPError.AddListener(ConnectTCP_Fail);


                debug?.Invoke("Find Server: " + _data.ip);
            }
            else if (state == State.Stop)
            {
                debug?.Invoke("Disconnect Server: " + _data.ip);
            }
        }
        /// <summary>
        /// TCP에 성공적으로 연결되었을 때 호출
        /// </summary>
        private void ConnectTCP_Success()
        {
            StopBroadcastIP();

            Network.onTCPConnected.RemoveListener(ConnectTCP_Success);
            Network.onTCPError.RemoveListener(ConnectTCP_Fail);

            debug?.Invoke("Connected Server: " + FNI_Device.IP.ServerAddress);
        }
        /// <summary>
        /// TCP연결에 실패 하면 호출
        /// </summary>
        /// <param name="message">실패 사유</param>
        private void ConnectTCP_Fail(string message)
        {
            debug?.Invoke("Fail Connect: " + message);
        }
        /// <summary>
        /// 클라이언트 기능 자신의 IP를 방송?
        /// </summary>
        /// <returns></returns>
        public void StartBroadcastIp()
        {
            isBroadcast = true;
            if (m_brodcastIP_Routine != null)
                StopCoroutine(m_brodcastIP_Routine);
            m_brodcastIP_Routine = BrodcastIP_Routine();

            StartCoroutine(m_brodcastIP_Routine);

            debug?.Invoke("Start Broadcasting");
        }
        /// <summary>
        /// 클라이언트 기능 자신의 IP를 방송 정지
        /// </summary>
        /// <returns></returns>
        public void StopBroadcastIP()
        {
            isBroadcast = false;
            if (m_brodcastIP_Routine != null)
                StopCoroutine(m_brodcastIP_Routine);
            m_brodcastIP_Routine = StopBrodcastIP_Routine();

            StartCoroutine(m_brodcastIP_Routine);
        }
        /// <summary>
        /// 모든 IP로 데이터를 보냅니다.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BrodcastIP_Routine()
        {
            while (true)
            {
                FNI_NET.udp.SendData(CreatData(State.Broadcast), FNI_Device.IP.AnyUDP);

                yield return new WaitForSeconds(broadcastTiming);
            }
        }
        /// <summary>
        /// 모든 IP에 연결중단메시지를 보냅니다.
        /// </summary>
        /// <returns></returns>
        private IEnumerator StopBrodcastIP_Routine()
        {
            yield return new WaitForEndOfFrame();

            FNI_NET.udp.SendData(CreatData(State.Stop), FNI_Device.IP.AnyUDP);
        }
#endif
    }
}
