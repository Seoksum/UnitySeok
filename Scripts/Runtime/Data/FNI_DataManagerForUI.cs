/// 작성자: 김윤빈
/// 작성일: 2021-04-14
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// (2021-07-15) 백인성
/// 기존 Text 전용 클래스에서 Datamanager상속받아 사용하는 용도로 변경.
/// 기능 축소

using FNI.XRST;
using FNI.NET.Utility;
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
    public class FNI_DataManagerForUI : FNI_DataManager
    {
        [Header("For UI")]
        public Text infoText;
        public float showTextTime = 2;

        private IEnumerator InfoCoroutine;

        public override bool CharacterChangeComplete(CharactorType charactorType)
        {
            string text;
            bool check = base.CharacterChangeComplete(charactorType);

            if (check)
            {
                text = "서버에서 선택한 케릭터와 일치합니다.";
            }
            else
            {
                text = "서버에서 선택한 케릭터와 일치하지 않습니다.";
            }

            if (InfoCoroutine != null)
                StopCoroutine(InfoCoroutine);
            InfoCoroutine = InfomationRoutine(text);
            StartCoroutine(InfoCoroutine);

            return check;
        }

        IEnumerator InfomationRoutine(string messege)
        {
            infoText.transform.parent.gameObject.SetActive(true);
            infoText.SetAllDirty();
            infoText.text = messege;
            yield return new WaitForSeconds(showTextTime);
            infoText.transform.parent.gameObject.SetActive(false);
        }
    }
}