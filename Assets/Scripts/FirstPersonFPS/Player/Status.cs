using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ü�� ������ �ٲ� ������ �ܺο� �ִ� �޼ҵ� �ڵ� ȣ���� �� �ְ� �̺�Ʈ Ŭ���� ����
/// </summary>
[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

/// <summary>
/// ĳ������ ���¸� �����ϴ� ��ũ��Ʈ
/// </summary>
public class Status : MonoBehaviour
{
    /// <summary>
    /// HP�̺�Ʈ Ŭ���� �ν��Ͻ� ����
    /// </summary>
    [HideInInspector]

    [Header("Move Speed")]
    public HPEvent onHPEvent = new HPEvent();
    /// <summary>
    /// �ȴ� �ӵ�
    /// </summary>
    [SerializeField]
    float walkSpeed;

    /// <summary>
    /// �ٴ� �ӵ�
    /// </summary>
    [SerializeField]
    float runSpeed;

    [Header("HP")]
    /// <summary>
    /// �ִ� ü��
    /// </summary>
    [SerializeField]
    int maxHP = 100;

    /// <summary>
    /// ���� ü��
    /// </summary>
    int currentHP;

    /// <summary>
    /// �ܺο��� �ȴ� �ӵ��� Ȯ���ϱ� ���� ������Ƽ
    /// </summary>
    public float WalkSpeed => walkSpeed;

    /// <summary>
    /// �ܺο��� �ٴ� �ӵ��� Ȯ���ϱ� ���� ������Ƽ
    /// </summary>
    public float RunSpeed => runSpeed;

    /// <summary>
    /// ���� ü�� HP Ȯ���� ���� ������Ƽ
    /// </summary>
    public int CurrentHP => currentHP;

    /// <summary>
    /// �ִ� ü�� HP Ȯ���� ���ϳ� ������Ƽ
    /// </summary>
    public int MaxHP => maxHP;

    private void Awake()
    {
        currentHP = maxHP; // ���� ü���� �ִ� ü������ ����
    }

    /// <summary>
    /// ������ ���� Ȯ�ο� ����
    /// </summary>
    /// <param name="damage">������</param>
    /// <returns></returns>
    public bool DecreaseHP(int damage)
    {
        int preHP = currentHP; // ������ ���� ü�� ����

        // ���� ü�¿��� �������� ���� �� 0���� ũ�� ���� ü�¿� ������ ��ŭ ������ ü���� ���� 0���� ������ ���� ü�¿� 0�� ����
        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        onHPEvent.Invoke(preHP, currentHP); // ü���� �ٲ���� �˸���

        if (currentHP == 0) // ���� ü���� 0�̸� 
        {
            return true; // ��
        }
        // ���� ü���� 0�� �ƴϸ�
        return false;   // ����
    }
}
