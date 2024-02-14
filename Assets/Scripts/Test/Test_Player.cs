using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Player : MonoBehaviour
{
    PlayerInputActions inputAction;
    Rigidbody rigid;
    Animator animator;

    /// <summary>
    /// 이동 방향(1 : 전진, -1 : 후진, 0 : 정지)
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 5.0f;

    /// <summary>
    /// 현재 이동속도
    /// </summary>
    float currentMoveSpeed = 5.0f;

    /// <summary>
    /// 회전방향(1 : 우회전, -1 : 좌회전, 0 : 정지)
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotateSpeed = 180.0f;

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

    /// <summary>
    /// 움직이기 위한 애니메이터용 해쉬값 
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    /// <summary>
    /// 사용을 하기위한 애니메이터용 해쉬값
    /// </summary>
    readonly int IsFireHash = Animator.StringToHash("IsFire");

    readonly int IsDieHash = Animator.StringToHash("Die");
    /// <summary>
    /// 플레이어 생존여부
    /// </summary>
    bool isAlive = true;

    /// <summary>
    /// 플레이어가 죽으면 알리는 델리게이트
    /// </summary>
    public Action onDie;

    /// <summary>
    /// 시작했을 때의 플레이어의 수명
    /// </summary>
    public float maxHp = 10.0f;

    /// <summary>
    /// 현재 플레이어의 수명
    /// </summary>
    float hp = 0.0f;

    /// <summary>
    /// 플레이어의 수명을 확인하고 설정하기 위한 프로퍼티
    /// </summary>
    public float HP
    {
        get => hp;
        set
        {
            // 추가 코드 필요
            hp = value;
            if (hp < 0.0f)
            {
                hp = 0.0f; // 수명ㅇ이 다 되었으면 0.0으로 숫자 정리 및 사망처리
                Die(); // 플레이어 죽음
            }
            onHpChange?.Invoke(hp / maxHp); // 현재 수명 비율을 알림
        }
    }

    /// <summary>
    /// 수명이 변경되었을 때 실핼될 델리게이트
    /// </summary>
    public Action<float> onHpChange;

    private void Awake()
    {
        //inputAction = new PlayerInputActions();
        inputAction = new();    // 인풋 액션 변수 초기화
        rigid = GetComponent<Rigidbody>();  // 리지드변수 초기화
        animator = GetComponent<Animator>(); // 애니메이터 초기화

        ItemUseChecker checker = GetComponentInChildren<ItemUseChecker>();  // ItemUseChecker 찾기
        checker.onItemUse += (interacable) => interacable.Use();    // 람다식으로 intercable의 Use 실행

    }
    private void Start()
    {
        currentMoveSpeed = moveSpeed;   // 현재 속도를 기준속도로 초기값 설정
        hp = maxHp;
    }

    private void Update()
    {
        JumpCoolRemains -= Time.deltaTime;  // 점프 쿨타임 줄이기
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += OnMoveInput;
        inputAction.Player.Move.canceled += OnMoveInput;
        inputAction.Player.Jump.performed += OnJumpInput;
        inputAction.Player.Fire.performed += OnFireInput;
        inputAction.Player.Fire.canceled += OnFireInput;
    }

    private void OnDisable()
    {
        inputAction.Player.Fire.canceled -= OnFireInput;
        inputAction.Player.Fire.performed -= OnFireInput;
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
        SetInput(context.ReadValue<Vector2>(), !context.canceled);  // 읽은 Vector2 값, 입력이 취소가 아닐때
    }

    /// <summary>
    /// 실제 점프하는 함수 실행
    /// </summary>
    /// <param name="_"></param>
    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump(); // 점프 처리 호출
    }

    private void OnFireInput(InputAction.CallbackContext context)
    {
        animator.SetTrigger(IsFireHash); // 애니메이터의 IsUse 해쉬값 변경
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
    /// 움직이는 물체에 ㅌ탑승했을 때 연결될 함수
    /// </summary>
    /// <param name="delta">움직인 양</param>
    void OnRideMovingobject(Vector3 delta)
    {
        rigid.MovePosition(rigid.position + delta);
    }

    /// <summary>
    /// 이동 입력 처리용 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    /// <param name="isMove">이동했는지(이동중 : true, 멈춤 : false)</param>
    void SetInput(Vector2 input, bool isMove)
    {
        rotateDirection = input.x;  // 회전 각도
        moveDirection = input.y;    // 움직이는

        animator.SetBool(IsMoveHash, isMove);   // IsMove 해쉬값 넣기
    }

    /// <summary>
    /// 실제 이동 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Move()
    {
        // 방향 움직이기
        rigid.MovePosition(rigid.position + Time.deltaTime * currentMoveSpeed * moveDirection * transform.forward);
    }

    /// <summary>
    /// 실제 회전 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Rotate()
    {
        // 이번 fixedUpdate 에서 추가로 회전할 회전(delta)
        Quaternion rotate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);
        rigid.MoveRotation(rigid.rotation * rotate); // 원래 회전에서 rotate 만큼 추가 회전
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

    /// <summary>
    /// 사망 처리용 함수
    /// </summary>
    public void Die()
    {
        if (isAlive)
        {
            Debug.Log("죽었음");
            animator.SetTrigger(IsDieHash); // 죽는 애니메이션 호출
            inputAction.Disable(); // 입력 처리 끄기
            Transform head = transform.GetChild(0); // 머리 위치 ㅈ찾기
            rigid.constraints = RigidbodyConstraints.None; // 물리 잠금을 전부 해제하기
            rigid.AddForceAtPosition(-transform.forward, head.position, ForceMode.Impulse); // 머리위치에서 힘줘서 밀기
            rigid.AddTorque(transform.up * 1.5f, ForceMode.Impulse); // 회전 빙글돌기
            onDie?.Invoke(); // 죽었다고 신호보내기(onDie 델리게이트 실행)
            isAlive = false; // 죽은거 표시
                             // 죽는 애니메이션 실행
                             // 더이상 움직이지 못함
                             // 빙글 돈다(뒤로 넘어가면서 y축으로 스핀을 한다.)
                             // 죽었다고 신호보내기(onDie 델리게이트 실행)
        }

    }
}