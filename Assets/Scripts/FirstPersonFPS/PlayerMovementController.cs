using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovementController : RotateMouse
{
    PlayerInputActionFPS inputAction;
    Rigidbody rigid;
    Animator animator;

    /// <summary>
    /// ���� �̵��ӵ�
    /// </summary>
    float currentSpeed = 0.0f;

    /// <summary>
    /// �ȴ� �̵� �ӵ�
    /// </summary>
    public float walkSpeed = 3.0f;

    /// <summary>
    /// �ٴ� �̵� �ӵ�
    /// </summary>
    public float runSpeed = 5.0f;

    /// <summary>
    /// ���� �޸��� ������ Ȯ���ϱ� ���� ���� true : �޸��� �� false : �ȴ���
    /// </summary>
    bool isRun = false;
    bool isWalk = false;

    readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");


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
        animator = transform.GetChild(1).GetComponent<Animator>();
        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        JumpCoolRemains -= Time.deltaTime;  // ���� ��Ÿ�� ���̱�
    }

    private void FixedUpdate()
    {
        MoveUD();   // �յ� �����̱�
        MoveRL();   // �翷 �����̱�
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += OnMoveStart;
        inputAction.Player.Move.canceled += OnMoveEnd;
        inputAction.Player.Jump.performed += OnJumpInput;
        inputAction.Player.Run.performed += OnRunStart;
        inputAction.Player.Run.canceled += OnRunEnd;
    }



    private void OnDisable()
    {
        inputAction.Player.Run.canceled -= OnRunEnd;
        inputAction.Player.Run.performed -= OnRunStart;
        inputAction.Player.Jump.performed -= OnJumpInput;
        inputAction.Player.Move.canceled -= OnMoveEnd;
        inputAction.Player.Move.performed -= OnMoveStart;
        inputAction.Player.Disable();
    }

    /// <summary>
    /// �����̴� �Լ� ����
    /// </summary>
    /// <param name="context"></param>
    private void OnMoveStart(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>());  // ���� Vector2 ��
        animator.SetFloat(MoveSpeed, 0.5f);   // IsMove �ؽ��� �ֱ�
        isWalk = true;
    }
    private void OnMoveEnd(InputAction.CallbackContext context)
    {
        isWalk = false;
        animator.SetFloat(MoveSpeed, 0);
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
    /// ���� �յڷ� �̵� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void MoveUD()
    {
        // ���� �����̱�
        if (isRun == true)
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;

        rigid.MovePosition(rigid.position + Time.deltaTime * currentSpeed * moveUpDown * transform.forward);
    }

    /// <summary>
    /// ���� �翷�� �̵� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void MoveRL()
    {
        currentSpeed = walkSpeed;
        rigid.MovePosition(rigid.position + Time.deltaTime * currentSpeed * moveRightLeft * transform.right);
    }

    /// <summary>
    /// ���� �����ϴ� �Լ� ����
    /// </summary>
    /// <param name="_"></param>
    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump(); // ���� ó�� ȣ��
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

    /// <summary>
    /// �޸��� ����
    /// </summary>
    /// <param name="context"></param>
    private void OnRunStart(InputAction.CallbackContext context)
    {
        isRun = true;
        isWalk = false;
        animator.SetFloat(MoveSpeed, 1.0f);   // IsMove �ؽ��� �ֱ�
    }

    /// <summary>
    ///  �޸��� ��
    /// </summary>
    /// <param name="context"></param>
    private void OnRunEnd(InputAction.CallbackContext context)
    {
        isRun = false;
        if(isWalk)
        {
            animator.SetFloat(MoveSpeed, 0.5f);   // IsMove �ؽ��� �ֱ�
        }
        else if(!isWalk)
        {
            animator.SetFloat(MoveSpeed, 0.0f);
        }
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


    

}
