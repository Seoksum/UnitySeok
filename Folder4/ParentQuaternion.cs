using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentQuaternion : MonoBehaviour
{
    Vector3 rot;
    float moveSpeed = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        rot = this.transform.eulerAngles;
        
    }

    // Update is called once per frame
    void Update()
    {
        this.moveSpeed *=0.96f;      

        if (Input.GetKey(KeyCode.Alpha1))
        {
            moveSpeed = 5.0f;
            rot += new Vector3(90f, 0, 0) * Time.deltaTime * moveSpeed;
            this.transform.rotation = Quaternion.Euler(rot);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            this.moveSpeed = 5.0f;
            rot += new Vector3(0, 90f, 0) * Time.deltaTime * moveSpeed;
            this.transform.rotation = Quaternion.Euler(rot);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            this.moveSpeed = 5.0f;
            rot += new Vector3(0,0,90f) * Time.deltaTime * moveSpeed;
            this.transform.rotation = Quaternion.Euler(rot);
        }
    }   
}
