using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    /// <summary>
    /// 애니메이터 컴포넌트 변수
    /// </summary>
    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();  // 플레이어 오브젝트의 자식 오브젝트에 Animator 컴포넌트 찾기
    }

    /// <summary>
    /// 애니메이터 해쉬값 변경용 프로퍼티
    /// </summary>
    public float MoveSpeed
    {
        set => animator.SetFloat("movementSpeed", value); // 애니메이터 파라미터 값을 value 설정
        get => animator.GetFloat("movementSpeed");          // 애니메이터 파라미터 값을 반환
    }

    /// <summary>
    /// 재장전 애니메이션 실행하는 함수
    /// </summary>
    public void OnReload()
    {
        animator.SetTrigger("onReload"); // 재장전 애니메이션 해쉬값 설정
    }

    /// <summary>
    /// 에임(줌) 하면 애니메이터 해쉬값 변경용 프로퍼티
    /// </summary>
    public bool AimModeIs
    {
        set => animator.SetBool("isAimMode", value);
        get => animator.GetBool("isAimMode");
    }

    /// <summary>
    /// 애니메이터를 컨트롤할 변수
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="layer"></param>
    /// <param name="normalizedTime"></param>
    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }

    /// <summary>
    /// 애니메이션이 현재 진행 중인지 확인하고 결과를 반환하는 함수
    /// </summary>
    /// <param name="name">반환할 애니메이션 이름</param>
    /// <returns></returns>
    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}
