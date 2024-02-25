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
    /// 애니메이터를 컨트롤할 변수
    /// </summary>
    /// <param name="stateNmae"></param>
    /// <param name="layer"></param>
    /// <param name="normalizedTime"></param>
    public void Play(string stateNmae, int layer, float normalizedTime)
    {
        animator.Play(stateNmae, layer, normalizedTime);
    }
}
