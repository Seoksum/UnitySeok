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
    /// 연결을 위해 선택한 장비 목록을 UI로 표시하는 클래스
    /// </summary>
    public class XRST_ConnectedDevices : MonoBehaviour
    {
        /// <summary>
        /// 선택한 OptiTrack 장비
        /// </summary>
        public XRST_Connector OptiTrack
        {
            get
            {
                if (m_optiTrack == null)
                    m_optiTrack = transform.Find("Margin/OptiTrack").GetComponent<XRST_Connector>();

                return m_optiTrack;
            }
        }
        /// <summary>
        /// 선택한 Tools 장비
        /// </summary>
        public XRST_Connector Tools
        {
            get
            {
                if (m_tools == null)
                    m_tools = transform.Find("Margin/Tools").GetComponent<XRST_Connector>();

                return m_tools;
            }
        }
        /// <summary>
        /// 선택한 Hololens장비
        /// </summary>
        public List<XRST_Connector> HololensList
        {
            get
            {
                if (m_hololensList == null || m_hololensList.Count != 5)
                {
                    m_hololensList = new List<XRST_Connector>();
                    Transform find = transform.Find("Margin/List");

                    for (int cnt = 0; cnt < find.childCount; cnt++)
                    {
                        m_hololensList.Add(find.GetChild(cnt).GetComponent<XRST_Connector>());
                    }
                }

                return m_hololensList;
            }
        }
        /// <summary>
        /// OptiTrack이 연결되었는지 확인
        /// </summary>
        public bool ConnectOptiTrack { get => OptiTrack.IsUse; }
        /// <summary>
        /// Tools가 연결되었는지 확인
        /// </summary>
        public bool ConnectTools { get => Tools.IsUse; }
        /// <summary>
        /// 전체 장비가 연결되었는지 확인, OptiTrack 1대, Tools 1대, Hololens 5대
        /// </summary>
        public bool IsFullLoad
        {
            get
            {
                return ConnectOptiTrack &&
                       ConnectTools &&
                       HololensConnectCount == 5;
            }
        }

        /// <summary>
        /// 최소 장비가 연결되었는지 확인, OptiTrack 1대, Tools 1대, Hololens 1대
        /// </summary>
        public bool IsMinimumLoad
        {
            get
            {
                return ConnectOptiTrack &&
                       ConnectTools &&
                       1 < HololensConnectCount;
            }
        }
        /// <summary>
        /// 연결중인 Hololens의 갯수
        /// </summary>
        public int HololensConnectCount
        {
            get
            {
                int count = 1;
                for (int cnt = 0; cnt < HololensList.Count; cnt++)
                {
                    if (HololensList[cnt].IsUse) count++;
                }

                return count;
            }
        }

        private XRST_Connector m_tools;
        private XRST_Connector m_optiTrack;
        private List<XRST_Connector> m_hololensList;
        private int incomingCount = 0;

        /// <summary>
        /// 선택정보 입력
        /// </summary>
        /// <param name="deviceType">장비 종류</param>
        /// <param name="deviceName">장비 명</param>
        /// <param name="ip">장비 IP</param>
        /// <returns></returns>
        public bool Set(FNI_Device.DeviceType deviceType, string deviceName, string ip)
        {
            switch (deviceType)
            {
                case FNI_Device.DeviceType.Hololens:  return SetHololens(deviceName, ip);
                case FNI_Device.DeviceType.Optitrack: SetOptiTrack(deviceName, ip);
                                           return true;
                case FNI_Device.DeviceType.Tools:     SetTools(deviceName, ip);
                                           return true;
                default:                   return false;
            }
        }

        public void Clear()
        {
            OptiTrack.SetContent(FNI_Device.DeviceType.Optitrack, XRSTConst.kNoConnect, XRSTConst.kNoIP);
            Tools.SetContent(FNI_Device.DeviceType.Tools, XRSTConst.kNoConnect, XRSTConst.kNoIP);

            for (int cnt = 0; cnt < HololensList.Count; cnt++)
            {
                XRST_Connector find = HololensList[0];

                find.SetContent(FNI_Device.DeviceType.Hololens, XRSTConst.kNoConnect, XRSTConst.kNoIP);
                find.transform.SetAsLastSibling();
                HololensList.Remove(find);
                HololensList.Add(find);

                RemoveIncommingCount();
            }
        }
        private void SetOptiTrack(string deviceName, string ip)
        {
            if (OptiTrack.DeviceName.text == "" || (OptiTrack.IPText.text != ip))
            {
                OptiTrack.SetContent(FNI_Device.DeviceType.Optitrack, deviceName, ip);
            }
            else
            {
                OptiTrack.SetContent(FNI_Device.DeviceType.Optitrack, XRSTConst.kNoConnect, XRSTConst.kNoIP);
            }
        }
        private void SetTools(string deviceName, string ip)
        {
            if (Tools.DeviceName.text == "" || Tools.IPText.text != ip)
            {
                Tools.SetContent(FNI_Device.DeviceType.Tools, deviceName, ip);
            }
            else
            {
                Tools.SetContent(FNI_Device.DeviceType.Tools, XRSTConst.kNoConnect, XRSTConst.kNoIP);
            }
        }
        private bool SetHololens(string deviceName, string ip)
        {
            XRST_Connector find = HololensList.Find(x => x.IPText.text == ip);

            if (find == null)
            {
                if (incomingCount < 0 && 5 <= incomingCount) return false;
                else
                {
                    HololensList[incomingCount].SetContent(FNI_Device.DeviceType.Hololens, deviceName, ip);
                    
                    return AddIncommingCount();
                }
            }
            else
            {
                find.SetContent(FNI_Device.DeviceType.Hololens, XRSTConst.kNoConnect, XRSTConst.kNoIP);
                find.transform.SetAsLastSibling();
                HololensList.Remove(find);
                HololensList.Add(find);

                return RemoveIncommingCount();
            }
        }

        private bool AddIncommingCount()
        {
            incomingCount++;
            if (5 < incomingCount)
            {
                incomingCount = 5;
                return false;
            }
            else
                return true;
        }

        private bool RemoveIncommingCount()
        {
            incomingCount--;
            if (incomingCount < 0)
            {  
                incomingCount = 0;
                return false;
            }
            else
                return true;
        }
    }
}