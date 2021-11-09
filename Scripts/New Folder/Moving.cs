using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{

    public float moveSpeed = 10.0f;
    public float rotationSpeed = 5.0f;
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        // 중력해제
        body.useGravity = false;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
      float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v);

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (90.0f > angle && angle > -90.0f)
            {
                angle = angle * rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up, angle);
            }
        }

        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}