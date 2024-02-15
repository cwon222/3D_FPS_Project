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
    /// 이동 방향(1 : 전진, -1 : 후진, 0 : 정지)
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 5.0f;

    /// <summary>
    /// 회전방향(1 : 우회전, -1 : 좌회전, 0 : 정지)
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotateSpeed = 180.0f;

    /// <summary>
    /// 애니메이터용 해시값
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int IsFireHash = Animator.StringToHash("IsFire");
    readonly int IsReloadHash = Animator.StringToHash("IsReload");
    readonly int OnDieHash = Animator.StringToHash("IsDie");

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
        Debug.Log("발사함!");
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
        
        //Factory.Instance.GetBullet(fireTransform.position, fireTransform.eulerAngles.z);   // 팩토리를 이용해 총알 생성
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
    /// 이동 입력 처리용 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    /// <param name="isMove">이동 중이면 true, 이동 중이 아니면 false</param>
    void SetInput(Vector2 input, bool isMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;

        animator.SetBool(IsMoveHash, isMove);
    }

    /// <summary>
    /// 실제 이동 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Move()
    {
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

    /// <summary>
    /// 사망 처리용 함수
    /// </summary>
    public void Die()
    {
        animator.SetTrigger(OnDieHash);
        Debug.Log("죽었음");
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