using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터의 상태를 관리하는 스크립트
/// </summary>
public class Status : MonoBehaviour
{
    /// <summary>
    /// 걷는 속도
    /// </summary>
    [SerializeField]
    float walkSpeed;

    /// <summary>
    /// 뛰는 속도
    /// </summary>
    [SerializeField]
    float runSpeed;

    /// <summary>
    /// 외부에서 걷는 속도를 확인하기 위한 프로퍼티
    /// </summary>
    public float WalkSpeed => walkSpeed;

    /// <summary>
    /// 외부에서 뛰는 속도를 확인하기 위한 프로퍼티
    /// </summary>
    public float RunSpeed => runSpeed;
}
