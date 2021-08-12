using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;

    private Vector3 movePos = Vector3.zero;
    private Vector3 moveDir = Vector3.zero;

    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                movePos = hit.point;
                moveDir = movePos - transform.position;
            }
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.black);
        }
        if (movePos != Vector3.zero)
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, moveDir, rotateSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.position = Vector3.MoveTowards(transform.position, movePos, moveSpeed * Time.deltaTime);
            anim.SetBool("isRun", true);
        }
        else
            anim.SetBool("isRun", false);
        float dis = Vector3.Distance(transform.position, movePos);
        if (dis <= 0.3f)
        {
            movePos = Vector3.zero;
        }

    }
}
