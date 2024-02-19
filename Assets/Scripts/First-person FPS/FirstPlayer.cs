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
    /// �̵� �ӵ�
    /// </summary>
    public float moveSpeed = 3.0f;

    /// <summary>
    /// ���� �̵��ӵ�
    /// </summary>
    float currentSpeed = 3.0f;

    /// <summary>
    /// �ִ� �̵��ӵ�
    /// </summary>
    public float maxSpeed;

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
        moveSpeed = 5.0f; // �޸��� ����
    }
    private void OnRunEnd(InputAction.CallbackContext context)
    {
        moveSpeed = 3.0f; // �޸��� ��
    }

    private void OnReloadInput(InputAction.CallbackContext context)
    {
        animator.SetBool(IsReloadHash, true); // ���� z�ڷ�ƾ����
    }

    private void OnAimStart(InputAction.CallbackContext context)
    {
        animator.SetBool(IsAimHash, true);  // aim ���� ����
    }

    private void OnAimEnd(InputAction.CallbackContext context)
    {
        animator.SetBool(IsAimHash, false);  // aim ���� ��
    }

    private void OnFireEnd(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();    // �Ѿ� �߻� ����
    }

    private void OnFireStart(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();    // �Ѿ� �߻� ����
    }

    private void OnCrouchEnd(InputAction.CallbackContext context)
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.6f, 1), Time.deltaTime * 15);    // ��ũ���� ����
    }

    private void OnCrouchStart(InputAction.CallbackContext context)
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 15);    // ��ũ���� ��
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
        //currentSpeed = rigid.velocity.magnitude;
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveSpeed * moveDirection * transform.forward);
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
