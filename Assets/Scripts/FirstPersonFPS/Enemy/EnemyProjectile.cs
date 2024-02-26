using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� �߻�ü�� �����ϴ� ��ũ��Ʈ
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    /// <summary>
    ///  �� �̵� ��ũ��Ʈ
    /// </summary>
    EnemyMovement movement;

    /// <summary>
    /// �� �߻�ü�� �ִ� ��Ÿ�
    /// </summary>
    float projectileDistance = 30.0f;

    /// <summary>
    /// �̵��� ������ �Լ�
    /// </summary>
    /// <param name="position">�������� ����</param>
    public void Setup(Vector3 position)
    {
        movement = GetComponent<EnemyMovement>();

        StartCoroutine("OnMove", position); // 
    }

    /// <summary>
    /// �̵� ���� ����, �̵� �ʰ� Ȯ�ο� �ڷ�ƾ
    /// </summary>
    /// <param name="targetPosition">Ÿ���� ��ġ</param>
    /// <returns></returns>
    IEnumerator OnMove(Vector3 targetPosition)
    {
        // ���� ��ġ
        Vector3 start = transform.position;

        // �̵� ������ ����(Ÿ���� ��ġ - �ڽ��� ��ġ)����ȭ
        movement.MoveTo((targetPosition - transform.position).normalized);

        while (true)
        {
            if(Vector3.Distance(transform.position, start) >= projectileDistance)
            {
                // �ڽ��� ��ġ�� ���� ��ġ�� �Ÿ��� �� �߻�ü�� �ִ� ��Ÿ� ���� ũ��
                Destroy(gameObject); // ������Ʈ �ı�

                yield break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))  // ���� ����� �±װ� Player �̸�
        {
            Debug.Log("�÷��̾� ����");

            Destroy(gameObject); // ������Ʈ �ı�
        }
    }
}