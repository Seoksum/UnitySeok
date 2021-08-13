/// 작성자: 김윤빈
/// 작성일: 2021-04-19
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// (2021-07-15) 백인성
/// 1. 캐릭터의 활성화 여부를 이곳에서만 설정 할 수 있도록 수정함
/// 2. 기존 GameObject로 관리되던 캐릭터를 XRST_SyncCharactor로 변경하여
///    캐릭터 동기화 데이터를 각자 보낼 수 있도록 수정함.
/// 3. 기존 SetGameObject를 통해 번호로만 찾아서 실행중인 캐릭터 선택하던 것에서
///    Change함수를 통해 캐릭터 타입에 따라 활성 화 할 수 있도록 수정함.

using FNI.XRST;
using FNI.Sync;
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

namespace FNI.Data
{
    public class FNI_CharacterManager : MonoBehaviour
    {
        public CharactorType defCharactor;

        /// <summary>
        /// 케릭터를 관리할 List
        /// </summary>
        public List<FNI_SyncCharactor> characterList = new List<FNI_SyncCharactor>();

        /// <summary>
        /// 연동시킬 케릭터 데이터
        /// </summary>
        private FNI_SyncCharactor playCharacterData;

        protected void Start()
        {
            SetGameObject(-1);

            playCharacterData = characterList.Find(x => (x.type & defCharactor) == defCharactor);
            playCharacterData.root.gameObject.SetActive(true);
        }

#if !SERVER
        public void SyncStart()
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                characterList[i].useSend = true;
            }
        }
        public void SyncPause()
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                characterList[i].useSend = false;
            }
        }
#endif

        public void Change(CharactorType charactor)
        {
            FNI_SyncCharactor find = characterList.Find(x => (x.type & charactor) == charactor);
            if (find != null)
            {
                if (playCharacterData != null)
                    SetActive(false);

                playCharacterData = find;

                SetActive(true);
            }
        }
        public void SetActive(bool isActive)
        {
            playCharacterData.root.gameObject.SetActive(isActive);
            /// 2021.07.27 백인성 불필요 값 삭제
            //#if !SERVER
            //            playCharacterData.useSend = isActive;
            //#endif
        }
        /// <summary>
        /// 케릭터를 관리합니다.
        /// </summary>
        /// <param name="value">false를 주면 모든 오브젝트를 꺼줍니다.</param>
        /// <param name="num">value를 True로 주고 num을 입력하면 해당 순서에 맞는 케릭터를 찾아서 켜줍니다.</param>
        public void SetGameObject(int num)
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                characterList[i].root.gameObject.SetActive(i == num);
            }
        }
    }
}