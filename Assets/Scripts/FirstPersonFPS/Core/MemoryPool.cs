using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ʈ���� ������ Ǯ ��ũ��Ʈ
/// </summary>
public class MemoryPool
{
    /// <summary>
    ///  �޸� Ǯ�� �����Ǵ� ������Ʈ
    /// </summary>
    private class PoolItem
    {
        /// <summary>
        /// ������Ʈ�� Ȱ��ȭ / ��Ȱ����ȭ ����
        /// </summary>
        public bool isActive;

        /// <summary>
        /// ȭ�鿡 ���̴� ���� ���� ������Ʈ
        /// </summary>
        public GameObject gameObject;
    }

    /// <summary>
    /// ��������Ʈ�� ������ �� Instantiate()�� �߰� �����Ǵ� ������Ʈ ����
    /// </summary>
    int increaseCount = 5;

    /// <summary>
    ///  ���� ����Ʈ�� ��ϵǾ��ִ� ������Ʈ ����
    /// </summary>
    int maxCount;

    /// <summary>
    /// ���� ���ӿ� ���ǰ� �ִ�(Ȱ��ȭ��) ������Ʈ ����
    /// </summary>
    int activeCount;

    /// <summary>
    /// ������Ʈ Ǯ������ �����ϴ� ���� ������Ʈ ������
    /// </summary>
    GameObject poolObject;
    
    /// <summary>
    /// �����Ǵ� ��� ������Ʈ�� �����ϴ� ����Ʈ
    /// </summary>
    List<PoolItem> poolItemList;

    /// <summary>
    /// �ܺο��� ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ
    /// </summary>
    public int MaxCount => maxCount;

    /// <summary>
    /// �ܺο��� ���� Ȱ��ȭ�� ������Ʈ�� ���� Ȯ���� ���� ������Ƽ
    /// </summary>
    public int ActiveCount => activeCount;

    /// <summary>
    /// ������ �ʱ�ȭ �۾�
    /// </summary>
    /// <param name="poolObject"></param>
    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;

        poolItemList = new List<PoolItem>(); //����Ʈ ����

        InstantiateObjects(); // Ǯ�� �Լ� ����
    }

    /// <summary>
    /// ������Ʈ �����ϴ� �Լ�
    /// </summary>
    public void InstantiateObjects()
    {
        maxCount += increaseCount; // �ִ� ���� ����

        for(int i = 0; i < increaseCount; ++i) 
        {
            PoolItem poolItem = new PoolItem(); // ����Ʈ ����

            poolItem.isActive = false;  // ������Ʈ ��Ȱ��ȭ�ϰ� ����� ������ ����
            poolItem.gameObject = GameObject.Instantiate(poolObject); // ������Ʈ ����
            poolItem.gameObject.SetActive(false); // ������Ʈ �Ⱥ��̰� �ϱ�

            poolItemList.Add(poolItem); // ����Ʈ �迭�� �߰�
        }
    }

    /// <summary>
    /// ���� �������� ��� ������Ʈ�� ����
    /// </summary>
    public void Destroyobjects()
    {
        if (poolItemList == null) return; // ������ ����Ʈ�� ��������� ��ȯ

        int count = poolItemList.Count; // ����Ʈ ���� ��ŭ ����
        for(int i = 0; i < count; ++i)
        {
            GameObject.Destroy(poolItemList[i].gameObject); // ����Ʈ�� ������Ʈ�� �ı�
        }

        poolItemList.Clear(); 
    }

    /// <summary>
    /// ���� ��Ȱ��ȭ ������ ������Ʈ �� �ϳ��� Ȱ��ȭ�� ����� ����ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    public GameObject ActivePoolItem()
    {
        if(poolItemList == null) return null; // ����Ʈ�� ��������� ��ȯ

        // ���� �����ؼ� �����ϴ� ��� ������Ʈ ������ ���� Ȱ��ȭ ������ ������Ʈ ���� ��
        // ��� ������Ʈ�� Ȱ��ȭ �����̸� ���ο� ������Ʈ �ʿ���
        if(maxCount == activeCount) // ��� Ȱ��ȭ �����̸� 
        {
            InstantiateObjects(); // �߰��� ���������� ����
        }

        int count = poolItemList.Count;
        for(int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i]; // ������ ��������Ʈ ����Ʈ�� ������Ʈ �ֱ�

            if(poolItem.isActive ==  false) // ��Ȱ��ȭ�� ������Ʈ��
            {
                activeCount++;

                poolItem.isActive = true; 
                poolItem.gameObject.SetActive (true);   // Ȱ��ȭ 

                return poolItem.gameObject; // ������ ������Ʈ ��ȯ
            }
        }

        return null;
    }

    /// <summary>
    /// ���� ����� �Ϸ�� ������Ʈ�� ��Ȱ��ȭ ���·� ����
    /// </summary>
    /// <param name="removeObject">��Ȱ��ȭ �� ������Ʈ</param>
    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null) return;   // ����Ʈ�� ����ְų� ��Ȱ��ȭ�� ������Ʈ�� ������ ��ȯ

        int count = poolItemList.Count;             // ����Ʈ ũ�� �� ����
        for(int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];    // ������ ��������Ʈ ����Ʈ�� ������Ʈ �ֱ�

            if (poolItem.gameObject == removeObject) // Ǯ�������� ������Ʈ�� ��Ȱ��ȭ �� ������Ʈ�̸�
            {
                activeCount--; // ī��Ʈ ���̱�

                poolItem.isActive = false;  
                poolItem.gameObject.SetActive (false); // ��Ȱ��ȭ

                return;
            }
        }
    }

    /// <summary>
    /// ���ӿ� ������� ��� ������Ʈ�� ��Ȱ��ȭ ���·� �����Ϥ��� �Լ�
    /// </summary>
    public void DeactivateAllPoolItems()
    {
        if (poolItemList == null) return; // ����Ʈ�� ������ ��ȯ

        int count = poolItemList.Count; // ����Ʈ ���� �� ����
        for(int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];    // ������ ��������Ʈ ����Ʈ�� ������Ʈ �ֱ�

            if (poolItem.gameObject != null && poolItem.isActive == true) // ������ ������Ʈ�� �ְ� Ȱ��ȭ �����̸�
            {
                poolItem.isActive = false;      
                poolItem.gameObject.SetActive (false);  // ��Ȱ��ȭ
            }
        }

        activeCount = 0;    // �ʱ�ȭ
    }
}
