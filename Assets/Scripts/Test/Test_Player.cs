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
    /// �̵� ����(1 : ����, -1 : ����, 0 : ����)
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// �̵� �ӵ�
    /// </summary>
    public float moveSpeed = 5.0f;

    /// <summary>
    /// ���� �̵��ӵ�
    /// </summary>
    float currentMoveSpeed = 5.0f;

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

    /// <summary>
    /// �����̱� ���� �ִϸ����Ϳ� �ؽ��� 
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    /// <summary>
    /// ����� �ϱ����� �ִϸ����Ϳ� �ؽ���
    /// </summary>
    readonly int IsFireHash = Animator.StringToHash("IsFire");

    readonly int IsDieHash = Animator.StringToHash("Die");
    /// <summary>
    /// �÷��̾� ��������
    /// </summary>
    bool isAlive = true;

    /// <summary>
    /// �÷��̾ ������ �˸��� ��������Ʈ
    /// </summary>
    public Action onDie;

    /// <summary>
    /// �������� ���� �÷��̾��� ����
    /// </summary>
    public float maxHp = 10.0f;

    /// <summary>
    /// ���� �÷��̾��� ����
    /// </summary>
    float hp = 0.0f;

    /// <summary>
    /// �÷��̾��� ������ Ȯ���ϰ� �����ϱ� ���� ������Ƽ
    /// </summary>
    public float HP
    {
        get => hp;
        set
        {
            // �߰� �ڵ� �ʿ�
            hp = value;
            if (hp < 0.0f)
            {
                hp = 0.0f; // ������ �� �Ǿ����� 0.0���� ���� ���� �� ���ó��
                Die(); // �÷��̾� ����
            }
            onHpChange?.Invoke(hp / maxHp); // ���� ���� ������ �˸�
        }
    }

    /// <summary>
    /// ������ ����Ǿ��� �� ���۵� ��������Ʈ
    /// </summary>
    public Action<float> onHpChange;

    private void Awake()
    {
        //inputAction = new PlayerInputActions();
        inputAction = new();    // ��ǲ �׼� ���� �ʱ�ȭ
        rigid = GetComponent<Rigidbody>();  // �����庯�� �ʱ�ȭ
        animator = GetComponent<Animator>(); // �ִϸ����� �ʱ�ȭ

        ItemUseChecker checker = GetComponentInChildren<ItemUseChecker>();  // ItemUseChecker ã��
        checker.onItemUse += (interacable) => interacable.Use();    // ���ٽ����� intercable�� Use ����

    }
    private void Start()
    {
        currentMoveSpeed = moveSpeed;   // ���� �ӵ��� ���ؼӵ��� �ʱⰪ ����
        hp = maxHp;
    }

    private void Update()
    {
        JumpCoolRemains -= Time.deltaTime;  // ���� ��Ÿ�� ���̱�
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
    /// �����̴� �Լ� ����
    /// </summary>
    /// <param name="context"></param>
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), !context.canceled);  // ���� Vector2 ��, �Է��� ��Ұ� �ƴҶ�
    }

    /// <summary>
    /// ���� �����ϴ� �Լ� ����
    /// </summary>
    /// <param name="_"></param>
    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump(); // ���� ó�� ȣ��
    }

    private void OnFireInput(InputAction.CallbackContext context)
    {
        animator.SetTrigger(IsFireHash); // �ִϸ������� IsUse �ؽ��� ����
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
    /// �����̴� ��ü�� ��ž������ �� ����� �Լ�
    /// </summary>
    /// <param name="delta">������ ��</param>
    void OnRideMovingobject(Vector3 delta)
    {
        rigid.MovePosition(rigid.position + delta);
    }

    /// <summary>
    /// �̵� �Է� ó���� �Լ�
    /// </summary>
    /// <param name="input">�Էµ� ����</param>
    /// <param name="isMove">�̵��ߴ���(�̵��� : true, ���� : false)</param>
    void SetInput(Vector2 input, bool isMove)
    {
        rotateDirection = input.x;  // ȸ�� ����
        moveDirection = input.y;    // �����̴�

        animator.SetBool(IsMoveHash, isMove);   // IsMove �ؽ��� �ֱ�
    }

    /// <summary>
    /// ���� �̵� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void Move()
    {
        // ���� �����̱�
        rigid.MovePosition(rigid.position + Time.deltaTime * currentMoveSpeed * moveDirection * transform.forward);
    }

    /// <summary>
    /// ���� ȸ�� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void Rotate()
    {
        // �̹� fixedUpdate ���� �߰��� ȸ���� ȸ��(delta)
        Quaternion rotate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);
        rigid.MoveRotation(rigid.rotation * rotate); // ���� ȸ������ rotate ��ŭ �߰� ȸ��
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
    /// ��� ó���� �Լ�
    /// </summary>
    public void Die()
    {
        if (isAlive)
        {
            Debug.Log("�׾���");
            animator.SetTrigger(IsDieHash); // �״� �ִϸ��̼� ȣ��
            inputAction.Disable(); // �Է� ó�� ����
            Transform head = transform.GetChild(0); // �Ӹ� ��ġ ��ã��
            rigid.constraints = RigidbodyConstraints.None; // ���� ����� ���� �����ϱ�
            rigid.AddForceAtPosition(-transform.forward, head.position, ForceMode.Impulse); // �Ӹ���ġ���� ���༭ �б�
            rigid.AddTorque(transform.up * 1.5f, ForceMode.Impulse); // ȸ�� ���۵���
            onDie?.Invoke(); // �׾��ٰ� ��ȣ������(onDie ��������Ʈ ����)
            isAlive = false; // ������ ǥ��
                             // �״� �ִϸ��̼� ����
                             // ���̻� �������� ����
                             // ���� ����(�ڷ� �Ѿ�鼭 y������ ������ �Ѵ�.)
                             // �׾��ٰ� ��ȣ������(onDie ��������Ʈ ����)
        }

    }
}