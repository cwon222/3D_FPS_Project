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
    /// �̵� ����(1 : ����, -1 : ����, 0 : ����)
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// ���� �̵� �ӵ�
    /// </summary>
    float currentMoveSpeed = 3.0f;

    /// <summary>
    /// �ȴ� �ӵ�
    /// </summary>
    float walkSpeed = 3.0f;

    /// <summary>
    /// �޸��� �ӵ�
    /// </summary>
    public float runSpeed = 5.0f;

    /// <summary>
    /// ȸ������(1 : ��ȸ��, -1 : ��ȸ��, 0 : ����)
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// ȸ�� �ӵ�
    /// </summary>
    public float rotateSpeed = 180.0f;

    /// <summary>
    /// ������
    /// </summary>
    public float jumpPower = 6.0f;

    /// <summary>
    /// ���� ������ �ƴ��� ��Ÿ���� ����
    /// </summary>
    bool isJumping = false;

    /// <summary>
    /// ���� �� Ÿ��
    /// </summary>
    public float jumpCoolTime = 5.0f;

    /// <summary>
    /// �����ִ� ��Ÿ��
    /// </summary>
    float jumpCoolRemains = -1.0f;

    /// <summary>
    /// ������ �������� Ȯ���ϴ� ������Ƽ(�������� �ƴϰ� ��Ÿ���� �� ������.)
    /// </summary>
    bool IsJumpAvailable => !isJumping && (jumpCoolRemains < 0.0f);

    readonly int IsWalkHash = Animator.StringToHash("IsWalk");
    readonly int IsRunHash = Animator.StringToHash("IsRun");
    readonly int IsReloadHash = Animator.StringToHash("IsReload");
    readonly int IsAimHash = Animator.StringToHash("IsAim");

    bool isAim = false;

    /// <summary>
    /// ���� ��ũ���� �ִ���
    /// </summary>
    bool isCrouching = false;

    private void Awake()
    {
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnWalkStart;
        inputActions.Player.Move.canceled += OnWalkStart;
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
        inputActions.Player.Move.canceled -= OnWalkStart;
        inputActions.Player.Move.performed -= OnWalkStart;
        inputActions.Player.Disable();
    }

    /// <summary>
    /// �÷��̾� �̵�
    /// </summary>
    /// <param name="context"></param>
    private void OnWalkStart(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), walkSpeed);
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="_"></param>
    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump();
    }

    /// <summary>
    /// �޸��� ����
    /// </summary>
    /// <param name="context"></param>
    private void OnRunStart(InputAction.CallbackContext context)
    {
        animator.SetFloat(IsRunHash, runSpeed);
        currentMoveSpeed = runSpeed; // �޸��� ����
    }
    /// <summary>
    /// �޸��� ��
    /// </summary>
    /// <param name="context"></param>
    private void OnRunEnd(InputAction.CallbackContext context)
    {
        animator.SetFloat(IsRunHash, walkSpeed);
        currentMoveSpeed = walkSpeed; // �޸��� ��
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="context"></param>
    private void OnReloadInput(InputAction.CallbackContext context)
    {
        animator.SetTrigger(IsReloadHash); // ����
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="context"></param>
    private void OnAimStart(InputAction.CallbackContext context)
    {
        isAim = true;
        animator.SetBool(IsAimHash, isAim);  // aim ���� ����
    }

    /// <summary>
    /// ���� ��
    /// </summary>
    /// <param name="context"></param>
    private void OnAimEnd(InputAction.CallbackContext context)
    {
        isAim = false;
        animator.SetBool(IsAimHash, isAim);  // aim ���� ��
    }

    /// <summary>
    /// �ѽ�� ����
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnFireEnd(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();    // �Ѿ� �߻� ����
    }

    /// <summary>
    /// �ѽ�� ��
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnFireStart(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();    // �Ѿ� �߻� ����
    }

    /// <summary>
    /// ��ũ���� ��
    /// </summary>
    /// <param name="context"></param>
    private void OnCrouchEnd(InputAction.CallbackContext context)
    {
        isCrouching = true;
        if(isCrouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z); // �÷��̾� ���� ũ��� �ٲٱ�
        }
        
    }
    /// <summary>
    /// ��ũ���� ����
    /// </summary>
    /// <param name="context"></param>
    private void OnCrouchStart(InputAction.CallbackContext context)
    {
        isCrouching = false;
        if(!isCrouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, 0.6f, transform.localScale.z); // �÷��̾� Y������ ũ�� ���̱�
        }
        
    }

    private void Update()
    {
        jumpCoolRemains -= Time.deltaTime; // ���� ��Ÿ�� ���̱�
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
    /// �̵� �Է� ó���� �Լ�
    /// </summary>
    /// <param name="input">�Էµ� ����</param>
    /// <param name="isMove">�̵� ���̸� true, �̵� ���� �ƴϸ� false</param>
    void SetInput(Vector2 input, float isMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;
        
        animator.SetFloat(IsWalkHash, isMove);
        animator.SetFloat(IsRunHash, isMove);
    }

    /// <summary>
    /// ���� �̵� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void Move()
    {
        //walkSpeed = rigid.velocity.magnitude;
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * currentMoveSpeed * moveDirection * transform.forward);
    }

    /// <summary>
    /// ���� ȸ�� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void Rotate()
    {
        // �̹� fixedUpdate���� �߰��� ȸ���� ȸ��(delta)
        Quaternion rotate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);

        // ���� ȸ������ rotate��ŭ �߰��� ȸ��
        rigid.MoveRotation(rigid.rotation * rotate);
    }
    /// <summary>
    /// ���� ���� ó���� �ϴ� �Լ�
    /// </summary>
    void Jump()
    {
        if (IsJumpAvailable) // ������ ������ ���� ����
        {
            rigid.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);  // �������� jumpPower��ŭ ���� ���ϱ�
            jumpCoolRemains = jumpCoolTime; // ��Ÿ�� �ʱ�ȭ
            isJumping = true;               // �����ߴٰ� ǥ��
        }
    }

}
