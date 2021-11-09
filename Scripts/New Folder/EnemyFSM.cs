using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chase,
        Attack,
        Dead,
        NoState
    }
    public State currentState = State.Idle;
    EnemyAni myAni;
    Transform player;

    float chaseDistance = 5;
    float attackDistance = 2.5f;
    float reChaseDistance = 3;
    public float rotAnglePerSecond = 360f;
    public float moveSpeed = 2f;
    float attackDelay = 2f;
    float attackTimer = 0f;
   


    void Start()
    {
        myAni = GetComponent<EnemyAni>();
        ChangeState(State.Idle, EnemyAni.IDLE);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void UpdateState()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Chase:
                ChaseState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Dead:
                DeadState();
                break;
            case State.NoState:
                NoState();
                break;
        }
    }
    void ChangeState(State newState, string aniName)
    {
        if (currentState == newState)
            return;

        myAni.ChangeAni(aniName);
        currentState = newState;
    }
    void IdleState() 
    {
        if (GetDistanceFromPlayer() < chaseDistance)
            ChangeState(State.Chase, EnemyAni.WALK);
    }
    void ChaseState()
    {
        if (GetDistanceFromPlayer()<attackDistance)
        {
            ChangeState(State.Attack, EnemyAni.ATTACK);
        }
        else
        {
            TurnToDestination();
            MoveToDestination();
        }
    }
    void AttackState()
    {
        if (GetDistanceFromPlayer()>reChaseDistance)
        {
            attackTimer = 0f;
            ChangeState(State.Chase, EnemyAni.WALK);
        }
        else
        {
            if (attackTimer > attackDelay)
            {
                transform.LookAt(player.position);
                myAni.ChangeAni(EnemyAni.ATTACK);
                attackTimer = 0;
            }
            attackTimer += Time.deltaTime;
        }
    }
    void DeadState() { }
    void NoState() { }

    void TurnToDestination()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotAnglePerSecond);
    }
    void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }
    float GetDistanceFromPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }
}
