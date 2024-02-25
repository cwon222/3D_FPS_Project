using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타격 이펙트를 제어하는 스크립트
/// </summary>
public class Impact : MonoBehaviour
{
    /// <summary>
    /// 파티클 시스템 변수
    /// </summary>
    ParticleSystem particle;

    /// <summary>
    /// 메모리풀 변수
    /// </summary>
    MemoryPool memoryPool;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>(); // 파티클 컴포넌트 찾기
    }

    /// <summary>
    /// 메모리 풀로 관리하는 함수
    /// </summary>
    /// <param name="pool">관리할 메모리</param>
    public void Setup(MemoryPool pool)
    {
        memoryPool = pool;
    }

    private void Update()
    {
        if(particle.isPlaying == false) // 파티클이 재생중이 아니면
        {
            memoryPool.DeactivatePoolItem(gameObject); // 삭제
        }    
    }
}
