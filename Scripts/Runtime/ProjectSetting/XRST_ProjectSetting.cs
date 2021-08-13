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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    [CreateAssetMenu(fileName = "new Project Setting Data", menuName = "FNI/Project Setting Data")]
    public class XRST_ProjectSetting : ScriptableObject
    {
        public FNI_Device.DeviceType deviceType;
    }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(XRST_ProjectSetting))]
    public class XRST_ProjectSettingEditor : Editor
    {
        private XRST_ProjectSetting Target
        {
            get
            {
                if (m_target == null)
                    m_target = base.target as XRST_ProjectSetting;

                return m_target;
            }
        }
        private XRST_ProjectSetting m_target;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Device Type", GUILayout.Width(110));
            Target.deviceType = (FNI_Device.DeviceType)EditorGUILayout.EnumPopup(Target.deviceType);
            if (Target.deviceType == FNI_Device.DeviceType.Opti_Tools)
                Target.deviceType = FNI_Device.DeviceType.None;
            if (Target.deviceType == FNI_Device.DeviceType.All)
                Target.deviceType = FNI_Device.DeviceType.None;
            EditorGUILayout.EndHorizontal();

            //여기까지 검사해서 필드에 변화가 있으면
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Changed Update Mode");
                //변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
                EditorUtility.SetDirty(Target);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
#endif
    }
}