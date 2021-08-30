using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveRotation : MonoBehaviour
{
    GameObject Center; 
    GameObject RightHand; 
    GameObject LeftHand;
    GameObject RightFoot; 
    GameObject LeftFoot;
    //IK로 받아온 공 오브젝트
    GameObject[] objList = new GameObject[5]; //위 오브젝트를 담아주는 배열 objList

    public static List<ObjectTransform> disList = new List<ObjectTransform>(); //ObjectTransform 구조체를 담는 리스트 
    
    int i = 0;
    static int cnt = 0;
    int j = 0;
    int k = 0;

    public struct ObjectTransform
    {
        public Transform CenterTransform;
        public Transform RightHandTransform;
        public Transform LeftHandTransform;
        public Transform RightFootTransform;
        public Transform LeftFootTransform;
    } // ObjectTransform는 몸이 회전할 때 각 공의 Transform값을 받아오는 구조체이다. 

    void Start()
    {
        Center = this.gameObject;
        RightHand = GameObject.Find("RightHandEffector");
        LeftHand = GameObject.Find("LeftHandEffector");
        RightFoot = GameObject.Find("RightFootEffector");
        LeftFoot = GameObject.Find("LeftFootEffector");
        //위의 이름을 가진 오브젝트들을 변수에 할당해주고 

        objList[0] = Center; objList[1] = RightHand;
        objList[2] = LeftHand; objList[3] = RightFoot; objList[4] = LeftFoot;
        //각 오브젝트들을 objList 배열에 담아준다. 
        
        ObjectTransform pt;
        pt.CenterTransform = objList[0].transform;
        pt.RightHandTransform = objList[1].transform;
        pt.LeftHandTransform = objList[2].transform;
        pt.RightFootTransform = objList[3].transform;
        pt.LeftFootTransform = objList[4].transform;

        disList.Add(pt);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Rotate(Vector3.up, 30f * Time.deltaTime, Space.World);

            ObjectTransform ot;
            ot.CenterTransform = objList[0].transform;
            ot.RightHandTransform = objList[1].transform;
            ot.LeftHandTransform = objList[2].transform;
            ot.RightFootTransform = objList[3].transform;
            ot.LeftFootTransform = objList[4].transform;

            disList.Add(ot);
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Rotate(Vector3.down, 30f * Time.deltaTime, Space.World);

            ObjectTransform ot;
            ot.CenterTransform = objList[0].transform;
            ot.RightHandTransform = objList[1].transform;
            ot.LeftHandTransform = objList[2].transform;
            ot.RightFootTransform = objList[3].transform;
            ot.LeftFootTransform = objList[4].transform;

            disList.Add(ot);
        }

        
    }


    
}


/*
        if (Input.GetKeyDown(KeyCode.D))
        {
            ObjectTransform preTrans;
            preTrans.CenterTransform = objList[0].transform;
            preTrans.RightHandTransform = objList[1].transform;
            preTrans.LeftHandTransform = objList[2].transform;
            preTrans.RightFootTransform = objList[3].transform;
            preTrans.LeftFootTransform = objList[4].transform;

            SetObjectTransform(ref disList,ref preTrans);
            //disList[cnt++] = preTrans;
            Debug.Log("disList[" + (cnt-1) + "] = " + disList[cnt-1].LeftHandTransform.rotation.normalized);
            
            //Debug.Log("disList[" + (cnt) + "] = " + disList[cnt].CenterTransform.eulerAngles);
            //Debug.Log("j-" + j + ": " + disList[j++].LeftHandTransform.position);
        }
        */