using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 플레이어의 이동을 관리하는 스크립트
// 이 명령이 포함된 스크립트를 게임 오브젝트에 컴포넌트로 적용하면 해당 컴포넌트도 같이 추가 된다
[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// 이동 힘(x, z와 y축을 별도로 계산해 실제 이동에 적용)
    /// </summary>
    Vector3 moveForce;

    /// <summary>
    /// 점프 힘
    /// </summary>
    public float jumpForce;

    /// <summary>
    /// 중력 값
    /// </summary>
    public float gravity;

    /// <summary>
    /// 이동속도를 제어하기 위한 프로퍼티
    /// </summary>
    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value); // 이동 속도는 음수가 안되게 설정
        get => moveSpeed;
    }

    /// <summary>
    /// 플레이어 이동을 제어하기 위한 컴포넌트
    /// </summary>
    CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>(); // 컴포넌트 찾기
    }

    private void Update()
    {
        // 게임 오브젝트의 발이 바닥과 충돌 체크(출동하면 true)
        if (!characterController.isGrounded) // 허공에 떠있으면 
        {
            moveForce.y += gravity * Time.deltaTime; // 중력만큼 y축 이동속도 감소
        }
        characterController.Move(Time.deltaTime * moveForce); // 프레임당 moveForce 만큼 이동
    }

    public void MoveTo(Vector3 direction)
    {

        // 이동 방향 = 캐릭터의 회전 값 * 방향 값
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // 이동 힘 = 이동 방향 * 속도
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void jump()
    {
        // 플레이어가 바닥에 있을 때만 점프 가능
        if(characterController.isGrounded)
        {
            moveForce.y = jumpForce; // y축으로 중력만큼 증가
        }
    }
}
