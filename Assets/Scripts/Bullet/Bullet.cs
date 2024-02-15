using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : RecycleObject
{
    // �÷��̾�: ������ ���, �� ���, �� ���� ���, ��ũ���� ���
    // �÷��̾�: �ٱ� ������ �ȱ� �ѽ�� �ɱ� �� ������ ����
    // �Ѿ�: �Ѿ� ���� �߻� �ݵ� �߻�� ����Ʈ  
    // ��: �Ѿ� ������ �ǰ�, �Ѿ� �� ���̰ų� ����, �÷��̾� �ڵ� ���� or ���� �̵�, �����ȿ� �÷��̾ ���̸� �Ѿ� �߻�

    // �������ڸ��� ��� ���������� �ʼ� 7�ʷ� �����̱�
    public float bulletSpeed = 7.0f;

    /// <summary>
    /// �Ѿ��� ����
    /// </summary>
    public float lifeTime = 10.0f;


    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(LifeOver(lifeTime)); // �ڷ�ƾ ����
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * bulletSpeed * Vector2.right);     // �Ѿ� �̵� // �� ���� ���� 4��
    }

    private void OnCollisionEnter(Collision collision)  // �浹�� ���� �Լ�
    {
        //Factory.Instance.GetHitEffect(transform.position);

        gameObject.SetActive(false);    // ��Ȱ��ȭ -> Ǯ�� �ǵ�����
    }
}
