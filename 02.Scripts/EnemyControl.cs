using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public Transform[] points;
    public int nextIdx = 1;
    public float speed = 3.0f;
    public float damping = 5.0f;

    private Transform tr;
    private Transform playerTr;
    public Transform rightHandObj = null;
    public Transform lookObj = null;

    private Vector3 movePos;
    private bool isAttack = false;
    public bool ikActive = true;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        tr = GetComponent<Transform>();
        points = GameObject.Find("WayPointGroup").GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(tr.position, playerTr.position);

        if (dist <= 2.0f) {
            isAttack = true;
            //anim.SetBool("isUpper", true);
        } 
        else if (dist <= 5.0f)
        {
            movePos = playerTr.position;
            isAttack = false;
           // anim.SetBool("isUpper", false);
        }
        else
        {
            movePos = points[nextIdx].position; isAttack = false;
            
        }
        anim.SetBool("isAttack", isAttack);
        

        if (!isAttack)
        {
            Quaternion rot = Quaternion.LookRotation(movePos - tr.position);//가야할위치-현재위치를 회전각도로 바꿔준대 
            tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * damping);
            tr.Translate(Vector3.forward * Time.deltaTime * speed);
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "WAY_POINT")
        {
            nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
        }
    }
    void OnAnimatorIK()
    {
        if (anim)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

            }
            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                anim.SetLookAtWeight(0);
            }
        }
    }
}
