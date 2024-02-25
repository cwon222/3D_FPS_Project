using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ĳ������ ���¸� �����ϴ� ��ũ��Ʈ
/// </summary>
public class Status : MonoBehaviour
{
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

    /// <summary>
    /// �ܺο��� �ȴ� �ӵ��� Ȯ���ϱ� ���� ������Ƽ
    /// </summary>
    public float WalkSpeed => walkSpeed;

    /// <summary>
    /// �ܺο��� �ٴ� �ӵ��� Ȯ���ϱ� ���� ������Ƽ
    /// </summary>
    public float RunSpeed => runSpeed;
}
