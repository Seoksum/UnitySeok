using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentQuaternion : MonoBehaviour
{
    Vector3 rot;
    // Start is called before the first frame update
    void Start()
    {
        rot = this.transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        rot += new Vector3(90, 0, 0) * Time.deltaTime;
        this.transform.rotation = Quaternion.Euler(rot);
        //transform.RotateAround(Vector3.zero, Vector3.up, 100f * Time.deltaTime);
    }
}
