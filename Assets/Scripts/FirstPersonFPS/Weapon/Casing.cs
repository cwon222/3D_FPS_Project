using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    /// <summary>
    /// 탄피 등장 후 비활성화 되는 시간
    /// </summary>
    [SerializeField]
    float deactivateTime = 5.0f;

    /// <summary>
    /// 탄피가 회전하는 속력 계수
    /// </summary>
    [SerializeField]
    float casingSpin = 1.0f;

    Rigidbody rigid;
    MemoryPool memoryPool;

    /// <summary>
    /// 오브젝트의 이동속도와 각도 설정할 함수
    /// </summary>
    /// <param name="pool">관리할 오브젝트</param>
    /// <param name="direction">방향</param>
    public void Setup(MemoryPool pool, Vector3 direction)
    {
        rigid = GetComponent<Rigidbody>();
        memoryPool = pool;

        // 탄피의 이동 속도과 회전 속도 설정
        rigid.velocity = new Vector3(direction.x, 1.0f, direction.z);
        rigid.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                            Random.Range(-casingSpin, casingSpin),
                                            Random.Range(-casingSpin, casingSpin));

        // 탄피 자동 비활성화를 위한 코루틴 실행
        StartCoroutine("DeactivateAfterTime");
    }

    /// <summary>
    /// 비활성활 시키기 위한 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime); // deactivateTime 기다리기

        memoryPool.DeactivatePoolItem(this.gameObject); // 기다린 후 비활성화
    }
}
