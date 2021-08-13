/// 작성자: 백인성
/// 작성일: 2021-04-13
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;
using FNI.XRST;
using FNICSLibrary.Net;
using FNIUnityEngine.hjchae.Net;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

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
    /// 기본적인 통신 모듈입니다.
    /// </summary>
    public class FNI_NET : MonoBehaviour
    {
        // SERVER Define을 사용하려면
        // Unity의 Edit => ProjectSetting => Player => Other Settings => Scripting Define Symbols에
        // SERVER를 추가 후 Enter키를 누른다.
#if SERVER
        /// <summary>
        /// TCP를 사용하기 위한 클래스 입니다.
        /// </summary>
        public static fniTCPServer tcp;
        /// <summary>
        /// TCP가 생성되면 호출합니다.
        /// </summary>
        public UnityEvent onTCPCreated;
        /// <summary>
        /// TCP가 시작하면 호출합니다.
        /// </summary>
        public UnityEvent onTCPStarted;
        /// <summary>
        /// TCP에 Client가 연결되면 호출합니다.
        /// </summary>
        public StringEventHandler onTCPConnected;
        /// <summary>
        /// TCP에 Client가 해제되면 호출합니다.
        /// </summary>
        public StringEventHandler onTCPDisconnected;
#else
        /// <summary>
        /// TCP를 사용하기 위한 클래스 입니다.
        /// </summary>
        public static fniTCPClient tcp;

        /// <summary>
        /// TCP에 연결되면 호출합니다.
        /// </summary>
        public UnityEvent onTCPConnected;
        /// <summary>
        /// TCP에서 해제되면 호출합니다.
        /// </summary>
        public UnityEvent onTCPDisconnected;
#endif

        #region 공용 부분
        /// <summary>
        /// UDP를 사용하기 위한 클래스 입니다.
        /// </summary>
        public static fniUDPStreamer udp;
#if SERVER
        public static bool canSendTCP => tcp.IsAlive;
#else
        public static bool canSendTCP => tcp != null ? (tcp.TcpClient != null ? tcp.TcpClient.Connected : false) : false;
#endif
        public static bool canSendUDP;
        public static bool CanSend => canSendTCP && canSendUDP && FNI_Device.IP.ServerAddress != null;
        /// <summary>
        /// TCP에서 Error가 발생하면 호출합니다.
        /// </summary>
        public StringEventHandler onTCPError;
        /// <summary>
        /// UDP에서 Error가 발생하면 호출합니다.
        /// </summary>
        public StringEventHandler onUDPError;

        /// <summary>
        /// Network를 초기화합니다.
        /// </summary>
        public void InitNetwork()
        {
            Debug.Log("My IP: " + FNI_Device.IP.My);

            InitUDP();
            InitTCP();
        }

        /// <summary>
        /// Data가 수신되면 호출되는 함수입니다.
        /// </summary>
        /// <param name="data">수신된 Data</param>
        /// <param name="from">보낸 곳</param>
        protected virtual void Tcp_DataReceived(byte[] data, TcpClient from) { }

        /// <summary>
        /// Data가 수신되면 호출되는 함수입니다.
        /// </summary>
        /// <param name="data">수신된 Data</param>
        /// <param name="from">보낸 곳</param>
        protected virtual void Udp_DataReceived(byte[] data, IPEndPoint from) { }

        /// <summary>
        /// UDP Error가 수신되면 호출
        /// </summary>
        /// <param name="e">수신된 Error정보</param>
        protected virtual void Udp_Error(SystemException e)
        {
            Debug.LogError(e.ToString());
            onUDPError?.Invoke(e.ToString());
        }
        /// <summary>
        /// TCP Error가 수신되면 호출
        /// </summary>
        /// <param name="e">수신된 Error정보</param>
        protected virtual void Tcp_Error(SystemException e)
        {
            Debug.LogError(e.ToString());
            onTCPError?.Invoke(e.ToString());
        }
        #endregion

#if SERVER
        /// <summary>
        /// UDP를 초기화 합니다.
        /// </summary>
        private void InitUDP()
        {
            StopUDP();

            udp = new fniUDPStreamer(this);
            udp.Create(FNI_Device.Port.Get(FNI_Device.Port.Type.UDP_Server));

            udp.DataReceived += Udp_DataReceived;
            udp.Error += Udp_Error;
        }
        /// <summary>
        /// TCP를 초기화 합니다.
        /// </summary>
        private void InitTCP()
        {
            StopTCP();

            tcp = new fniTCPServer(this);

            tcp.Created += Tcp_Created;
            tcp.Started += Tcp_Started;
            tcp.ClientConnected += Tcp_ClientConnected;
            tcp.ClientDisconnected += Tcp_ClientDisconnected;
            tcp.DataReceived += Tcp_DataReceived;
            tcp.Error += Tcp_Error;
        }
        /// <summary>
        /// UDP를 정지합니다.
        /// </summary>
        private void StopUDP()
        {
            if (udp != null)
            {
                udp.DataReceived -= Udp_DataReceived;

                udp.Destroy();

                udp = null;
            }
        }
        /// <summary>
        /// TCP를 정지합니다.
        /// </summary>
        private void StopTCP()
        {
            if (tcp != null)
            {
                tcp.Created -= Tcp_Created;
                tcp.Started -= Tcp_Started;
                tcp.ClientConnected -= Tcp_ClientConnected;
                tcp.ClientDisconnected -= Tcp_ClientDisconnected;
                tcp.DataReceived -= Tcp_DataReceived;
                tcp.Error -= Tcp_Error;

                if (tcp.IsAlive)
                    tcp.Stop();

                tcp = null;
            }
        }
        /// <summary>
        /// Network를 정지합니다.
        /// </summary>
        public void StopNetwork()
        {
            StopUDP();
            StopTCP();
        }
        /// <summary>
        /// TCP를 시작합니다.
        /// </summary>
        public void StartTCP()
        {
            tcp.Start(FNI_Device.IP.My, FNI_Device.Port.Get(FNI_Device.Port.Type.TCP_Server));
        }
        /// <summary>
        /// TCP를 생성하면 호출
        /// </summary>
        protected virtual void Tcp_Created()
        {
            Debug.Log("TCP Server Create!");
            onTCPCreated?.Invoke();
        }
        /// <summary>
        /// TCPServer가 시작되면 호출
        /// </summary>
        protected virtual void Tcp_Started()
        {
            Debug.Log("TCP Server Started!");
            onTCPStarted?.Invoke();
        }
        /// <summary>
        /// TCP Client가 Server에 연결되면 호출
        /// </summary>
        /// <param name="client">접속한 Client</param>
        /// <param name="ip">접속한 Client의 IP</param>
        /// <param name="port">접속한 Client의 Port</param>
        protected virtual void Tcp_ClientConnected(TcpClient client, IPAddress ip, int port)
        {
            Debug.Log($"TCP Client Connected: {client} {ip}:{port}");
            onTCPConnected?.Invoke($"TCP Client Connected: {client} {ip}:{port}");
        }
        /// <summary>
        /// TCP Client가 Server에 연결이 해제되면 호출
        /// </summary>
        /// <param name="client">해제한 Client</param>
        /// <param name="ip">해제한 Client의 IP</param>
        /// <param name="port">해제한 Client의 Port</param>
        protected virtual void Tcp_ClientDisconnected(TcpClient client, IPAddress ip, int port)
        {
            Debug.Log($"TCP Client Disconnected: {client} {ip}:{port}");
            onTCPDisconnected?.Invoke($"TCP Client Disconnected: {client} {ip}:{port}");
        }
        /// <summary>
        /// 데이터를 전송합니다. data의 0번째는 항상 <see cref="NetworkDataType"/>을 <see cref="byte"/>로 변환하여 사용해야 합니다.
        /// </summary>
        /// <param name="ip">전송할 주소</param>
        /// <param name="data">전송할 데이터</param>
        public static void SendData(IPEndPoint ip, params byte[] data)
        {
            if (data == null || data.Length == 0) return;

            if (0 <= data[0] && data[0] < (byte)NetworkDataType.T_DeviceFeedback)
            {
                udp.SendData(data, ip);
            }
            else if (data[0] < (byte)NetworkDataType.Lenth)
            {
                TcpClientInfo tcpClient = tcp.FindClient(ip.Address.ToString());
                tcp.SendData(data, tcpClient.tcpClient);
            }
            else
            {
                Debug.Log("형식외의 데이터를 사용했거나 형식에 맞지 않습니다. " +
                          "인자값의 data는 항상 0번째가 NetworkDataType을 byte로 변환한 값입니다.");
            }
        }
#else
        /// <summary>
        /// UDP를 초기화 합니다.
        /// </summary>
        private void InitUDP()
        {
            udp = new fniUDPStreamer(this);
            udp.Create(FNI_Device.Port.Get(FNI_Device.Port.Type.UDP_Client));

            udp.DataReceived += Udp_DataReceived;
            udp.Error += Udp_Error;

            canSendUDP = true;
        }
        /// <summary>
        /// TCP를 초기화 합니다.
        /// </summary>
        private void InitTCP()
        {
            tcp = new fniTCPClient(this);

            tcp.Connected += Tcp_Connected;
            tcp.Disconnected += Tcp_Disconnected;
            tcp.DataReceived += Tcp_DataReceived;
            tcp.Error += Tcp_Error;
        }
        /// <summary>
        /// UDP를 정지합니다.
        /// </summary>
        private void StopUDP()
        {
            if (udp != null)
            {
                udp.DataReceived -= Udp_DataReceived;

                udp.Destroy();

                canSendUDP = false;
                udp = null;
            }
        }
        /// <summary>
        /// TCP를 정지합니다.
        /// </summary>
        private void StopTCP()
        {
            if (tcp != null)
            {
                tcp.Connected -= Tcp_Connected;
                tcp.Disconnected -= Tcp_Disconnected;
                tcp.DataReceived -= Tcp_DataReceived;
                tcp.Error -= Tcp_Error;

                if (tcp.IsAlive)
                    tcp.Disconnect();

                tcp = null;
            }
        }
        /// <summary>
        /// Network를 정지합니다.
        /// </summary>
        public void StopNetwork()
        {
            FNI_Device.IP.ServerAddress = null;
            StopUDP();
            StopTCP();
        }

        /// <summary>
        /// ServerIP가 특정되면 호출해야 합니다.
        /// </summary>
        public void SetServerIP(IPEndPoint ip)
        {
            FNI_Device.IP.ServerAddress = ip.Address;
        }

        /// <summary>
        /// TCP를 실행합니다. <see cref="SetServerIP"/>를 먼저 호출해야 합니다.
        /// </summary>
        public void StartTCP()
        {
            if (FNI_Device.IP.ServerAddress != null)
            {
                Debug.Log("Try TCP Server!");
                tcp.Connect(FNI_Device.IP.ServerAddress.ToString(), FNI_Device.Port.Get(FNI_Device.Port.Type.TCP_Server));
            }
            else
            {
                Debug.Log("No TCP ServerIP!");
            }
        }
        /// <summary>
        /// TCP Server에 연결되면 호출됩니다.
        /// </summary>
        protected virtual void Tcp_Connected()
        {
            Debug.Log("Connected to the TCP Server!");
            onTCPConnected?.Invoke();
        }
        /// <summary>
        /// TCP Server에서 연결이 해제 되면 호출됩니다.
        /// </summary>
        protected virtual void Tcp_Disconnected()
        {
            Debug.Log("Disconnected to the TCP Server!");

            StopNetwork();
            onTCPDisconnected?.Invoke();
        }

        /// <summary>
        /// 데이터를 전송합니다. data의 0번째는 항상 <see cref="NetworkDataType"/>을 <see cref="byte"/>로 변환하여 사용해야 합니다.
        /// </summary>
        /// <param name="ip">전송할 주소</param>
        /// <param name="data">전송할 데이터</param>
        public static void SendData(params byte[] data)
        {
            if (FNI_Device.IP.ServerAddress == null) return;
            if (data == null || data.Length == 0) return;

            if (0 <= data[0] && data[0] < (byte)NetworkDataType.T_DeviceFeedback)
            {
                udp.SendData(data, FNI_Device.IP.UDP_Server);
            }
            else if (data[0] < (byte)NetworkDataType.Length)
            {
                tcp.SendData(data);
            }
            else
            {
                Debug.Log("형식외의 데이터를 사용했거나 형식에 맞지 않습니다. " +
                          "인자값의 data는 항상 0번째가 NetworkDataType을 byte로 변환한 값입니다.");
            }
        }
#endif
    }
}