using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType
{
    Normal = 0, // 벽, 바닥
    Obstacle,   // 장애물
    Enemy       // 적
}

/// <summary>
/// 타격 이펙트를 메모리 풀로 관리하는 스크립트
/// </summary>
public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// 피격 이펙트 프리펩 담을 배열 변수
    /// </summary>
    GameObject[] impactPrefab;

    /// <summary>
    /// 피격 이펙트 메모리 풀들 담을 배열
    /// </summary>
    MemoryPool[] memoryPool;

    private void Awake()
    {
        
        memoryPool = new MemoryPool[impactPrefab.Length];   // 프리펩들의 개수 만큼 메모리풀 생성
        for(int i = 0; i < impactPrefab.Length; ++i)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]); // 종류 별로 메모리풀에 담기
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        // 부딪힌 오브젝트의 Tag 정보에 따라 다르게 처리
        if(hit.transform.CompareTag("ImpactNormal")) // 벽이나 바닥에 닿으면
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal)); // 피격 이펙트 생성
        }
        else if (hit.transform.CompareTag("ImpactObstacle")) // 장애물에 닿으면
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal)); // 피격 이펙트 생성
        }
        else if(hit.transform.CompareTag("ImpactEnemy")) // 적에게 닿으면
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal)); // 피격 이펙트 생성
        }
    }

    /// <summary>
    /// 피격 이펙트를 생성하는 함수
    /// </summary>
    /// <param name="type">피격 이펙트 타입</param>
    /// <param name="position">생성 위치</param>
    /// <param name="rotation">회전</param>
    private void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation)
    {
        GameObject item = memoryPool[(int)type].ActivePoolItem();   // 오브젝트 활성화
        item.transform.position = position;                         // 위치 설정
        item.transform.rotation = rotation;                         // 회전 설ㅇ정
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);   // 메모리 풀 찾기
    }
}
