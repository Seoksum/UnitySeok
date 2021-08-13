/// 작성자: 백인성
/// 작성일: 2021-07-14
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
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class FNI_Trigger : MonoBehaviour
    {
        public UnityEvent onStart;

        private void Start()
        {
            onStart?.Invoke();
            Debug.Log("Start Event");
        }
    }
}