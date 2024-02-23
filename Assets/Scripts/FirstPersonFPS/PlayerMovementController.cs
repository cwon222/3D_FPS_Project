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
    /// �̵� �ӵ�
    /// </summary>
    public float moveSpeed = 5.0f;

    /// <summary>
    /// ������
    /// </summary>
    public float jumpPower = 6.0f;

    /// <summary>
    /// ���߿� �� �ִ��� �ƴ��� ��Ÿ���� ����
    /// </summary>
    bool InAir
    {
        get => GroundCount < 1; // groundCount�� 1���� ������ ����
    }

    /// <summary>
    /// �����ϰ� �ִ� "Ground"�±� ������Ʈ�� ����Ȯ�� �� ������ ������Ƽ
    /// </summary>
    int GroundCount
    {
        get => groundCount;
        set
        {
            if (groundCount < 0) // 0���ϸ� 0���� ����
            {
                groundCount = 0;
            }
            groundCount = value;
            if (groundCount < 0) // ������ ���� 0 ���ϸ� 0
            {
                groundCount = 0;
            }
        }
    }
    /// <summary>
    /// �����ϰ� �ִ� "Ground"�±� ������Ʈ�� ����
    /// </summary>
    int groundCount = 0;

    /// <summary>
    /// ���� �� Ÿ��
    /// </summary>
    public float jumpCoolTime = 5.0f;

    /// <summary>
    /// �����ִ� ��Ÿ��
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
    /// ������ �������� Ȯ���ϴ� ������Ƽ(�������� �ƴϰ� ����ְ� ��Ÿ���� �� ������.)
    /// </summary>
    bool IsJumpAvailable => !InAir && (JumpCoolRemains < 0.0f) && isAlive;


    /// �÷��̾� ��������
    /// </summary>
    bool isAlive = true;

    /// <summary>
    /// �÷��̾ ������ �˸��� ��������Ʈ
    /// </summary>
    public Action onDie;

    

    /// <summary>
    /// ������ ����Ǿ��� �� ���۵� ��������Ʈ
    /// </summary>
    public Action<float> onHpChange;
    
    /// <summary>
    /// �յ� ����
    /// </summary>
    float moveUpDown;
    /// <summary>
    /// �翷 ����
    /// </summary>
    float moveRightLeft;

    private void Awake()
    {
        inputAction = new();    // ��ǲ �׼� ���� �ʱ�ȭ
        rigid = GetComponent<Rigidbody>();  // �����庯�� �ʱ�ȭ

    }

    private void Update()
    {
        JumpCoolRemains -= Time.deltaTime;  // ���� ��Ÿ�� ���̱�
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
    /// �����̴� �Լ� ����
    /// </summary>
    /// <param name="context"></param>
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>());  // ���� Vector2 ��
    }

    /// <summary>
    /// �̵� �Է� ó���� �Լ�
    /// </summary>
    /// <param name="input">�Էµ� ����</param>
    void SetInput(Vector2 input)
    {
        moveRightLeft = input.x;  // �޿�
        moveUpDown = input.y;    // �յ�
    }

    /// <summary>
    /// ���� �̵� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void MoveUD()
    {
        // ���� �����̱�
        rigid.MovePosition(rigid.position + Time.deltaTime * moveSpeed * moveUpDown * transform.forward);
    }

    void MoveRL()
    {
        rigid.MovePosition(rigid.position + Time.deltaTime * moveSpeed * moveRightLeft * transform.right);
    }

    /// <summary>
    /// ���� �����ϴ� �Լ� ����
    /// </summary>
    /// <param name="_"></param>
    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump(); // ���� ó�� ȣ��
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))   // �ݶ��̴��� ���� ����� �±װ� Ground �̸�
        {
            GroundCount++;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))   // �ݶ��̴��� ���� ����� �±װ� Ground �̸�
        {
            GroundCount--;
        }
    }


    /// <summary>
    /// ���� ���� ó���ϴ� �Լ�
    /// </summary>
    void Jump()
    {
        // ForceMode.Force : ������ �̴� ��, ForceMode.Impulse : �ѹ��� �̴� ��
        if (IsJumpAvailable) // ������ ������ ���� ����
        {
            rigid.AddForce(jumpPower * Vector3.up, ForceMode.Impulse); // �������� jumpPower��ŭ ���� ���ϱ� World ��������
            JumpCoolRemains = jumpCoolTime; // ��Ÿ�� �ʱ�ȭ
            //GroundCount = 0; // ��� ������ ��������
        }
    }

}
