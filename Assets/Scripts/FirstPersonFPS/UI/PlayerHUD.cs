using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    /// <summary>
    /// ���� ������ ��µǴ� ����
    /// </summary>
    [SerializeField]
    Weapon weapon;

    [Header("Weapon Base")]
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

    [Header("Ammo")]
    /// <summary>
    /// ���� / �ִ� ź �� ��¿� Text
    /// </summary>
    [SerializeField]
    TextMeshProUGUI textAmmo;

    [Header("Magazine")]
    /// <summary>
    /// źâ UI ������
    /// </summary>
    [SerializeField]
    GameObject magazineUIPrefab;

    /// <summary>
    /// źâ UI�� ��ġ�Ǵ� �ǳ�
    /// </summary>
    [SerializeField]
    Transform magazineParent;

    /// <summary>
    /// ź�� UI ����Ʈ
    /// </summary>
    List<GameObject> magazineList;

    private void Awake()
    {
        SetupWeapon();  // ���� ���� ���� ���� �Լ� ȣ��
        SetupMagazine();    // ���� źâ ������ ���� �Լ� ȣ��

        // �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon��)��
        // Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
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

    private void SetupMagazine()
    {
        // weapon�� ��ϵǾ��ִ� �ִ� źâ ������ŭ Image Icon�� ����
        magazineList = new List<GameObject>();
        for(int i = 0; i <weapon.MaxMagazine; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab); 
            clone.transform.SetParent(magazineParent); // magazineParent ������Ʈ�� �ڽ����� ���
            clone.SetActive(false); // �� ��� ��Ȱ��ȭ

            magazineList.Add(clone);// ��� ����Ʈ�� ����
        }

        // weapon�� ��ϵǾ� �ִ� ���� źâ ���� ��ŭ ������Ʈ Ȱ��ȭ
        for(int i = 0; i < weapon.CurrentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    private void UpdateMagazineHUD(int currentMagazine)
    {
        // ���� ��Ȱ��ȭ
        for(int i = 0; i < magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false);
        }
        // currentMagazine ���� ��ŭ Ȱ��ȭ
        for(int i = 0; i < currentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }
}
