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
    readonly int IsReloadHash = Animator.StringToHash("IsReload");
    readonly int OnDieHash = Animator.StringToHash("IsDie");

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

    Transform fireTransform;

    private void Awake()
    {
        //inputAction = new PlayerInputActions();
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        GameObject firePos = GameObject.Find("FirePosition");
        fireTransform =  firePos.GetComponent<Transform>();

    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Jump.performed += OnJumpInput;
        inputActions.Player.Fire.performed += OnFireInput;
        inputActions.Player.Fire.canceled += OnFireInput;
        inputActions.Player.Reload.performed += OnReloadInput;
        
    }


    private void OnDisable()
    {
        inputActions.Player.Reload.performed -= OnReloadInput;
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
        Debug.Log("�߻���!");
        //Fire(fireTransform);
        BulleFire();    //
    }

    private void OnReloadInput(InputAction.CallbackContext context)
    {
        if(currentBullet < maxBullet && !isReload)  //
        {
            isReload = true;        //
            StartCoroutine(ReloadBullet()); //
        }
        //Reload();
    }

    private void Reload()
    {
        animator.SetTrigger(IsReloadHash);
    }

    void Fire(Transform fireTransform)
    {
        animator.SetBool("IsFire", true);
        
        //Factory.Instance.GetBullet(fireTransform.position, fireTransform.eulerAngles.z);   // ���丮�� �̿��� �Ѿ� ����
    }

    private void Update()
    {
        jumpCoolRemains -= Time.deltaTime;

        currentDamp -= Time.deltaTime;  //
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
        animator.SetTrigger(OnDieHash);
        Debug.Log("�׾���");
    }

    public float maxBullet; //
    float currentBullet;    //
    public float fireDamp;  //
    float currentDamp;  //
    public float reloadTime;    //
    bool isReload = false;  //
    public GameObject bullet;   //
    public Transform firePos;   //

    void BulleFire()    //
    {
        if(currentDamp <= 0 && currentBullet > 0 && !isReload)  //
        {
            currentDamp = fireDamp; //
            currentDamp--;  //

            Instantiate(bullet, firePos.position, firePos.rotation);    //
        }
        else if(currentBullet <= 0 && !isReload)    //
        {
            isReload = true;//
           StartCoroutine(ReloadBullet());  //
        }
    }
    IEnumerator ReloadBullet()  //
    {
        animator.SetTrigger(IsReloadHash);  //
        for(float i = reloadTime; i > 0; i -= 0.1f) //
        {
            yield return new WaitForSeconds(0.1f);  //
        }
        isReload = false;   //
        currentBullet = maxBullet;  //
    }

    private void Start()    //
    {
        currentBullet = maxBullet;  //
        currentDamp = 0;    //
    }
}