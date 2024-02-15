using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : RecycleObject
{
    // 플레이어: 걸을때 모션, 줌 모션, 줌 해제 모션, 웅크리기 모션
    // 플레이어: 뛰기 가만히 걷기 총쏘기 앉기 줌 줌해제 장전
    // 총알: 총알 생성 발사 반동 발사시 이펙트  
    // 적: 총알 맞으면 피격, 총알 피 깍이거나 죽음, 플레이어 자동 추적 or 랜덤 이동, 범위안에 플레이어가 보이면 총알 발사

    // 시작하자마자 계속 오른쪽으로 초속 7초로 움직이기
    public float bulletSpeed = 7.0f;

    /// <summary>
    /// 총알의 수명
    /// </summary>
    public float lifeTime = 10.0f;


    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(LifeOver(lifeTime)); // 코루틴 실행
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * bulletSpeed * Vector2.right);     // 총알 이동 // 총 곱한 수는 4번
    }

    private void OnCollisionEnter(Collision collision)  // 충돌시 실행 함수
    {
        //Factory.Instance.GetHitEffect(transform.position);

        gameObject.SetActive(false);    // 비활성화 -> 풀로 되돌린다
    }
}
