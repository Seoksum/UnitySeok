using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildQuaternion : MonoBehaviour
{
    Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        origin = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = origin;
    }
}
