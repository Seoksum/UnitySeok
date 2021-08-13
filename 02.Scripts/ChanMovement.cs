using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//사용자 입력에 따라 플레이어 캐릭터를 움직이는 스크립트
public class ChanMovement : MonoBehaviour
{
    public float moveSpeed = 5f; //앞뒤 움직임의 속도
    public float rotateSpeed = 180f; //좌우 회전 속도
    bool isMove;


 
    private Rigidbody playerRigidbody; //플레이어 캐릭터의 리지드바디
    private Animator playerAnimator; //플레이어 캐릭터의 애니메이터

    // Start is called before the first frame update 
    void Start()
    {

        //사용할 컴포넌트의 참조 가져오기
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    //FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    private void FixedUpdate()
    {
        playerAnimator.SetBool("isRun", false);    
        float rotate = Input.GetAxis("Horizontal");

        Rotate();
        Move();
    }


    //입력값에 따라 캐릭터를 앞뒤로 움직임
    private void Move()
    {
        
        float move = Input.GetAxis("Vertical");
        Vector3 moveDistance = move * transform.forward * moveSpeed * Time.deltaTime;
        
        if (moveDistance != new Vector3(0, 0, 0))
        {
            playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
            playerAnimator.SetBool("isRun", true);
        }

    }
    //입력값에 따라 캐릭터를 좌우로 회전
    private void Rotate()
    {
        float rotate = Input.GetAxis("Horizontal");
        //리지드바디를 이용해 게임 오브젝트 회전 변경
        float turn = rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0f);
    }
}