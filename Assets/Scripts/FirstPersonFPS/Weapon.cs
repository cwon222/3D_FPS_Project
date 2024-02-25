using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// ���� ������ ���� ����
    /// </summary>
    public WeaponSetting weaponSetting;

    /// <summary>
    /// ������ �߻� �ð� üũ�� ����
    /// </summary>
    float lastAttackTime = 0.0f;

    /// <summary>
    /// �߻� �ִϸ��̼��� ���� �ϱ����� ����
    /// </summary>
    PlayerAnimatorController animator;

    private void Awake()
    {
        animator = GetComponentInParent<PlayerAnimatorController>(); // �θ� ������Ʈ�� �ִ� �÷��̾� ������Ʈ�� �ִ� PlayerAnimatorController ã��
    }

    /// <summary>
    /// ���ݽ� ������ �Լ�
    /// </summary>
    /// <param name="type">���� ��</param>
    public void StartWeaponAction(int type = 0)
    {
        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            // ���� ����
            if(weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine(OnAttackLoop()); // ���� ���� �ڷ�ƾ ����
            }
            // �ܹ� ����
            else
            {
                OnAttack(); // �ܹ� ���� �Լ� ȣ��
            }
        }
    }

    /// <summary>
    /// ���� ���� ���� �Լ�
    /// </summary>
    /// <param name="type">���� ��</param>
    public void StopWeaponAction(int type = 0)
    {
        // ���콺 ����  Ŭ�� (���� ����)
        if(type == 0)
        {
            StopCoroutine(OnAttackLoop());  // ���� ���� �ڷ�ƾ ����
        }
    }

    /// <summary>
    /// ���� ���ݽ� ������ �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator OnAttackLoop()
    {
        while(true) // ���
        {
            OnAttack();  // �ܹ� ����

            yield return null;
        }
    }

    /// <summary>
    /// �ܹ� ���ݽ� ������ �Լ�
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnAttack()
    {
        // ���� �ð� - ������ �߻� �ð� > ���� �ӵ�
        if (Time.deltaTime - lastAttackTime > weaponSetting.attackRate) 
        {
            // �ٰ� ���� ���� ���� �Ұ���
            if(animator.MoveSpeed > 0.5) // �ٰ� �ִ� �ִϸ��̼��� ��� ���̸�
            {
                return; // �ٰ� ���� ���� ��ȯ
            }

            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ������ �Լ�
            lastAttackTime = Time.time;

            // ���� �ִϸ��̼� ���
            animator.Play("Fire", -1, 0); // ���� �ִϸ��̼��� �ݺ��� �� �ִϸ��̼��� ���� ó������ �ٽ� ���
            //animator.Play("Fire"); // ���� �ִϸ��̼��� �ݺ��� �� �߰��� ���� ���ϰ� ��� �Ϸ� �� �ٽ� ���
        }
    }
}


