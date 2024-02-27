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

        // ��ó�� �跲�� ������ �ٽ� ���� �跲�� ��Ʈ������ �� ��(�����÷ο� ����)
        isExplode = true;

        // ���� ����Ʈ ����
        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);

        // ���� ������ �ִ� ��� ������Ʈ�� �ݶ��̴� ���� �޾ƿ� ���� ȿ�� ó��
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach(Collider hit in colliders)
        {
            // ���� ������ �ε��� ������Ʈ�� �÷��̾��� �� ó��
            PlayerController player = hit.GetComponent<PlayerController>();
            if(player != null)
            {
                player.TakeDamage(50);
                continue;
            }

            // ���� ������ �ε��� ������Ʈ�� �� ĳ������ ���
            EnemyStatus enemy = hit.GetComponentInParent<EnemyStatus>();
            if(enemy != null)
            {
                enemy.TakeDamage(300);
                continue;
            }

            // ���� ������ �ε��� ������Ʈ�� ��ȣ�ۿ� ������Ʈ�� ��� TakeDamage()�� ���� �ֱ�
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if(interaction != null)
            {
                interaction.TakeDamage(300);
            }

            // �߷��� ������ �ִ� ������Ʈ�̸� ���� �޾� �з������� ����
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // �跲 ������Ʈ ����
        Destroy(gameObject);
    }
}
