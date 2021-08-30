using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightTest : MonoBehaviour
{
    GameObject sphere;
        
    Vector3 dis;
    float y;
    float diffX, diffY, diffZ;

    Vector3 nowPos;

    void Start()
    {
        sphere = GameObject.Find("Characters");
        y = this.transform.localPosition.y;
        //dis = Vector3.Distance(this.transform.position,sphere.transform.position);
        
        dis = sphere.transform.position - this.transform.position;
        diffX = dis.x; diffY = 0.13f; diffZ = dis.z;

    }


    void Update()
    {
        
        if (Input.GetKey(KeyCode.D))
        {
            // this.transform.RotateAround(sphere.transform.position, Vector3.up, 30f * Time.deltaTime);

            if (this.gameObject.name == "RightHand")
            {
                Quaternion q = MoveRotation.disList[0].CenterTransform.rotation;
                this.transform.rotation = q;
                this.transform.position = MoveRotation.disList[0].RightHandTransform.position;
            }

            if (this.gameObject.name == "LeftHand")
            {
                Quaternion q = MoveRotation.disList[0].CenterTransform.rotation;
                this.transform.rotation = q;
                this.transform.position = MoveRotation.disList[0].LeftHandTransform.position;
            }
            if (this.gameObject.name == "RightFoot")
            {
                Quaternion q = MoveRotation.disList[0].CenterTransform.rotation;
                this.transform.rotation = q;
                this.transform.position = MoveRotation.disList[0].RightFootTransform.position;
            }

            if (this.gameObject.name == "LeftFoot")
            {
                Quaternion q = MoveRotation.disList[0].CenterTransform.rotation;
                this.transform.rotation = q;
                this.transform.position = MoveRotation.disList[0].LeftFootTransform.position;
            }

            
        }

        if (Input.GetKey(KeyCode.A))
        {
            // this.transform.RotateAround(sphere.transform.position, Vector3.up, 30f * Time.deltaTime);

            if (this.gameObject.name == "RightHand")
            {
                Quaternion q = MoveRotation.disList[0].CenterTransform.rotation;
                this.transform.rotation = q;
                this.transform.position = MoveRotation.disList[0].RightHandTransform.position;
            }

            if (this.gameObject.name == "LeftHand")
            {
                Quaternion q = MoveRotation.disList[0].CenterTransform.rotation;
                this.transform.rotation = q;
                this.transform.position = MoveRotation.disList[0].LeftHandTransform.position;
            }
            if (this.gameObject.name == "RightFoot")
            {
                Quaternion q = MoveRotation.disList[0].CenterTransform.rotation;
                this.transform.rotation = q;
                this.transform.position = MoveRotation.disList[0].RightFootTransform.position;
            }

            if (this.gameObject.name == "LeftFoot")
            {
                Quaternion q = MoveRotation.disList[0].CenterTransform.rotation;
                this.transform.rotation = q;
                this.transform.position = MoveRotation.disList[0].LeftFootTransform.position;
            }


        }
        
    }

}
