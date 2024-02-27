using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBarrel : InteractionObject
{
    [SerializeField]
    GameObject explosionPrefab;

    [SerializeField]
    float explosionDelayTime = 0.3f;

    [SerializeField]
    float explosionRadius = 10.0f;

    [SerializeField]
    float explosionForce = 1000.0f;

    bool isExplode = false;

    public override void TakeDamage(int damage)
    {
        currentHP -= damage;

        if(currentHP < 0 && isExplode == false)
        {
            StartCoroutine("ExplodeBarrel");
        }

    }

    IEnumerator ExplodeBarrel()
    {
        yield return new WaitForSeconds(explosionDelayTime);

        // 근처의 배럴이 터져서 다시 현재 배럴을 터트리려고 할 떄(오버플로우 방지)
        isExplode = true;

        // 폭방 이펙트 생성
        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);

        // 폭발 범위에 있는 모든 오브젝트의 콜라이더 정보 받아와 폭발 효과 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach(Collider hit in colliders)
        {
            // 폭발 범위에 부딧힌 오브젝트가 플레이어일 때 처리
            PlayerController player = hit.GetComponent<PlayerController>();
            if(player != null)
            {
                player.TakeDamage(50);
                continue;
            }

            // 폭발 범위에 부딧힌 오브젝트가 적 캐릭터인 경우
            EnemyStatus enemy = hit.GetComponentInParent<EnemyStatus>();
            if(enemy != null)
            {
                enemy.TakeDamage(300);
                continue;
            }

            // 폭발 범위에 부딧힌 오브젝트가 상호작용 오브젝트인 경우 TakeDamage()로 피해 주기
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if(interaction != null)
            {
                interaction.TakeDamage(300);
            }

            // 중력을 가지고 있는 오브젝트이면 힘을 받아 밀려나도록 설정
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // 배럴 오브젝트 삭제
        Destroy(gameObject);
    }
}
