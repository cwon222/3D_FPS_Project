using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UnityEvent�� Ŭ������ �Ϲ�ȭ ���ǿ� ���� ȣ���� �� �ִ� �̺�Ʈ �޼ҵ��� ������������ �����ȴ�
/// </summary>
[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

/// <summary>
/// ������ źâ �� ������ �ٲ� ������ �ܺο� �ִ� �޼ҵ带 �ڵ� ȣ���Ҽ��ֵ��� �̺�Ʈ Ŭ���� ����
/// </summary>
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// ź �̺�Ʈ ����
    /// </summary>
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();

    /// <summary>
    /// źâ �� �̺�Ʈ ����
    /// </summary>
    [HideInInspector]
    public MagazineEvent onMagazineEvent = new MagazineEvent();

    [Header("Fire Effect")]
    /// <summary>
    /// �ѱ����� ������ ����Ʈ ����
    /// </summary>
    [SerializeField]
    GameObject fireEffect;

    [Header("Spawn Points")]
    /// <summary>
    /// ź�� ���� ��ġ
    /// </summary>
    [SerializeField]
    Transform casingSpawnPoint;

    /// <summary>
    /// �Ѿ� ���� ��ġ
    /// </summary>
    [SerializeField]
    Transform bulletSpawnPoint;

    [Header("Weapon Setting")]
    /// <summary>
    /// ���� ������ ���� ����
    /// </summary>
    [SerializeField]
    WeaponSetting weaponSetting;

    [Header("Aim UI")]
    /// <summary>
    /// ���� ��忡 ���� Aim�̹��� Ȱ��ȭ ��Ȱ��ȭ
    /// </summary>
    [SerializeField]
    Image imageAim;


    /// <summary>
    /// ������ �߻� �ð� üũ�� ����
    /// </summary>
    float lastAttackTime = 0.0f;

    /// <summary>
    /// ������ ������ üũ�� �Լ�(ture : ������ ��, false : ������ �� �ƴ�)
    /// </summary>
    bool isReload = false;

    /// <summary>
    /// ���� ���� üũ�� ����
    /// </summary>
    bool isAttack = false;

    /// <summary>
    /// ��� ��ȯ üũ�� ����
    /// </summary>
    bool isModeChange = false;

    /// <summary>
    /// �⺻ ��忡�� ī�޶� ȭ��(FOV)
    /// </summary>
    float defaultModeFOV = 60.0f;

    /// <summary>
    /// aim��忡���� ī�޶� ȭ��(FOV)
    /// </summary>
    float aimModeFOV = 30.0f;

    /// <summary>
    /// �߻� �ִϸ��̼��� ���� �ϱ����� ����
    /// </summary>
    PlayerAnimatorController animator;

    /// <summary>
    /// ź�� ���� �� Ȱ��ȭ/��Ȱ��ȭ ����
    /// </summary>
    CasingMemoryPool casingMemoryPool;

    /// <summary>
    /// ���� ȿ�� ���� �� Ȱ��ȭ/��Ȱ��ȭ ����
    /// </summary>
    ImpactMemoryPool impactMemoryPool;

    /// <summary>
    /// ���� �߻縦 ���� ī�޶�
    /// </summary>
    Camera mainCamera;

    /// <summary>
    /// �ܺο��� �ʿ��� ���� �̸� ������ ���� ���� ������Ƽ
    /// </summary>
    public WeaponName WeaponName => weaponSetting.WeaponName;

    /// <summary>
    /// �ܺο��� �ʿ��� ���� źâ �� ������ ���� ���� ������Ƽ
    /// </summary>
    public int CurrentMagazine => weaponSetting.currentMagazine;

    /// <summary>
    /// �ܺο��� �ʿ��� �ִ� źâ �� ������ ���� ���� ������Ƽ
    /// </summary>
    public int MaxMagazine => weaponSetting.maxMagazine;

    private void Awake()
    {
        animator = GetComponentInParent<PlayerAnimatorController>(); // �θ� ������Ʈ�� �ִ� �÷��̾� ������Ʈ�� �ִ� PlayerAnimatorController ã��
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        // ó�� źâ ���� �ִ� źâ ���� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // ó�� ź ���� �ִ� ź���� ����
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        fireEffect.SetActive(false); // �Ѿ� �߻� ����Ʈ ��Ȱ��ȭ

        // ���Ⱑ Ȱ��ȭ �� �� �ش� ������ źâ ���� ���� �Ѵ�
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        // ���Ⱑ Ȱ��ȭ �� �� �ش� ������ ź ���� ���� �Ѵ�
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariables();
    }


    /// <summary>
    /// ���ݽ� ������ �Լ�
    /// </summary>
    /// <param name="type">���� ��</param>
    public void StartWeaponAction(int type = 0)
    {
        // ������ ���� ���� ���� �׼� �Ұ���
        if (isReload == true) return;

        // ��� ��ȯ ���̸� ���� �׼� �Ұ���
        if(isModeChange == true) return;

        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            // ���� ����
            if(weaponSetting.isAutomaticAttack == true)
            {
                isAttack = true; // ���� ���� �� ����
                StartCoroutine("OnAttackLoop"); // ���� ���� �ڷ�ƾ ����
            }
            // �ܹ� ����
            else
            {
                OnAttack(); // �ܹ� ���� �Լ� ȣ��
            }
        }
        // ���콺 ������ Ŭ�� (��� ��ȯ)
        else
        {
            // ���� ���� ���� ��� ��ȯ �Ұ���
            if (isAttack == true) return;

            StartCoroutine("OnModeChage"); // ���� ��� ��ȯ �ڷ�ƾ ����
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
            isAttack = false;   // ���� �� �ƴ� ����
            StopCoroutine("OnAttackLoop");  // ���� ���� �ڷ�ƾ ����
        }
    }

    /// <summary>
    /// ������ ���� ��Ű�� �Լ�
    /// </summary>
    public void StartReload()
    {
        // ���� ������ ���̰ų� źâ ���� 0�̸� ������ �Ұ���
        if(isReload == true || weaponSetting.currentMagazine <= 0) return;

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
            //animator.Play("Fire", -1, 0); // ���� �ִϸ��̼��� �ݺ��� �� �ִϸ��̼��� ���� ó������ �ٽ� ���
            //animator.Play("Fire"); // ���� �ִϸ��̼��� �ݺ��� �� �߰��� ���� ���ϰ� ��� �Ϸ� �� �ٽ� ���

            // ���� �ִϸ��̼� ��� (��忡 ���� AimFire �Ǵ� Fire �ִϸ��̼� ���)
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);

            // �ѱ� ����Ʈ�� default ����� ���� ���
            if (animator.AimModeIs == false) StartCoroutine("FireEffect");

            StartCoroutine("FireEffect"); // �Ѿ� �߻�� ����Ʈ �ڷ�ƾ ����

            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right); // ź�� ����

            // ������ �߻��� ���ϴ� ��ġ ���ݰ� ����Ʈ ȿ�� �Լ� ����
            TwoStepRaycast();
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
                isReload = false;   // ������ �� �ƴ� ����

                // ���� źâ ���� 1����
                weaponSetting.currentMagazine--;
                // �ٲ� źâ �� ������ Text UI�� ����
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // ���� ź ���� �ִ�� ����
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                // �ٲ� ź �� ������ Text UI�� ����
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// ���� �߻�Ǵ� ��ġ�� ���� �������� �ϴ� Ÿ���� ��ġ�� �޶� Ÿ�� �Ұ��� �׷���
    /// ������ �ִ� ȭ�� �߾���ġ�� �����ϴ� ������ �߻�
    /// �߻��� ������ ������ �ѱ����� �ٽ� ���� �߻�
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // ȭ���� �߾� ��ǥ Aim �������� Raycast ����
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        // ���� ��Ÿ��ȿ� �ε����� ������Ʈ�� ������ 
        if(Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;    // targetPoint�� ������ �ε��� ��ġ
        }
        // ���� ��Ÿ� �ȿ� �ε����� ������Ʈ�� ������ 
        else
        {
            // targetPoint�� �ִ� ��Ÿ�
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        // ȭ�� �߾� Raycast �� �׸���(Ȯ�ο�)
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        // ���� Raycast �������� ����� targetPoint�� ��ǥ �������� ����
        // (������ ���� Ÿ������Ʈ - �Ѿ� ���� ��ġ)����ȭ = ���� ����
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        // �ѱ��� ������������ �ؼ� Raycast
        if(Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if(hit.transform.CompareTag("ImpactEnemy")) // ���� ����� �±װ� ImpactEnemy�̸�
            {
                // ���� ����� ��ġ���ִ� EnemyStatus ������Ʈ�� TakeDamage�� �Ķ���͸� weaponSetting.damage �ְ� ȣ��
                hit.transform.GetComponent<EnemyStatus>().TakeDamage(weaponSetting.damage);
            }
        }
        // �ѱ� �� Raycast �� �׸���(Ȯ�ο�)
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
    }

    /// <summary>
    /// ���� ��� ��ȯ�ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator OnModeChage()
    {
        float current = 0;
        float percent = 0;
        float time = 0.35f;

        animator.AimModeIs = !animator.AimModeIs; // ���� ��� �Ķ���Ͱ� ���̸� ����, �����̸� ��
        imageAim.enabled = !imageAim.enabled;       // ���� �̹����� ���̸� ������ �ʰ�, ������ ������ ���̰� ����

        float start = mainCamera.fieldOfView;   // ���� ī�޶��� FOV
        float end = animator.AimModeIs == true ? aimModeFOV : defaultModeFOV; // ���Ӹ�� �Ķ���Ϳ� ���� ����FOV �ƴϸ� e����ƮFOV�� ����

        isModeChange = true; // ��� ���� ��

        while(percent < 1)
        {
            current += Time.deltaTime; // �ð� ����
            percent = current / time;

            // a��忡 ���� ī�޶��� �þ߰��� ����
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;   // ��� ���� �� �ƴ�
    }

    /// <summary>
    /// ����, ����, ��� �ʱ�ȭ
    /// </summary>
    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }
}


