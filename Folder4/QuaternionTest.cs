using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionTest : MonoBehaviour
{
    Vector3 rot;
    private Ray ray;
    private RaycastHit hit;
    protected float moveSpeed = 2f;

    void Start()
    {
        rot = this.transform.eulerAngles;       
    }

    void Update()
    {

        rotate();

    }
    protected void rotate()
    {
        rot += new Vector3(0, 0, 90f) * Time.deltaTime;
        this.transform.rotation = Quaternion.Euler(rot);
    }
    
}
