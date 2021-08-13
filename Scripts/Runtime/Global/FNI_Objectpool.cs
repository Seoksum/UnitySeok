/// 작성자: 백인성
/// 작성일: 2021-04-19
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    /// <summary>
    /// 오브젝트 Pool입니다.
    /// </summary>
    /// <typeparam name="T">형식</typeparam>
    [Serializable]
    public class FNI_ObjectPool<T> where T : Component
    {
        /// <summary>
        /// 오브젝트 풀의 특정 번호의 오브젝트를 반환합니다.
        /// </summary>
        /// <param name="index">특정 번호</param>
        /// <returns></returns>
        public T this[int index] { get => objectPool[index]; }
        /// <summary>
        /// 현재 오브젝트 풀의 총 갯수
        /// </summary>
        public int Count { get => objectPool.Count; }
        /// <summary>
        /// 활성화중인 오브젝트 갯수
        /// </summary>
        public int ActiveCount
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 생성된 모든 오브젝트
        /// </summary>
        [SerializeField]
        protected List<T> objectPool;
        /// <summary>
        /// 카피될 대상
        /// </summary>
        private T refObject;
        /// <summary>
        /// 카피된 대상이 들어갈 부모
        /// </summary>
        private Transform parent;

        /// <summary>
        /// 생성자
        /// </summary>
        public FNI_ObjectPool()
        {
            objectPool = new List<T>();
        }
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="refObject">복사할 대상</param>
        /// <param name="parent">복사된 대상이 하위로 들어갈 부모</param>
        public FNI_ObjectPool(T refObject, Transform parent)
        {
            objectPool = new List<T>();
            this.refObject = refObject;
            this.parent = parent;
        }
        /// <summary>
        /// 대상을 추가 합니다.
        /// </summary>
        /// <param name="obj"></param>
        public void Add(T obj)
        {
            objectPool.Add(obj);
        }
        public void AllHide()
        {
            for (int cnt = 0; cnt < objectPool.Count; cnt++)
            {
                if (objectPool[cnt].gameObject.activeSelf)
                    objectPool[cnt].gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 활성화 가능한 제일 마지막 오브젝트를 반환합니다. 없다면 추가로 생성합니다.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            T find = FirstDeactiveObject();
            if (find == null)
            {
                T obj = GameObject.Instantiate<T>(refObject);
                obj.transform.SetParent(parent, false);
                obj.gameObject.SetActive(true);
                objectPool.Add(obj);
                return obj;
            }
            else
            {
                find.gameObject.SetActive(true);
                return find;
            }
        }
        /// <summary>
        /// 대상을 찾습니다.
        /// </summary>
        /// <param name="match">검색 조건</param>
        /// <returns></returns>
        public T Find(Predicate<T> match)
        {
            return objectPool.Find(match);
        }
        /// <summary>
        /// 지정한 대상을 숨기고 제일 뒤로 이동시킵니다.
        /// </summary>
        /// <param name="match">조건</param>
        public void Push(Predicate<T> match)
        {
            T find = Find(match);
            if (find != null)
            {
                HideAndMoveLast(find);
            }
        }
        /// <summary>
        /// 대상을 숨기고 마지막으로 이동시킵니다.
        /// </summary>
        /// <param name="target"></param>
        public void HideAndMoveLast(T target)
        {
            target.transform.SetAsLastSibling();
            target.gameObject.SetActive(false);
            objectPool.Remove(target);
            objectPool.Add(target);
        }
        /// <summary>
        /// 첫번째 비활성화 된 오브젝트를 찾습니다.
        /// </summary>
        /// <returns></returns>
        public T FirstDeactiveObject()
        {
            T find = null;
            for (int cnt = 0; cnt < objectPool.Count; cnt++)
            {
                if (objectPool[cnt].gameObject.activeSelf == false)
                {
                    find = objectPool[cnt];
                    break;
                }
            }

            return find;
        }
        /// <summary>
        /// 마지막으로 활성화 되어 있는 오브젝트를 찾습니다.
        /// </summary>
        /// <returns></returns>
        public T LastActiveObject()
        {
            T find = null;
            int cnt = AtLastActiveObject();

            if (cnt != 0)
                find = objectPool[cnt - 1];

            return find;
        }
        /// <summary>
        /// 마지막으로 활성화 되어 있는 오브젝트의 번호를 찾습니다.
        /// </summary>
        /// <returns></returns>
        public int AtLastActiveObject()
        {
            int cnt = 0;

            for (; cnt < objectPool.Count; cnt++)
            {
                if (objectPool[cnt].gameObject.activeSelf == false)
                {
                    break;
                }
            }

            return cnt;
        }
    }

    public class FNI_Pool<T>
    {
        public bool IsEmpty => _objects.IsEmpty;
        public int Count => _objects.Count;

        // ConcurrentBag.TryPeek => 값만 확인
        // ConcurrentBag.TryTake => 값 확인 후 삭제

        private readonly ConcurrentBag<T> _objects;
        private readonly Func<T> _objectGenerator;

        public FNI_Pool(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            _objects = new ConcurrentBag<T>();
        }

        public T Get() => _objects.TryTake(out T item) ? item : _objectGenerator();

        public void Set(T item) => _objects.Add(item);
    }
}