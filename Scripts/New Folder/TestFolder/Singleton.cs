using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null) //인스턴스 있는지 확인
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                //FindObjectOfType : 오브젝트형(혹은 컴포넌트의 형)으로 검색해서 가장 처음 나타난 오브젝트를 반환
                //as : 캐스트 성공시 그 결과를 리턴하고, 실패시 null 리턴
                if (_instance == null)
                {
                    GameObject newInstance = new GameObject();
                    _instance = newInstance.AddComponent<T>(); //새오브젝트 생성 후 컴포넌트 T를 추가 
                    newInstance.name = "Singleton("+typeof(T).ToString()+")"; //이름을 지어줌
                }
            }
            return _instance;
        }
    }

}
