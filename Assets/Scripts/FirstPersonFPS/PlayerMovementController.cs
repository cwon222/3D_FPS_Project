using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovementController : RotateMouse
{
    PlayerInputActions inputAction;
    Rigidbody rigid;


    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 5.0f;

    /// <summary>
    /// 점프력
    /// </summary>
    public float jumpPower = 6.0f;

    /// <summary>
    /// 공중에 떠 있는지 아닌지 나타내는 변수
    /// </summary>
    bool InAir
    {
        get => GroundCount < 1; // groundCount가 1보다 작으면 공중
    }

    /// <summary>
    /// 접촉하고 있는 "Ground"태그 오브젝트의 개수확인 및 설정용 프로퍼티
    /// </summary>
    int GroundCount
    {
        get => groundCount;
        set
        {
            if (groundCount < 0) // 0이하면 0에서 설정
            {
                groundCount = 0;
            }
            groundCount = value;
            if (groundCount < 0) // 설정한 값이 0 이하면 0
            {
                groundCount = 0;
            }
        }
    }
    /// <summary>
    /// 접촉하고 있는 "Ground"태그 오브젝트의 개수
    /// </summary>
    int groundCount = 0;

    /// <summary>
    /// 점프 쿨 타임
    /// </summary>
    public float jumpCoolTime = 5.0f;

    /// <summary>
    /// 남아있는 쿨타임
    /// </summary>
    float jumpCoolRemains = -1.0f;

    float JumpCoolRemains
    {
        get => jumpCoolRemains;
        set
        {
            jumpCoolRemains = value;
            onJumpCoolTimeChange?.Invoke(jumpCoolRemains / jumpCoolTime);
        }
    }

    public Action<float> onJumpCoolTimeChange;

    /// <summary>
    /// 점프가 가능한지 확인하는 프로퍼티(점프중이 아니고 살아있고 쿨타임이 다 지났다.)
    /// </summary>
    bool IsJumpAvailable => !InAir && (JumpCoolRemains < 0.0f) && isAlive;


    /// 플레이어 생존여부
    /// </summary>
    bool isAlive = true;

    /// <summary>
    /// 플레이어가 죽으면 알리는 델리게이트
    /// </summary>
    public Action onDie;

    

    /// <summary>
    /// 수명이 변경되었을 때 실핼될 델리게이트
    /// </summary>
    public Action<float> onHpChange;
    
    /// <summary>
    /// 앞뒤 방향
    /// </summary>
    float moveUpDown;
    /// <summary>
    /// 양옆 방향
    /// </summary>
    float moveRightLeft;

    private void Awake()
    {
        inputAction = new();    // 인풋 액션 변수 초기화
        rigid = GetComponent<Rigidbody>();  // 리지드변수 초기화

    }

    private void Update()
    {
        JumpCoolRemains -= Time.deltaTime;  // 점프 쿨타임 줄이기
    }

    private void FixedUpdate()
    {
        MoveUD();
        MoveRL();
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += OnMoveInput;
        inputAction.Player.Move.canceled += OnMoveInput;
        inputAction.Player.Jump.performed += OnJumpInput;
    }

    private void OnDisable()
    {
        inputAction.Player.Jump.performed -= OnJumpInput;
        inputAction.Player.Move.canceled -= OnMoveInput;
        inputAction.Player.Move.performed -= OnMoveInput;
        inputAction.Player.Disable();
    }

    /// <summary>
    /// 움직이는 함수 실행
    /// </summary>
    /// <param name="context"></param>
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>());  // 읽은 Vector2 값
    }

    /// <summary>
    /// 이동 입력 처리용 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    void SetInput(Vector2 input)
    {
        moveRightLeft = input.x;  // 왼오
        moveUpDown = input.y;    // 앞뒤
    }

    /// <summary>
    /// 실제 이동 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void MoveUD()
    {
        // 방향 움직이기
        rigid.MovePosition(rigid.position + Time.deltaTime * moveSpeed * moveUpDown * transform.forward);
    }

    void MoveRL()
    {
        rigid.MovePosition(rigid.position + Time.deltaTime * moveSpeed * moveRightLeft * transform.right);
    }

    /// <summary>
    /// 실제 점프하는 함수 실행
    /// </summary>
    /// <param name="_"></param>
    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump(); // 점프 처리 호출
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))   // 콜라이더에 닿은 대상의 태그가 Ground 이면
        {
            GroundCount++;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))   // 콜라이더에 닿은 대상의 태그가 Ground 이면
        {
            GroundCount--;
        }
    }


    /// <summary>
    /// 실제 점프 처리하는 함수
    /// </summary>
    void Jump()
    {
        // ForceMode.Force : 서서히 미는 힘, ForceMode.Impulse : 한번에 미는 힘
        if (IsJumpAvailable) // 점프가 가능할 때만 점프
        {
            rigid.AddForce(jumpPower * Vector3.up, ForceMode.Impulse); // 위쪽으로 jumpPower만큼 힘을 더하기 World 기준으로
            JumpCoolRemains = jumpCoolTime; // 쿨타임 초기화
            //GroundCount = 0; // 모든 땅에서 떨어졌음
        }
    }

}
