using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �̵��� �����ϴ� ����ũ��Ʈ
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    /// <summary>
    ///  �̵� �ӵ�
    /// </summary>
    [SerializeField]
    float moveSpeed = 0.0f;

    /// <summary>
    /// �̵� ����
    /// </summary>
    [SerializeField]
    Vector3 moveDirection = Vector3.zero;

    private void Update()
    {
        transform.position += Time.deltaTime * moveSpeed * moveDirection;
    }

    /// <summary>
    /// �̵� ������ ������ �Լ�
    /// </summary>
    /// <param name="direction">�̵� ����</param>
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
