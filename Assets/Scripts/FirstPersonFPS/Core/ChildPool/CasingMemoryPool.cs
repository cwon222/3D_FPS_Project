using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    /// <summary>
    /// ź�� ������Ʈ
    /// </summary>
    [SerializeField]
    GameObject casingPrefab;

    /// <summary>
    /// ź�� �޸�Ǯ
    /// </summary>
    MemoryPool memoryPool;

    private void Awake()
    {
        memoryPool = new MemoryPool(casingPrefab); // �޸�Ǯ ������ �޸� �Ҵ�
    }

    /// <summary>
    /// ź�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="position">��ġ</param>
    /// <param name="direction">����</param>ź
    public void SpawnCasing(Vector3 position, Vector3 direction)
    {
        GameObject item = memoryPool.ActivePoolItem();  // ���� ������Ʈ �ϳ� ȭ��ȭ
        item.transform.position = position; // ��ġ ����
        item.transform.rotation = Random.rotation;  // ���� ����(����)
        item.GetComponent<Casing>().Setup(memoryPool, direction); // Casing ������Ʈ ã�� ���� ����
    }
}
