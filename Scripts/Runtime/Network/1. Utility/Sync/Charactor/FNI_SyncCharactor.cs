/// 작성자: 백인성
/// 작성일: 2021-07-15
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
using FNI.XRST;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.Sync
{
    /// <summary>
    /// AsyncCharactor 데이터 종류
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 환자 번호
        /// </summary>
        PatientCase,
        /// <summary>
        /// 얼굴 블랜딩 값
        /// </summary>
        Facial,
        /// <summary>
        /// 케릭터 뼈 값
        /// </summary>
        Transform
    }
    public class FNI_SyncCharactor : FNI_SyncBone
    {
        [Header("[Type]")]
        public CharactorType type;

        protected override void Start()
        {
            dataType = NetworkDataType.U_SyncCharactor;
            base.Start();
        }
        public override void Send()
        {
            Send_Transform();

            base.Send();
        }
        private void Send_Transform()
        {
            List<byte> datas = new List<byte>
            {
                (byte)dataType,
                (byte)type,
                (byte)DataType.Transform,
                (byte)boneList.Count
            };

            for (int cnt = 0; cnt < boneList.Count; cnt++)
            {
                datas.AddRange(boneList[cnt].Get());
            }

            dataPool.Set(datas.ToArray());
        }
        protected sealed override void Receive(NetData datas)
        {
            _ = datas.data[0];//netDataType
            CharactorType charactorType = (CharactorType)datas.data[1];
            DataType dataType = (DataType)datas.data[2];

            if ((type & charactorType) != charactorType) return;

            Receive(dataType, datas.data);
        }

        protected virtual void Receive(DataType dataType, byte[] data)
        {
            if (dataType != DataType.Transform) return;

            _ = data[0];//netDataType
            _ = data[1];//charactorType
            _ = data[2];//dataType
            int boneLength = data[3];

            int start = 4;
            for (int cnt = 0; cnt < boneLength; cnt++)
            {
                int count = data[start];

                if (count != 1)
                {
                    byte[] _update = IS_Bytes.GetArea(data, start, count);
                    try
                    {
                        boneList[cnt].Set(_update);
                    }
                    catch(ArgumentOutOfRangeException aoore)
                    {
                        Debug.Log($"{type}[{cnt}=>{count}]\n{aoore}");
                    }
                }
                start += count;
            }
        }
    }
}