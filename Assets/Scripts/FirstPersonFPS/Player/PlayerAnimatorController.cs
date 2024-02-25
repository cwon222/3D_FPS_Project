using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    /// <summary>
    /// �ִϸ����� ������Ʈ ����
    /// </summary>
    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();  // �÷��̾� ������Ʈ�� �ڽ� ������Ʈ�� Animator ������Ʈ ã��
    }

    /// <summary>
    /// �ִϸ����� �ؽ��� ����� ������Ƽ
    /// </summary>
    public float MoveSpeed
    {
        set => animator.SetFloat("movementSpeed", value); // �ִϸ����� �Ķ���� ���� value ����
        get => animator.GetFloat("movementSpeed");          // �ִϸ����� �Ķ���� ���� ��ȯ
    }

    /// <summary>
    /// ������ �ִϸ��̼� �����ϴ� �Լ�
    /// </summary>
    public void OnReload()
    {
        animator.SetTrigger("onReload"); // ������ �ִϸ��̼� �ؽ��� ����
    }

    /// <summary>
    /// �ִϸ����͸� ��Ʈ���� ����
    /// </summary>
    /// <param name="stateNmae"></param>
    /// <param name="layer"></param>
    /// <param name="normalizedTime"></param>
    public void Play(string stateNmae, int layer, float normalizedTime)
    {
        animator.Play(stateNmae, layer, normalizedTime);
    }

    /// <summary>
    /// �ִϸ��̼��� ���� ���� ������ Ȯ���ϰ� ����� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="name">��ȯ�� �ִϸ��̼� �̸�</param>
    /// <returns></returns>
    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}