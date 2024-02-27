using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어를 관리하는 스크립트
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 달리기 키 설정
    /// </summary>
    KeyCode keyCodeRun = KeyCode.LeftShift;

    /// <summary>
    /// 점프 키 설정
    /// </summary>
    KeyCode keyCodeJump = KeyCode.Space;

    /// <summary>
    /// 재장전 키 설정
    /// </summary>
    KeyCode keyCodeReload = KeyCode.R;

    /// <summary>
    /// 마우스 이동으로 카메라 회전
    /// </summary>
    RotateMouse rotateToMouse;

    /// <summary>
    /// 키보드 입력으로 플레이어 이동, 점프
    /// </summary>
    PlayerMovementController movement; 

    /// <summary>
    /// 플레이어의 이동속도 정보
    /// </summary>
    Status status;

    /// <summary>
    /// 플레이어 에니메이션 제어 하기 위해 찾을 컴포넌트 변수
    /// </summary>
    PlayerAnimatorController animator;

    /// <summary>
    /// 무기를 사용해서 공격을 제어하기 위해 컴포넌트를 찾기 위한 변수
    /// </summary>
    Weapon weapon;


    private void Awake()
    {
        // 마우스 커서를 보이지 않게 설정
        Cursor.visible = false;
        // 현재 위치에 고정
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateMouse>();            // RotateMouse 컴포넌트 찾기
        movement = GetComponent<PlayerMovementController>();    // PlayerMovementController 컴포넌트 찾기
        status = GetComponent<Status>();                        // Status 컴포넌트 찾기
        animator = GetComponent<PlayerAnimatorController>();    // PlayerAnimatorController 컴포넌트 찾기
        weapon = GetComponentInChildren<Weapon>();              // 자식 오브젝트 안에 있는 Weapon 컴포넌트 찾기
    }

    private void Update()
    {
        UpdateRotate();         // 마우스 이동
        UpdateMove();           // 플레이어 이동 실행
        UpdateJump();           // 점프 실행
        UpdateWeaponAction(); ; // 공격 실행
    }


    /// <summary>
    /// 마우스의 X,Y의 이동값을 받기 위한 함수
    /// </summary>
    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X"); // 마우스 X의 이동 값
        float mouseY = Input.GetAxis("Mouse Y"); // 마우스 Y의 이동 값

        rotateToMouse.UpdateRotate(mouseX, mouseY); // 이동한 마우스의 X,Y의 값을 매개변수로 넣기
    }

    /// <summary>
    /// 플레이어 이동 값 받아 이동 실제 이동 시키는 함수
    /// </summary>
    void UpdateMove()
    {
        float x = Input.GetAxis("Horizontal");  // 앞뒤 값 받기
        float z = Input.GetAxis("Vertical");    // 양옆 값 받기

        if (x != 0 || z != 0) // 이동 중일 때(걷기 아니면 뛰기)
        {
            bool isRun = false; //달리기 버튼을 누르면 ture 아니면 false

            // 옆이나 뒤로 이동할 떄는 달릴 수 없음
            if (z > 0) // 앞으로 이동 중일 때
            {
                isRun = Input.GetKey(keyCodeRun); // 쉬프트 키를 눌른거에 따라 true 또는 false
            }
            // isRunning == true 이면 RunSpeed(뛰는 속도) // isRunning == false 이면 WalkSpeed (걷는 속도)
            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            // 달리는 중이면 애니메이터 MoveSpeed 해쉬값 1(뛰는 애니메이션)
            // 걷는 중이면 애니메이터 MoveSpeed 해쉬값 0.5(걷는 애니메이션)
            animator.MoveSpeed = isRun == true ? 1 : 0.5f; 
        }
        else // 멈춰 있을 때
        {
            movement.MoveSpeed = 0; // 이동속도 0
            animator.MoveSpeed = 0; // 애니메이터 MoveSpeed 해쉬값 0(Idle 애니메이션)
        }

        movement.MoveTo(new Vector3(x, 0, z));  // 받은 이동 값에 따라 플레이어 이동
    }

    /// <summary>
    /// 플레이어가 실제 점프 시키는 함수
    /// </summary>
    void UpdateJump()
    {
        if(Input.GetKey(keyCodeJump)) // 스페이스 바 누르면
        {
            movement.jump(); // 점프 실행
        }
    }

    /// <summary>
    /// 플레이어가 실제 공격을 실행 시키는 함수
    /// </summary>
    void UpdateWeaponAction()
    {
        if(Input.GetMouseButtonDown(0)) // 마우스 버튼을 누르면
        {
            weapon.StartWeaponAction(); // 공격 시작 함수 실행
        }
        else if(Input.GetMouseButtonUp(0)) // 마우스 버튼을 떼면
        {
            weapon.StopWeaponAction();  // 공격 중지 함수 실행
        }

        if(Input.GetMouseButtonDown(1)) // 마우스 오른쪽 버튼 누르면 
        {
            weapon.StartWeaponAction(1);    // 에임 모드 시작
        }
        else if( Input.GetMouseButtonUp(1)) // 마우스 오른쪽 버튼 뗴면
        {
            weapon.StopWeaponAction(1);     // 에임 모드 중지
        }

        if(Input.GetKeyDown(keyCodeReload)) // R키 누르면
        {
            weapon.StartReload();       // 재장전 함수 실행
        }
    }

    /// <summary>
    /// 플레이어 데미지 받을 때 실행할 함수
    /// </summary>
    /// <param name="damage">받은 데미지 </param>
    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if(isDie)
        {
            Debug.Log("플레이어 죽음");
        }
    }
}
