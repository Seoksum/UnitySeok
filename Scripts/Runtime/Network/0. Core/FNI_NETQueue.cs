/// 작성자: 백인성
/// 작성일: 2021-04-14
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.XRST;
using FNI.Common.Utils;

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
    /// 수신된 데이터를 전달하기 위한 클래스 입니다.
    /// </summary>
    public class FNI_NETQueue : FNI_NET
    {
        /// <summary>
        /// <see cref="Tcp_DataReceived"/>를 통해 들어온 값이 저장됩니다.
        /// </summary>
        protected Queue<NetData> tcp_DataList = new Queue<NetData>();
        /// <summary>
        /// <see cref="Udp_DataReceived"/>를 통해 들어온 값이 저장됩니다.
        /// </summary>
        protected Queue<NetData> udp_DataList = new Queue<NetData>();

        /// <summary>
        /// 체크 되어 있다면 자동 실행
        /// </summary>
        public bool autoStart = true;

        private void Start()
        {
            if (autoStart)
            {
                InitNetwork();
#if SERVER
                StartTCP();
#endif
            }
        }

        /// <summary>
        /// 데이터가 수신되면 호출됩니다.
        /// </summary>
        /// <param name="data">수신된 데이터</param>
        /// <param name="from">데이터를 발송한 곳</param>
        protected override void Tcp_DataReceived(byte[] data, TcpClient from)
        {
#if SERVER
            tcp_DataList.Enqueue(new NetData() { data = data, ip = (IPEndPoint)from.Client.RemoteEndPoint });
#else
            tcp_DataList.Enqueue(new NetData() { data = data, ip = FNI_Device.IP.TCP_Server });
#endif
        }
        /// <summary>
        /// 데이터가 수신되면 호출됩니다.
        /// </summary>
        /// <param name="data">수신된 데이터</param>
        /// <param name="from">데이터를 발송한 곳</param>
        protected override void Udp_DataReceived(byte[] data, IPEndPoint from)
        {
            udp_DataList.Enqueue(new NetData { data = data, ip = from });
        }
    }
}