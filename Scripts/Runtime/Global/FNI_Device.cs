/// 작성자: 백인성
/// 작성일: 2021-04-22
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using FNI.XRST;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    /// <summary>
    /// 장치 정보를 볼 수 있는 곳입니다.
    /// </summary>
    public class FNI_Device
    {
        public static class IP
        {
            /// <summary>
            /// 현재 PC의 IP가져오기
            /// </summary>
            public static string My
            {
                get
                {
                    return MyAddress.ToString();
                }
            }
            /// <summary>
            /// 현재 PC의 IP가져오기
            /// </summary>
            public static IPAddress MyAddress
            {
                get
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip;
                        }
                    }

                    throw new Exception("No network adapters with an IPv4 address in the system!");
                }
            }
            /// <summary>
            /// 모든 IP, IPAddress.Any가 작동하지 않아 만들었음
            /// </summary>
            public static IPEndPoint AnyUDP
            {
                get
                {
                    if (any == null)
                    {
                        any = new IPAddress(new byte[] { 255, 255, 255, 255 });
                    }
#if SERVER
                    return new IPEndPoint(any, Port.Get(Port.Type.UDP_Client));
#else
                    return new IPEndPoint(any, Port.Get(Port.Type.UDP_Server));
#endif
                }
            }
#if !SERVER
            /// <summary>
            /// TCP Server 주소입니다.
            /// </summary>
            public static IPEndPoint TCP_Server { get => new IPEndPoint(ServerAddress, Port.Get(Port.Type.TCP_Server)); }
            /// <summary>
            /// UDP Server 주소입니다.
            /// </summary>
            public static IPEndPoint UDP_Server { get => new IPEndPoint(ServerAddress, Port.Get(Port.Type.UDP_Server)); }
            /// <summary>
            /// Server의 IP를 저장합니다.
            /// </summary>
            public static IPAddress ServerAddress;
#endif
            private static IPAddress any;
        }
        /// <summary>
        /// XRST프로젝트의 포트를 공통적으로 적용하기 위해 사용합니다.
        /// </summary>
        public static class Port
        {
            /// <summary>
            /// Port Type
            /// </summary>
            public enum Type
            {
                /// <summary>
                /// 기본 포트
                /// </summary>
                Base,
                /// <summary>
                /// UDP Client용 포트
                /// </summary>
                UDP_Client,
                /// <summary>
                /// UDP Server용 포트
                /// </summary>
                UDP_Server,
                /// <summary>
                /// TCP Server용 포트
                /// </summary>
                TCP_Server
            }

            private readonly static int basePort = 6000;

            private readonly static int addPortUDP_Server = 0;
            private readonly static int addPortUDP_Client = 1;

            private readonly static int addPortTCP = 10;

            /// <summary>
            /// 설정된 Port를 가져옵니다.
            /// </summary>
            /// <param name="type">가져올 타입</param>
            /// <returns></returns>
            public static int Get(Type type)
            {
                switch (type)
                {
                    case Type.UDP_Client: return basePort + addPortUDP_Client;
                    case Type.UDP_Server: return basePort + addPortUDP_Server;
                    case Type.TCP_Server: return basePort + addPortTCP;
                    default: return basePort;
                }
            }
        }
        /// <summary>
        /// 연결되는 Device의 종류 입니다.
        /// </summary>
        public enum DeviceType
        {
            None = 0x0000,
            Hololens = 0x0001,
            Optitrack = 0x0002,
            Tools = 0x0004,
            Opti_Tools = Optitrack | Tools,
            All = Hololens | Optitrack | Tools
        }

        public static IPEndPoint MyIP
        {
            get
            {
                if (myIP == null)
                    myIP = new IPEndPoint(IP.MyAddress, Port.Get(Port.Type.UDP_Client));
                return myIP;
            }
        }
        public static string GetTypeAndName => $"{Type}/{GetDeviceName}";
        public static string GetDeviceName => SystemInfo.deviceName;
        public static DeviceType Type;

        private static IPEndPoint myIP;
    }
}