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
        rot += new Vector3(90, 0, 0) * Time.deltaTime;
        this.transform.rotation = Quaternion.Euler(rot);
    }
}
