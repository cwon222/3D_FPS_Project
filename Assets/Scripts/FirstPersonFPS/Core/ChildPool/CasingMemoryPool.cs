using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    /// <summary>
    /// 탄피 오브젝트
    /// </summary>
    [SerializeField]
    GameObject casingPrefab;

    /// <summary>
    /// 탄피 메모리풀
    /// </summary>
    MemoryPool memoryPool;

    private void Awake()
    {
        memoryPool = new MemoryPool(casingPrefab); // 메모리풀 변수에 메모리 할당
    }

    /// <summary>
    /// 탄피 스폰하는 함수
    /// </summary>
    /// <param name="position">위치</param>
    /// <param name="direction">방향</param>탄
    public void SpawnCasing(Vector3 position, Vector3 direction)
    {
        GameObject item = memoryPool.ActivePoolItem();  // 게임 오브젝트 하나 화성화
        item.transform.position = position; // 위치 지정
        item.transform.rotation = Random.rotation;  // 각도 지정(랜덤)
        item.GetComponent<Casing>().Setup(memoryPool, direction); // Casing 컴포넌트 찾아 방향 지정
    }
}
