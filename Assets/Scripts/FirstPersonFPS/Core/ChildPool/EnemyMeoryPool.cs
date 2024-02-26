using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// ���� �޸� Ǯ�� ������ ��ũ��Ʈ
/// </summary>
public class EnemyMeoryPool : MonoBehaviour
{

    /// <summary>
    /// Ÿ�� (��ǥ) ��ġ
    /// </summary>
    [SerializeField]
    Transform target;
    /// <summary>
    /// ���� ������ �� ���� ���� ��ġ�� �˷��ִ� ������ �ֱ�
    /// </summary>
    [SerializeField]
    GameObject enemySpawnPointPrefab;

    /// <summary>
    /// �����Ǵ� �� �������� ���� ����
    /// </summary>
    [SerializeField]
    GameObject enemyPrefab;

    /// <summary>
    /// �� ���� �ֱ�
    /// </summary>
    [SerializeField]
    float enemySpawnTime = 1.0f;

    /// <summary>
    /// Ÿ�� ���� �� ���� �����ϱ���� ��� �ð�
    /// </summary>
    [SerializeField]
    float enemySpawnLatency = 1.0f;

    /// <summary>
    /// �� ���� ��ġ�� �˷��ִ� ������Ʈ ������ Ȱ��Ȱ/ ��Ȱ��ȭ�� �����ϱ� ���� ����
    /// </summary>
    MemoryPool spawnPointMemoryPool;

    /// <summary>
    /// �� ������ Ȱ��ȭ / ��Ȱ��ȭ�� �����ϱ� ���� ����
    /// </summary>
    MemoryPool enemyMemoryPool;

    /// <summary>
    /// ���ÿ� �����Ǵ� ���� ����
    /// </summary>
    int concurrentGenerationNumber = 1;

    /// <summary>
    /// ���� ũ��
    /// </summary>
    Vector2Int mapSize = new Vector2Int(100, 100);

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);   // ��� �ٲٱ� �ν��Ͻ� ������ ������
        enemyMemoryPool = new MemoryPool(enemyPrefab);                  // ��� �ٲٱ� �ν��Ͻ� ������ ������
        //spawnPointMemoryPool = AddedComponent(enemySpawnPointPrefab) as MemoryPool;


        StartCoroutine("SpawnTile");    // Ÿ�ϸ� �����ϴ� �ڷ�ƾ ����
    }

    /// <summary>
    /// ���� �����Ǳ� ���� ����� ����� �����ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnTile()
    {
        // ���� �ѹ��� �����Ǵ� ����� ��
        int currentNumber = 0;
        // �ִ� �ѹ��� �����Ǵ� ����� ��
        int maximumNumber = 50;

        while(true)
        {
            for(int i = 0; i < concurrentGenerationNumber; ++i) // ���ÿ� �����Ǵ� ���� �� ��ŭ
            {
                GameObject item = spawnPointMemoryPool.ActivePoolItem(); // ���� �����Ǵ� ��� ������Ʈ�� Ȱ��ȭ ������

                // �� ��� ��ġ = x�� z�� �������� ��ġ ����
                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.y * 0.49f), 1,
                                                      Random.Range(-mapSize.x * 0.49f, mapSize.y * 0.49f));

                StartCoroutine("SpawnEnemy", item); // �� ���� �ڷ�ƾ ����
            }

            currentNumber++; // ���� �ѹ��� �����Ǵ� ���� ��� �� ����

            if(currentNumber >= maximumNumber) // ���� ����� �ѹ��� �����Ǵ� ���� �ִ� �� ���� ũ�� 
            {
                currentNumber = 0;      // ���� �ѹ��� �����Ǵ� ���� ��� �� �ʱ�ȭ
                concurrentGenerationNumber++;   // ���ÿ� ���� �����Ǵ� �� ����
            }

            yield return new WaitForSeconds(enemySpawnTime);    // ���� �ֱ⿡ ���� ����
        }
    }

    /// <summary>
    /// �� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="point">������ ��ġ</param>
    /// <returns></returns>
    IEnumerator SpawnEnemy(GameObject point)
    {
        // �� ���� ���� ��Ÿ���� ��� Ÿ�� ���� �ð��� ������ ����
        yield return new WaitForSeconds(enemySpawnLatency);

        // ���� ������Ʈ�� ����
        GameObject item = enemyMemoryPool.ActivePoolItem();
        // ���� ��ġ�� point�� ��ġ�� ����
        item.transform.position = point.transform.position;

        // �� �̵� ��ũ��Ʈ�� ã�� Setup �޼ҵ��� �Ű������� target ����
        item.GetComponent<EnemyStatus>().Setup(target, this);

        // �� �������� ����� Ÿ�� ������Ʈ ��Ȱ��ȭ
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    /// <summary>
    /// �� ��Ȱ��ȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="enemy">��Ȱ��ȭ�� ��</param>
    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy); // ��Ȱ��ȭ
    }
}
