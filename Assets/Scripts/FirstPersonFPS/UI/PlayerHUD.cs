using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    /// <summary>
    /// ���� ������ ��µǴ� ����
    /// </summary>
    [SerializeField]
    Weapon weapon;

    /// <summary>
    /// ���� �̸�
    /// </summary>
    [SerializeField]
    TextMeshProUGUI textWeaponName;
    /// <summary>
    /// ���� ������
    /// </summary>
    [SerializeField]
    Image imageWeaponIcon;

    /// <summary>
    /// ���� �����ܿ� ���Ǵ� sprite �迭
    /// </summary>
    [SerializeField]
    Sprite[] spriteWeaponIcons;

    /// <summary>
    /// ���� / �ִ� ź �� ��¿� Text
    /// </summary>
    [SerializeField]
    TextMeshProUGUI textAmmo;

    private void Awake()
    {
        SetupWeapon();  // ���� ���� ���� ���� �Լ� ȣ��

        // �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon��)��
        // Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        // Weapon::OnEnable() : ���� ������Ʈ�� Ȱ��ȭ �� ��
        // Weapon::OnAttack() : �������� ź�� �Ҹ�Ǿ��� �� 
        //UpdateAmmoHUD() ȣ���Ѵ�
    }

    /// <summary>
    /// ���� ���� ������ ������ �Լ�
    /// </summary>
    void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString(); // ���� �̸� ����
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName]; // ���� ������ ����
    }

    /// <summary>
    /// ź ���� �ؽ�Ʈ ������ ������ �Լ�
    /// </summary>
    /// <param name="currentAmmo">���� ź��</param>
    /// <param name="maxAmmo">�ִ� ź��</param>
    void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}"; // �ؽ�Ʈ ����
    }
}
