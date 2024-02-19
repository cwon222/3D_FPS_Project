using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPlayer : MonoBehaviour
{
    PlayerInputActions inputActions;
    Rigidbody rigid;
    Animator animator;

    /// <summary>
    /// 이동 방향(1 : 전진, -1 : 후진, 0 : 정지)
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 3.0f;

    /// <summary>
    /// 현재 이동속도
    /// </summary>
    float currentSpeed = 3.0f;

    /// <summary>
    /// 최대 이동속도
    /// </summary>
    public float maxSpeed;

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
    /// 점프 중인지 아닌지 나타내는 변수
    /// </summary>
    bool isJumping = false;

    /// <summary>
    /// 점프 쿨 타임
    /// </summary>
    public float jumpCoolTime = 5.0f;

    /// <summary>
    /// 남아있는 쿨타임
    /// </summary>
    float jumpCoolRemains = -1.0f;

    /// <summary>
    /// 점프가 가능한지 확인하는 프로퍼티(점프중이 아니고 쿨타임이 다 지났다.)
    /// </summary>
    bool IsJumpAvailable => !isJumping && (jumpCoolRemains < 0.0f);

    readonly int IsWalkHash = Animator.StringToHash("Walk Speed");
    readonly int IsRunHash = Animator.StringToHash("Max Speed");
    readonly int IsReloadHash = Animator.StringToHash("Reloading");
    readonly int IsAimHash = Animator.StringToHash("Aiming");

    private void Awake()
    {
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Jump.performed += OnJumpInput;
        inputActions.Player.Run.performed += OnRunStart;
        inputActions.Player.Run.canceled += OnRunEnd;
        inputActions.Player.Aim.performed += OnAimStart;
        inputActions.Player.Aim.canceled += OnAimEnd;
        inputActions.Player.Reload.performed += OnReloadInput;
        inputActions.Player.Fire.performed += OnFireStart;
        inputActions.Player.Fire.canceled += OnFireEnd;
        inputActions.Player.Crouch.performed += OnCrouchStart;
        inputActions.Player.Crouch.canceled += OnCrouchEnd;
    }

    

    private void OnDisable()
    {
        inputActions.Player.Crouch.canceled -= OnCrouchEnd;
        inputActions.Player.Crouch.performed -= OnCrouchStart;
        inputActions.Player.Fire.canceled -= OnFireEnd;
        inputActions.Player.Fire.performed -= OnFireStart;
        inputActions.Player.Reload.performed -= OnReloadInput;
        inputActions.Player.Aim.canceled -= OnAimEnd;
        inputActions.Player.Aim.performed -= OnAimStart;
        inputActions.Player.Run.canceled -= OnRunEnd;
        inputActions.Player.Run.performed -= OnRunStart;
        inputActions.Player.Jump.performed -= OnJumpInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), currentSpeed);
    }

    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump();
    }

    private void OnRunStart(InputAction.CallbackContext context)
    {
        moveSpeed = 5.0f; // 달리기 시작
    }
    private void OnRunEnd(InputAction.CallbackContext context)
    {
        moveSpeed = 3.0f; // 달리기 끝
    }

    private void OnReloadInput(InputAction.CallbackContext context)
    {
        animator.SetBool(IsReloadHash, true); // 장전 z코루틴쓰기
    }

    private void OnAimStart(InputAction.CallbackContext context)
    {
        animator.SetBool(IsAimHash, true);  // aim 조준 시작
    }

    private void OnAimEnd(InputAction.CallbackContext context)
    {
        animator.SetBool(IsAimHash, false);  // aim 조준 끝
    }

    private void OnFireEnd(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();    // 총알 발사 시작
    }

    private void OnFireStart(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();    // 총알 발사 중지
    }

    private void OnCrouchEnd(InputAction.CallbackContext context)
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.6f, 1), Time.deltaTime * 15);    // 웅크리기 시작
    }

    private void OnCrouchStart(InputAction.CallbackContext context)
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 15);    // 웅크리기 끝
    }

    private void Update()
    {
        jumpCoolRemains -= Time.deltaTime;
        currentSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    /// <summary>
    /// 이동 입력 처리용 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    /// <param name="isMove">이동 중이면 true, 이동 중이 아니면 false</param>
    void SetInput(Vector2 input, float isMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;
        
        animator.SetFloat(IsWalkHash, isMove);
        animator.SetFloat(IsRunHash, isMove);
    }

    /// <summary>
    /// 실제 이동 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Move()
    {
        //currentSpeed = rigid.velocity.magnitude;
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveSpeed * moveDirection * transform.forward);
    }

    /// <summary>
    /// 실제 회전 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Rotate()
    {
        // 이번 fixedUpdate에서 추가로 회전할 회전(delta)
        Quaternion rotate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);

        // 현재 회전에서 rotate만큼 추가로 회전
        rigid.MoveRotation(rigid.rotation * rotate);
    }
    /// <summary>
    /// 실제 점프 처리를 하는 함수
    /// </summary>
    void Jump()
    {
        if (IsJumpAvailable) // 점프가 가능할 때만 점프
        {
            rigid.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);  // 위쪽으로 jumpPower만큼 힘을 더하기
            jumpCoolRemains = jumpCoolTime; // 쿨타임 초기화
            isJumping = true;               // 점프했다고 표시
        }
    }

}
