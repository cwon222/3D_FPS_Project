using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ���� ����Ʈ ������ ��ũ��Ʈ
/// </summary>
public class EnemySpawnPoint : MonoBehaviour
{
    /// <summary>
    /// ���� ��Ÿ�� ��ġ�� �����Ÿ��� ǥ�� ���ִ� �ӵ�
    /// </summary>
    [SerializeField]
    float fadeSpeed = 4;

    /// <summary>
    /// �Ž������� ����
    /// </summary>
    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>(); // �Ž������� ������Ʈ ã��
    }

    private void OnEnable()
    {
        StartCoroutine("OnFadeEffect"); // �����Ÿ��� ȿ�� �ڷ�ƾ ����
    }

    private void OnDisable()
    {
        StopCoroutine("OnFadeEffect"); // �����Ÿ��� ȿ�� �ڷ�ƾ ����
    }

    /// <summary>
    /// ���� �Ÿ��� ȿ���� ��Ÿ���� �� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator OnFadeEffect()
    {
        while (true)
        {
            Color color = meshRenderer.material.color;  // �������� ���׸��� ���� ������ �÷� ����
            // Mathf.PingPong(Time.deltaTime * fadeSpeed, 1) : Time.deltaTime * fadeSpeed ���� ���� 0���� 1 ������ ���� ��ȯ
            // Time.deltaTime * fadeSpeed ������ �� 1���� Time.deltaTime * fadeSpeed ���� ��ȯ�ϰ� 
            // Time.deltaTime * fadeSpeed ���� 1���� Ŀ���� �� ���������� 0 ���� ���ش�, 1���� ���ϱ� �ݺ�
            // ���׸����� ���� ���İ��� ���� �����ֱ�
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));
            // �� ����
            meshRenderer.material.color = color;

            yield return null;
        }
    }
}
