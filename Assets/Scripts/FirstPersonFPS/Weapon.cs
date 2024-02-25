using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// 무기 설정을 위한 변수
    /// </summary>
    public WeaponSetting weaponSetting;

    /// <summary>
    /// 마지막 발사 시간 체크용 변수
    /// </summary>
    float lastAttackTime = 0.0f;

    /// <summary>
    /// 발사 애니메이션을 제어 하기위한 변수
    /// </summary>
    PlayerAnimatorController animator;

    private void Awake()
    {
        animator = GetComponentInParent<PlayerAnimatorController>(); // 부모 오브젝트에 있는 플레이어 오브젝트에 있는 PlayerAnimatorController 찾기
    }

    /// <summary>
    /// 공격시 실행할 함수
    /// </summary>
    /// <param name="type">받은 값</param>
    public void StartWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 (공격 시작)
        if (type == 0)
        {
            // 연속 공격
            if(weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine(OnAttackLoop()); // 연사 공격 코루틴 시작
            }
            // 단발 공격
            else
            {
                OnAttack(); // 단발 공격 함수 호출
            }
        }
    }

    /// <summary>
    /// 연사 공격 종료 함수
    /// </summary>
    /// <param name="type">받은 값</param>
    public void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽  클릭 (공격 종료)
        if(type == 0)
        {
            StopCoroutine(OnAttackLoop());  // 연사 공격 코루틴 정지
        }
    }

    /// <summary>
    /// 연사 공격시 실행할 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator OnAttackLoop()
    {
        while(true) // 계속
        {
            OnAttack();  // 단발 공격

            yield return null;
        }
    }

    /// <summary>
    /// 단발 공격시 실행할 함수
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnAttack()
    {
        // 현재 시간 - 마지막 발사 시간 > 공격 속도
        if (Time.deltaTime - lastAttackTime > weaponSetting.attackRate) 
        {
            // 뛰고 있을 때는 공격 불가능
            if(animator.MoveSpeed > 0.5) // 뛰고 있는 애니메이션이 재생 중이면
            {
                return; // 뛰고 있을 때는 반환
            }

            // 공격주기가 되어야 공격할 수 있도록 하기 위한 현재 시간 저장할 함수
            lastAttackTime = Time.time;

            // 무기 애니메이션 재생
            animator.Play("Fire", -1, 0); // 같은 애니메이션을 반복할 때 애니메이션을 끊고 처음부터 다시 재생
            //animator.Play("Fire"); // 같은 애니메이션을 반복할 때 중간에 끊지 못하고 재생 완료 후 다시 재생
        }
    }
}


