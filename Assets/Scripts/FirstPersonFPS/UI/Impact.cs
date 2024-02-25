using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ÿ�� ����Ʈ�� �����ϴ� ��ũ��Ʈ
/// </summary>
public class Impact : MonoBehaviour
{
    /// <summary>
    /// ��ƼŬ �ý��� ����
    /// </summary>
    ParticleSystem particle;

    /// <summary>
    /// �޸�Ǯ ����
    /// </summary>
    MemoryPool memoryPool;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>(); // ��ƼŬ ������Ʈ ã��
    }

    /// <summary>
    /// �޸� Ǯ�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="pool">������ �޸�</param>
    public void Setup(MemoryPool pool)
    {
        memoryPool = pool;
    }

    private void Update()
    {
        if(particle.isPlaying == false) // ��ƼŬ�� ������� �ƴϸ�
        {
            memoryPool.DeactivatePoolItem(gameObject); // ����
        }    
    }
}
