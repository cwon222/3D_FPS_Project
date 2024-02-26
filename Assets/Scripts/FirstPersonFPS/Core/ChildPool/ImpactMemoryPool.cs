using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType
{
    Normal = 0, // ��, �ٴ�
    Obstacle,   // ��ֹ�
    Enemy       // ��
}

/// <summary>
/// Ÿ�� ����Ʈ�� �޸� Ǯ�� �����ϴ� ��ũ��Ʈ
/// </summary>
public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// �ǰ� ����Ʈ ������ ���� �迭 ����
    /// </summary>
    GameObject[] impactPrefab;

    /// <summary>
    /// �ǰ� ����Ʈ �޸� Ǯ�� ���� �迭
    /// </summary>
    MemoryPool[] memoryPool;

    private void Awake()
    {
        
        memoryPool = new MemoryPool[impactPrefab.Length];   // ��������� ���� ��ŭ �޸�Ǯ ����
        for(int i = 0; i < impactPrefab.Length; ++i)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]); // ���� ���� �޸�Ǯ�� ���
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        // �ε��� ������Ʈ�� Tag ������ ���� �ٸ��� ó��
        if(hit.transform.CompareTag("ImpactNormal")) // ���̳� �ٴڿ� ������
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal)); // �ǰ� ����Ʈ ����
        }
        else if (hit.transform.CompareTag("ImpactObstacle")) // ��ֹ��� ������
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal)); // �ǰ� ����Ʈ ����
        }
        else if(hit.transform.CompareTag("ImpactEnemy")) // ������ ������
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal)); // �ǰ� ����Ʈ ����
        }
    }

    /// <summary>
    /// �ǰ� ����Ʈ�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="type">�ǰ� ����Ʈ Ÿ��</param>
    /// <param name="position">���� ��ġ</param>
    /// <param name="rotation">ȸ��</param>
    private void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation)
    {
        GameObject item = memoryPool[(int)type].ActivePoolItem();   // ������Ʈ Ȱ��ȭ
        item.transform.position = position;                         // ��ġ ����
        item.transform.rotation = rotation;                         // ȸ�� ������
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);   // �޸� Ǯ ã��
    }
}
