/// 작성자: 백인성 
/// 작성일: 2018-05-01 
/// 수정일: 2018-07-25
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력 
/// (2018-07-25) 백인성 
///    1. 함수 구조 및 네이밍 규칙 적용

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using FNI;
namespace FNI
{
    namespace DebugWriter
    {
        /// *** 사용방법 ***
        /// 로그기록을 원하는 곳에 아래 함수를 복사에서 붙여 넣습니다. 세번째 인자인 숫자는 줄번호이므로 직접 해당 줄번호를 입력해야 하고, 네번째 인자는 출력할 메시지이므로 직접입력합니다.
        /// 
        /// CustomLogger.StartUp(string sPathName, int nLimitKiloByte = 0)을 먼저 실행 한다.
        /// CustomLogger.WriteLog(this.name, GetType().Name, 13, "여기에 메시지를 입력하세요.");
        ///
        /// 로그 파일은 Editor에서는 Asset 폴더에 저장되고 exe 실행 중에는 Data 폴더에 runtime_log.txt 파일로 저장됩니다.
        public class CustomLogger
        {
            /// <summary>
            /// 현재 시간을 지정한 형식의 문자열 형태로 가져온다.
            /// </summary>
            public static string TimeNow
            {
                get
                {
                    return System.DateTime.Now.ToString("HH:mm:ss.fff");
                }
            }
            
            /// <summary>
            /// 로그가 저장될 기본위치
            /// </summary>
            private static string GetLogPath
            {
                get
                {
                    return string.Format("{0}/FNI/Log", Application.streamingAssetsPath);
                }
            }

            /// <summary>
            /// 오늘의 폴더명
            /// </summary>
            private static string GetTodayFolder
            {
                get
                {
                    return DateTime.Now.ToString("yyyyMMdd");
                }
            }

            #region public
            /// <summary>
            /// 폴더의 갯수 한계입니다. 이 이상 폴더가 생성되면 가장 오래된 폴더를 삭제 합니다. (용량 보존을 위해)
            /// </summary>
            public int maxFolderCount = 7;
            #endregion

            /// <summary>
            /// 이 툴이 처음 실행됐는지 확인
            /// </summary>
            private bool isFirstFlag = true;
            /// <summary>
            /// 조합된 저장 경로
            /// </summary>
            private string m_sPathName;
            /// <summary>
            /// 저장 한계
            /// </summary>
            private int m_nLimitKiloByte = 0;
            /// <summary>
            /// 현재 파일 번호
            /// </summary>
            private int curFileNum = 0;
            
            /// <summary>
            /// 가장 먼저 사용해야 하는 함수 입니다. 한번만 실행합니다.
            /// </summary>
            /// <param name="sPathName">파일이 저장될 경로입니다.</param>
            /// <param name="nLimitKiloByte">파일의 용량제한입니다.</param>
            public void StartUp(string sPathName, int nLimitKiloByte = 0)
            {
                string path = string.Format("{0}/{1}", GetLogPath, GetTodayFolder);
                m_sPathName = path + "/" + sPathName;
                m_nLimitKiloByte = nLimitKiloByte;

                //처음 실행인가?
                isFirstFlag = true;

                if (Directory.Exists(path))//폴더 경로를 검사하고 없다면 경로를 생성합니다.
                {
                    Directory.CreateDirectory(path);
                    OldFolderChecker(new DirectoryInfo(GetLogPath));
                }
                else
                {
                    Directory.CreateDirectory(path);
                }

                //파일 경로를 점검 합니다. 지정한 이름이 없을 때까지 반복합니다.
                string fullpath;
                FileInfo fi;
                while (true)
                {
                    fullpath = string.Format("{0}_{1}.log", m_sPathName, curFileNum);

                    fi = new FileInfo(fullpath);
                    if (fi.Exists)//지정한 이름의 파일이 있다면
                    {
                        if (fi.Length > m_nLimitKiloByte * 1024)//파일의 용량을 점검하고
                        {
                            curFileNum++;//지정용량보다 크다면 카운드를 올리고 다시 While문을 돕니다.
                        }
                        else
                        {
                            break;//지정용량보다 작다면 종료, 위에서 작성한 fullPath가 마지막 경로가 된다.
                        }
                    }
                    else//지정한 이름의 파일이 없다면
                    {
                        //if (0 < curFileNum)
                        //    curFileNum--;
                        break;
                    }
                }
            }
            /// <summary>
            /// 메시지를 기록할 때 사용하는 함수 입니다.
            /// </summary>
            /// <param name="sLogMessage">기록할 메시지 입니다.</param>
            /// <param name="time">시간을 기록 여부 입니다.</param>
            public void WriteLog(string sLogMessage, bool time = true)
            {
                string sCurDateTime = time ? string.Format("\r\n[{0}] ", TimeNow) : "";

                string fullpath = string.Format("{0}_{1}.log", m_sPathName, curFileNum);
                FileInfo fi = new FileInfo(fullpath);
                if (fi.Exists)//파일이 존재 할 때 기록시작
                {
                    if (isFirstFlag)//처음 기록일 때
                    {
                        isFirstFlag = false;
                        sCurDateTime = string.Format("\r\n{0}[RestartApp] \r\n{0}", sCurDateTime);
                    }

                    if (fi.Length > m_nLimitKiloByte * 1024)//이미 StartUp에서 카운팅을 하고 온 상태라 기록하면서 용량이 늘었을 때만 체크함
                    {
                        curFileNum++;
                        fullpath = string.Format("{0}_{1}.log", m_sPathName, curFileNum);
                    }
                }
                
                //파일을 기록합니다.
                File.AppendAllText(fullpath, sCurDateTime + sLogMessage, Encoding.UTF8);
            }
            /// <summary>
            /// 오래된 폴더를 삭제 합니다.
            /// </summary>
            private void OldFolderChecker(DirectoryInfo parentDirInfo)
            {
                //오래된 폴더 삭제하기.
                DirectoryInfo[] directorys = parentDirInfo.GetDirectories();//생성해야하는 폴더의 부모로 자녀 검색

                if (maxFolderCount < directorys.Length)//검색한 폴더의 수가 maxFolderCount를 초과하면
                {
                    //폴더명 추가
                    List<int> dis = new List<int>();
                    for (int cnt = 0; cnt < directorys.Length; cnt++)
                    {
                        dis.Add(int.Parse(directorys[cnt].Name));//폴더명이 날짜로 지정되어 있다.
                    }

                    //초과한 갯수만큼 반복하여 오래된 폴더를 삭제한다.
                    while (maxFolderCount < dis.Count)
                    {
                        int min = Mathf.Min(dis.ToArray());//숫자로 저장된 폴더명이기에 가장 작은 숫자가 오래된 폴더이다.
                        string path = parentDirInfo.FullName + "/" + min.ToString();

                        DirectoryInfo minPath = new DirectoryInfo(path);//제일 오래된 폴더
                        if (minPath.Exists)//이 폴더가 있는지 재검사
                        {
                            Directory.Delete(path, true);//폴더 삭제
                            dis.Remove(min);//목록에서 삭제
                        }
                    }
                }
            }
        }
    }
}
