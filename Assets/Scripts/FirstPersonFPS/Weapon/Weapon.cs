using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UnityEvent�� Ŭ������ �Ϲ�ȭ ���ǿ� ���� ȣ���� �� �ִ� �̺�Ʈ �޼ҵ��� ������������ �����ȴ�
/// </summary>
[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
public class Weapon : MonoBehaviour
{
    /// <summary>
    /// ź �̺�Ʈ ����
    /// </summary>
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();

    /// <summary>
    /// �ѱ����� ������ ����Ʈ ����
    /// </summary>
    [SerializeField]
    GameObject fireEffect;

    /// <summary>
    /// ź�� ���� ��ġ
    /// </summary>
    [SerializeField]
    Transform casingSpawnPoint;

    /// <summary>
    /// ���� ������ ���� ����
    /// </summary>
    [SerializeField]
    WeaponSetting weaponSetting;

    /// <summary>
    /// ������ �߻� �ð� üũ�� ����
    /// </summary>
    float lastAttackTime = 0.0f;

    /// <summary>
    /// ������ ������ üũ�� �Լ�(ture : ������ ��, false : ������ �� �ƴ�)
    /// </summary>
    bool isReload = false;

    /// <summary>
    /// �߻� �ִϸ��̼��� ���� �ϱ����� ����
    /// </summary>
    PlayerAnimatorController animator;

    /// <summary>
    /// ź�� ���� �� Ȱ��ȭ/��Ȱ��ȭ ����
    /// </summary>
    CasingMemoryPool casingMemoryPool;

    /// <summary>
    /// �ܺο��� �ʿ��� ������ ���� ���� ������Ƽ
    /// </summary>
    public WeaponName WeaponName => weaponSetting.WeaponName;

    private void Awake()
    {
        animator = GetComponentInParent<PlayerAnimatorController>(); // �θ� ������Ʈ�� �ִ� �÷��̾� ������Ʈ�� �ִ� PlayerAnimatorController ã��
        casingMemoryPool = GetComponent<CasingMemoryPool>();

        // ó�� ź ���� �ִ� ź���� ����
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        fireEffect.SetActive(false); // �Ѿ� �߻� ����Ʈ ��Ȱ��ȭ

        // ���Ⱑ Ȱ��ȭ �� �� �ش� ������ ź ���� ���� �Ѵ�
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    /// <summary>
    /// ���ݽ� ������ �Լ�
    /// </summary>
    /// <param name="type">���� ��</param>
    public void StartWeaponAction(int type = 0)
    {
        // ������ ���� ���� ���� �׼� �Ұ���
        if (isReload == true) return;

        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            // ���� ����
            if(weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop"); // ���� ���� �ڷ�ƾ ����
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
            StopCoroutine("OnAttackLoop");  // ���� ���� �ڷ�ƾ ����
        }
    }

    /// <summary>
    /// ������ ���� ��Ű�� �Լ�
    /// </summary>
    public void StartReload()
    {
        // ���� ������ ���̸� ������ �Ұ���
        if(isReload == true) return;

        // ���� �׼� ���ֿ� R Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
        StopWeaponAction();

        StartCoroutine("OnReload"); // ������ �ڷ�ƾ ����
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
    public void OnAttack()
    {
        // ���� �ð� - ������ �߻� �ð� > ���� �ӵ�
        if (Time.time - lastAttackTime > weaponSetting.attackRate) 
        {
            // �ٰ� ���� ���� ���� �Ұ���
            if(animator.MoveSpeed > 0.5f) // �ٰ� �ִ� �ִϸ��̼��� ��� ���̸�
            {
                return; // �ٰ� ���� ���� ��ȯ
            }

            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ������ �Լ�
            lastAttackTime = Time.time;
            
            // ź ���� ������ ���� ���ϰ� ����
            if(weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            // ���ݽ� ���� ���� ź �� 1 ����
            weaponSetting.currentAmmo--;
            // ź�� UI ����
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // ���� �ִϸ��̼� ���
            animator.Play("Fire", -1, 0); // ���� �ִϸ��̼��� �ݺ��� �� �ִϸ��̼��� ���� ó������ �ٽ� ���
            //animator.Play("Fire"); // ���� �ִϸ��̼��� �ݺ��� �� �߰��� ���� ���ϰ� ��� �Ϸ� �� �ٽ� ���

            StartCoroutine("FireEffect"); // �Ѿ� �߻�� ����Ʈ �ڷ�ƾ ����

            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
        }
    }

    /// <summary>
    /// �Ѿ� �߻�� �ѱ� ����Ʈ �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator FireEffect()
    {
        fireEffect.SetActive(true); // �ѱ� ����Ʈ Ȱ��ȭ

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f); // ��ٸ���

        fireEffect.SetActive(false);    // ��Ȱ��ȭ
    }

    IEnumerator OnReload()
    {
        isReload = true;

        animator.OnReload(); // ������ �ִϸ��̼� ����

        while(true)
        {
            // ���� �ִϸ��̼Ǵ� movement�̸� ������ �ִϸ��̼� ����� ����Ȱ�
            if(animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                // ���� ź ���� �ִ�� ����
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                // �ٲ� ź �� ������ Text UI�� ����
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }
}


