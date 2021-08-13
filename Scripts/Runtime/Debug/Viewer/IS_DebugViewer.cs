using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FNI;
namespace FNI
{
    public class IS_DebugViewer : IS_Debug
    {
        [System.Serializable]
        public class LogSet
        {
            public string log;
            public string stack;
            public LogType type;
            public DateTime wrightTime;
        }
        public enum LogType
        {
            None = 0,
            //
            // 요약:
            //     LogType used for Errors.
            Error,
            //
            // 요약:
            //     LogType used for Asserts. (These could also indicate an error inside Unity itself.)
            Assert,
            //
            // 요약:
            //     LogType used for Warnings.
            Warning,
            //
            // 요약:
            //     LogType used for regular log messages.
            Log,
            //
            // 요약:
            //     LogType used for Exceptions.
            Exception
        }

        private Text Contents
        {
            get
            {
                if (content == null)
                    content = transform.Find("Image/Scroll View/Viewport/Content/Text").GetComponent<Text>();

                return content;
            }
        }

        public bool showStack;
        public bool showLogType;
        public bool showTime;
        public LogType logFilter;

        private Text content;
        private List<LogSet> logList = new List<LogSet>();
        protected override void ExpendHandleLog(string logString, string stackTrace, UnityEngine.LogType type)
        {
            base.ExpendHandleLog(logString, stackTrace, type);

            LogType log = LogType.None;
            switch (type)
            {
                case UnityEngine.LogType.Error:     log = LogType.Error; break;
                case UnityEngine.LogType.Assert:    log = LogType.Assert; break;
                case UnityEngine.LogType.Warning:   log = LogType.Warning; break;
                case UnityEngine.LogType.Log:       log = LogType.Log; break;
                case UnityEngine.LogType.Exception: log = LogType.Exception; break;
            }

            LogSet logSet = new LogSet()
            {
                log = logString,
                stack = stackTrace,
                type = log,
                wrightTime = DateTime.Now
            };

            logList.Add(logSet);

            ShowContents();
        }

        public void OnShowStack(bool isOn)
        {
            showStack = isOn;
            ShowContents();
        }
        public void OnShowLogType(bool isOn)
        {
            showLogType = isOn;
            ShowContents();
        }
        public void OnShowType(int select)
        {
            logFilter = (LogType)select;
            ShowContents();
        }
        public void OnShowTime(bool isOn)
        {
            showTime = isOn;
            ShowContents();
        }

        public void ShowContents()
        {
            Contents.text = "";
            int count = 0;
            for (int cnt = 0; cnt < logList.Count; cnt++)
            {
                if (logFilter == LogType.None || logList[cnt].type == logFilter)
                {
                    if (count != 0)
                        Contents.text += "\n\n";

                    if (showLogType)
                    {
                        string color = "";
                        switch (logList[cnt].type)
                        {
                            case LogType.Error: color = "#ff0000ff";/*red*/ break;
                            case LogType.Exception: color = "#ffa500ff"/*orange*/; break;
                            case LogType.Warning: color = "#ffff00ff";/*yello*/ break;
                            case LogType.Assert: color = "#ffffffff";/*white*/ break;
                            case LogType.Log: color = "";/*None*/ break;
                        }
                        Contents.text += (showTime ? $"[{logList[cnt].wrightTime.ToString(@"hh\:mm\:ss")}] " : "") + $"<color={color}>[{logList[cnt].type}]</color>";
                        Contents.text += "\n";
                    }
                    else
                    {
                        if (showTime)
                            Contents.text += $"[{logList[cnt].wrightTime.ToString(@"hh\:mm\:ss")}] \n";
                    }
                    Contents.text += logList[cnt].log;

                    if (showStack)
                    {
                        Contents.text += "\n";
                        Contents.text += logList[cnt].stack;
                    }

                    count++;
                }
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(IS_DebugViewer))]
public class IS_DebugViewerEditor : Editor
{
    private IS_DebugViewer m_target;

    void OnEnable()
    {
        m_target = base.target as IS_DebugViewer;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        float labelWidth = (EditorGUIUtility.currentViewWidth - 70) * 0.5f;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show Stack", GUILayout.Width(labelWidth));
        bool showStack = EditorGUILayout.Toggle(m_target.showStack);
        if (m_target.showStack != showStack)
        {
            m_target.showStack = showStack;
            m_target.ShowContents();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show Log Type", GUILayout.Width(labelWidth));
        bool showLogType = EditorGUILayout.Toggle(m_target.showLogType);
        if (m_target.showLogType != showLogType)
        {
            m_target.showLogType = showLogType;
            m_target.ShowContents();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show Time", GUILayout.Width(labelWidth));
        bool showTime = EditorGUILayout.Toggle(m_target.showTime);
        if (m_target.showTime != showTime)
        {
            m_target.showTime = showTime;
            m_target.ShowContents();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Type Filter", GUILayout.Width(labelWidth));
        IS_DebugViewer.LogType log = (IS_DebugViewer.LogType)EditorGUILayout.EnumPopup(m_target.logFilter);
        if (m_target.logFilter != log)
        {
            m_target.logFilter = log;
            m_target.ShowContents();
        }
        EditorGUILayout.EndHorizontal();


        //여기까지 검사해서 필드에 변화가 있으면
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Changed Update Mode");
            //변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
            EditorUtility.SetDirty(m_target);
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}
#endif