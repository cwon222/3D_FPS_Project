using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    /// <summary>
    /// ź�� ���� �� ��Ȱ��ȭ �Ǵ� �ð�
    /// </summary>
    [SerializeField]
    float deactivateTime = 5.0f;

    /// <summary>
    /// ź�ǰ� ȸ���ϴ� �ӷ� ���
    /// </summary>
    [SerializeField]
    float casingSpin = 1.0f;

    Rigidbody rigid;
    MemoryPool memoryPool;

    /// <summary>
    /// ������Ʈ�� �̵��ӵ��� ���� ������ �Լ�
    /// </summary>
    /// <param name="pool">������ ������Ʈ</param>
    /// <param name="direction">����</param>
    public void Setup(MemoryPool pool, Vector3 direction)
    {
        rigid = GetComponent<Rigidbody>();
        memoryPool = pool;

        // ź���� �̵� �ӵ��� ȸ�� �ӵ� ����
        rigid.velocity = new Vector3(direction.x, 1.0f, direction.z);
        rigid.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                            Random.Range(-casingSpin, casingSpin),
                                            Random.Range(-casingSpin, casingSpin));

        // ź�� �ڵ� ��Ȱ��ȭ�� ���� �ڷ�ƾ ����
        StartCoroutine("DeactivateAfterTime");
    }

    /// <summary>
    /// ��Ȱ��Ȱ ��Ű�� ���� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime); // deactivateTime ��ٸ���

        memoryPool.DeactivatePoolItem(this.gameObject); // ��ٸ� �� ��Ȱ��ȭ
    }
}
