/// 작성자: 백인성
/// 작성일: 2021-05-04
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
using FNI.NET.Utility;
using FNI.XRST;
using FNI.NET;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.Sync
{
    /// <summary>
    /// Async의 기본 형태
    /// </summary>
    public class FNI_SyncBone : FNI_SyncUpdate
    {
        /// <summary>
        /// 동기화 종류 설정
        /// </summary>
        public enum SyncType
        {
            /// <summary>
            /// 아무것도 동기화 하지 않습니다.
            /// </summary>
            SyncNone,
            /// <summary>
            /// 전체 값을 동기화 합니다. [Local위치][각도][크기][활성화]
            /// </summary>
            SyncLPRSA,
            /// <summary>
            /// 전체 값을 동기화 합니다. [위치][각도][크기][활성화]
            /// </summary>
            SyncPRSA,
            /// <summary>
            /// 위치값 제외한 값을 동기화 합니다. [각도][크기][활성화]
            /// </summary>
            SyncRSA,
            /// <summary>
            /// 크기와 활성화 값을 동기화 합니다. [크기][활성화]
            /// </summary>
            SyncSA,
            /// <summary>
            /// 활성화 값만 동기화 합니다. [활성화]
            /// </summary>
            SyncA
        }
        [Header("[Async Option]")]
        public Transform root;
        public bool isSingle = false;

        protected List<Bone> boneList = new List<Bone>();

        protected override void Start()
        {
            base.Start();

            if (isSingle)
            {
                if (Enum.TryParse(root.tag, out SyncType type))
                    boneList.Add(new Bone(root, type));
            }
            else
                GetBones(root);
        }

        private void GetBones(Transform parent)
        {
            if (parent.tag != SyncType.SyncNone.ToString())
            {
                if (Enum.TryParse(parent.tag, out SyncType type))
                    boneList.Add(new Bone(parent, type));
            }

            for (int cnt = 0; cnt < parent.childCount; cnt++)
            {
                GetBones(parent.GetChild(cnt));
            }
        }
    }


    [Serializable]
    public class Bone
    {
        public Transform target;

        public bool canApply;
        public bool usePos;
        public bool useLPos;
        public bool useRot;
        public bool useScl;

        public Bone(Transform target)
        {
            this.target = target;
            canApply = true;
            usePos = false;
            useLPos = false;
            useRot = false;
            useScl = false;
        }
        public Bone(Transform target, FNI_SyncBone.SyncType sync) //다형성
        {
            this.target = target;
            canApply = true;
            usePos = false;
            useLPos = false;
            useRot = false;
            useScl = false;

            switch (sync) //SynLPRSA(LPos,Rot,useScl) , SyncPRSA(Pos,useRot,useScl)
            {
                case FNI_SyncBone.SyncType.SyncLPRSA:
                    useLPos = true;
                    usePos = false;
                    goto case FNI_SyncBone.SyncType.SyncRSA;
                case FNI_SyncBone.SyncType.SyncPRSA:
                    useLPos = false;
                    usePos = true;
                    goto case FNI_SyncBone.SyncType.SyncRSA;
                case FNI_SyncBone.SyncType.SyncRSA:
                    useRot = true;
                    goto case FNI_SyncBone.SyncType.SyncSA;
                case FNI_SyncBone.SyncType.SyncSA:
                    useScl = true;
                    break;
            }
        }

        public byte[] Get()
        {
#if !SERVER
            if (target.name.Contains("Bip"))
            {
#endif
                List<byte> datas = new List<byte>();
                bool isActive = target.gameObject.activeSelf;
                datas.Add((byte)0);
                datas.Add((byte)(isActive ? 1 : 0));
                datas.Add((byte)(usePos ? 1 : 0));
                datas.Add((byte)(useLPos ? 1 : 0));
                datas.Add((byte)(useRot ? 1 : 0));
                datas.Add((byte)(useScl ? 1 : 0)); //datas 리스트에 true면 1byte넣고 false면 0byte를 넣는다.

                if (isActive)
                {
                    if (usePos) //SyncPRSA
                    {
                        datas.AddRange(target.position.x.ToByte());
                        datas.AddRange(target.position.y.ToByte());
                        datas.AddRange(target.position.z.ToByte());
                    }
                    if (useLPos) //SynLPRSA
                    {
                        datas.AddRange(target.localPosition.x.ToByte());
                        datas.AddRange(target.localPosition.y.ToByte());
                        datas.AddRange(target.localPosition.z.ToByte());
                    }
                    if (useRot)
                    {
                        datas.AddRange(target.localRotation.x.ToByte());
                        datas.AddRange(target.localRotation.y.ToByte());
                        datas.AddRange(target.localRotation.z.ToByte());
                        datas.AddRange(target.localRotation.w.ToByte());
                    }
                    if (useScl)
                    {
                        datas.AddRange(target.localScale.x.ToByte());
                        datas.AddRange(target.localScale.y.ToByte());
                        datas.AddRange(target.localScale.z.ToByte());
                    }
                }

                datas[0] = (byte)datas.Count;               
                return datas.ToArray();
#if !SERVER
            }
            else
                return new byte[] { 1 };
#endif
            //     Option.L + (byte.L * (pos3 + rot4 + scl3))
            // Min => 4 + (4 * (0 + 0 + 0)) = 4 + 0 = 4
            // Max => 4 + (4 * (3 + 4 + 3)) = 4 + 28 = 32

        }
        public void Set(byte[] data)
        {
            if (canApply == false) return;

            _ = data[0];//count
            int active = data[1];
            int usePos = data[2];
            int useLPos = data[3];
            int useRot = data[4];
            int useScl = data[5];
            int startNum = 6;// 데이터 시작 지점
            int addNum = 4;// 데이터가져올 byte 크기

            if (active == 0)
                target.gameObject.SetActive(false);
            else
            {
                target.gameObject.SetActive(true);

                if (usePos == 1)
                {
                    target.position = new Vector3(data.ByteToFloat(out _, startNum + (addNum * 0)), //4
                                                  data.ByteToFloat(out _, startNum + (addNum * 1)), //8
                                                  data.ByteToFloat(out _, startNum + (addNum * 2)));//12

                    startNum += (addNum * 3);//16
                    Debug.Log("target!!!!!!");
                }
                if (useLPos == 1)
                {
                    target.localPosition = new Vector3(data.ByteToFloat(out _, startNum + (addNum * 0)), //4
                                                       data.ByteToFloat(out _, startNum + (addNum * 1)), //8
                                                       data.ByteToFloat(out _, startNum + (addNum * 2)));//12

                    startNum += (addNum * 3);//16
                }

                if (useRot == 1)
                {
                    target.localRotation = new Quaternion(data.ByteToFloat(out _, startNum + (addNum * 0)), //4,  16+0  = 16
                                                          data.ByteToFloat(out _, startNum + (addNum * 1)), //8,  16+4  = 20
                                                          data.ByteToFloat(out _, startNum + (addNum * 2)), //12, 16+8  = 24
                                                          data.ByteToFloat(out _, startNum + (addNum * 3)));//16, 16+12 = 28
                    startNum += (addNum * 4);//32
                }

                if (useScl == 1)
                {
                    target.localScale = new Vector3(data.ByteToFloat(out _, startNum + (addNum * 0)), //4  32+0  = 32
                                                    data.ByteToFloat(out _, startNum + (addNum * 1)), //8  32+4  = 36
                                                    data.ByteToFloat(out _, startNum + (addNum * 2)));//12 32+8  = 40
                }
            }
        }
    }
}