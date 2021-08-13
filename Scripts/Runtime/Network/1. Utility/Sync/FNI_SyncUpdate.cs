/// 작성자: 백인성
/// 작성일: 2021-05-06
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.NET;
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

namespace FNI.Sync
{
    public class FNI_SyncUpdate : FNI_NetBase
    {
        private float UpdateTime { get => 1f / (float)targetFrame; }

        [Header("[Send Option]")]
        public int targetFrame = 30;

        private float curTime = 0;

        public bool useSend = false;

        protected FNI_Pool<byte[]> dataPool;

        protected override void Start()
        {
            base.Start();
            dataPool = new FNI_Pool<byte[]>(() => new byte[0]);
        }

        protected virtual void Update()
        {
            if (useSend == false) return;

            if (UpdateTime < curTime)
            {
                curTime = 0;
                Send();
            }
            else
            {
                curTime += Time.deltaTime;
            }
        }
        public virtual void Send()
        {
            while (!dataPool.IsEmpty)
            {
                byte[] data = dataPool.Get();
#if SERVER
                Send(data, FNI_Device.DeviceType.Hololens);
#else
                Send(data);
#endif
            }
        }
    }
}