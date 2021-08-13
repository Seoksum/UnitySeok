using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MoveCtrl : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    Vector3 movePos = Vector3.zero;
    
    private Animator anim;
    private NavMeshAgent nv;
    
    private Transform tr;
    private Transform enemyTr;
    bool isDamaged;
    // Start is called before the first frame update
    void Start()
    {
        enemyTr = GameObject.FindWithTag("Enemy").GetComponent<Transform>();
        nv = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();//현재 player의 위치
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(tr.position, enemyTr.position);
        if (dist <= 2.0f)
            isDamaged = true;
        else
            isDamaged = false;

        anim.SetBool("isDamaged", isDamaged);

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);

        if (Input.GetMouseButtonDown(1) && Physics.Raycast(ray, out hit, 100f, 1 << 8))
        {
            movePos = hit.point;
            Debug.Log(movePos); // movePos는 마우스 커서 찍은 위치 
            anim.SetBool("isRun", true);
            nv.SetDestination(movePos);
            nv.isStopped = false;
        }
        if(nv.remainingDistance <=0.2f && nv.velocity.magnitude >= 0.2f)
        {
            anim.SetBool("isRun", false);
        }
        /*if ((tr.position - movePos).sqrMagnitude >= 0.2f * 0.2f) //이동거리가 0.2유닛보다 작으면?
        {
            Quaternion rot = Quaternion.LookRotation(movePos - tr.position);
            tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * 8f);
            tr.Translate(Vector3.forward * Time.deltaTime * 3.0f);
            anim.SetBool("isRun", true);
        }
        else
            anim.SetBool("isRun", false);*/

        if (Input.GetKey(KeyCode.K))
        {
            SceneManager.LoadSceneAsync("Scene02");
        }

    }
}
