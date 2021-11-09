using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
    
    public enum State
    {
        Idle,
        Move,
        Attack,
        AttackWait,
        Dead
    }

    public State currentState = State.Idle;

    Vector3 curTargetPos;       //마우스 클릭 지점(목적지 좌표)
    
    GameObject curEnemy;

    public float rotAnglePerSecond = 360f;
    public float moveSpeed = 2f;
    float atttackDelay = 2f;
    float attackTimer = 0f;
    float attackDistance = 1.5f;
    float chaseDistance = 2.5f;

    PlayerAni myAni;

    void Start()
    {
        myAni = GetComponent<PlayerAni>();
        ChangeState(State.Idle, PlayerAni.ANI_IDLE);
    }
    public void AttackEnemy(GameObject enemy)
    {
        if(curEnemy !=null && curEnemy == enemy) { return; }
        curEnemy = enemy;
        curTargetPos = curEnemy.transform.position;
        ChangeState(State.Move, PlayerAni.ANI_WALK);
    }

    void ChangeState(State newState,int aniNumber)
    {
        if (currentState == newState)
            return;

        myAni.ChangeAni(aniNumber);
        currentState = newState;
    }
    // Update is called once per frame
    void UpdateState()
    {
        switch(currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Move:
                MoveState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.AttackWait:
                AttackWaitState();
                break;
            case State.Dead:
                DeadState();
                break;
            default:
                break;
        }
    }
    void IdleState() { }
    void MoveState() { 
        TurnToDestination();
        MoveToDestination();
    }
    void AttackState() 
    {
        attackTimer = 0f;
        transform.LookAt(curTargetPos);
        ChangeState(State.AttackWait, PlayerAni.ANI_ATKIDLE);
    }
    void AttackWaitState() 
    {
        if (attackTimer > atttackDelay)
            ChangeState(State.Attack, PlayerAni.ANI_ATTACK);
        attackTimer += Time.deltaTime;
    }
    void DeadState() { }

    public void MoveTo(Vector3 tpos)
    {
        curEnemy = null;
        curTargetPos = tpos;
        ChangeState(State.Move, PlayerAni.ANI_WALK);
    }
    void TurnToDestination()
    {
        Quaternion lookRotation = Quaternion.LookRotation(curTargetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotAnglePerSecond);
    }
    void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, curTargetPos, moveSpeed * Time.deltaTime);
        if (curEnemy == null)
        {
            if (transform.position == curTargetPos)
            {
                ChangeState(State.Idle, PlayerAni.ANI_IDLE);
            }
        }
        else if(Vector3.Distance(transform.position, curTargetPos)<attackDistance)
        {
            ChangeState(State.Attack, PlayerAni.ANI_ATTACK);
        }

               
    }
    
    void Update()
    {
        UpdateState();
    }
}
