using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
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
    public float moveSpeed = 5.0f;

    /// <summary>
    /// ȸ������(1 : ��ȸ��, -1 : ��ȸ��, 0 : ����)
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// ȸ�� �ӵ�
    /// </summary>
    public float rotateSpeed = 180.0f;

    /// <summary>
    /// �ִϸ����Ϳ� �ؽð�
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int IsFireHash = Animator.StringToHash("IsFire");

    readonly int OnDieHash = Animator.StringToHash("Die");

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

    private void Awake()
    {
        //inputActions = new PlayerInputActions();
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
        inputActions.Player.Fire.performed += OnFireInput;
        inputActions.Player.Fire.canceled += OnFireInput;
    }

    private void OnDisable()
    {
        inputActions.Player.Fire.canceled -= OnFireInput;
        inputActions.Player.Fire.performed -= OnFireInput;
        inputActions.Player.Jump.performed -= OnJumpInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), !context.canceled);
    }

    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump();
    }

    private void OnFireInput(InputAction.CallbackContext context)
    {
        //animator.SetTrigger(IsFireHash);
        Debug.Log("�߻���!");
    }

    private void Update()
    {
        jumpCoolRemains -= Time.deltaTime;
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
    void SetInput(Vector2 input, bool isMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;

        animator.SetBool(IsMoveHash, isMove);
    }

    /// <summary>
    /// ���� �̵� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void Move()
    {
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

    /// <summary>
    /// ��� ó���� �Լ�
    /// </summary>
    public void Die()
    {
        Debug.Log("�׾���");
    }
}