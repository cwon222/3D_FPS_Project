using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // ��ƼŬ�� ������� �ƴϸ� ����
        if (particle.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}
