using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRotate : MonoBehaviour
{ 
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
            this.transform.Rotate(Vector3.forward, 30.0f * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.A))
            this.transform.Rotate(Vector3.back, 30f * Time.deltaTime, Space.World);
    }
}
