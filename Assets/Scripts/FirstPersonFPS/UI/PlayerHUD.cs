using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    /// <summary>
    /// 현재 정보가 출력되는 무기
    /// </summary>
    [SerializeField]
    Weapon weapon;

    /// <summary>
    /// 무기 이름
    /// </summary>
    [SerializeField]
    TextMeshProUGUI textWeaponName;
    /// <summary>
    /// 무기 아이콘
    /// </summary>
    [SerializeField]
    Image imageWeaponIcon;

    /// <summary>
    /// 무기 아이콘에 사용되는 sprite 배열
    /// </summary>
    [SerializeField]
    Sprite[] spriteWeaponIcons;

    /// <summary>
    /// 현재 / 최대 탄 수 출력용 Text
    /// </summary>
    [SerializeField]
    TextMeshProUGUI textAmmo;

    private void Awake()
    {
        SetupWeapon();  // 현재 무기 정보 갱신 함수 호출

        // 메소드가 등록되어 있는 이벤트 클래스(weapon들)의
        // Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        // Weapon::OnEnable() : 무기 오브젝트가 활성화 될 때
        // Weapon::OnAttack() : 공격으로 탄이 소모되었을 떄 
        //UpdateAmmoHUD() 호출한다
    }

    /// <summary>
    /// 현재 무기 정보를 갱신할 함수
    /// </summary>
    void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString(); // 무기 이름 갱신
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName]; // 무기 아이콘 갱신
    }

    /// <summary>
    /// 탄 수의 텍스트 정보를 갱신할 함수
    /// </summary>
    /// <param name="currentAmmo">현재 탄수</param>
    /// <param name="maxAmmo">최대 탄수</param>
    void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}"; // 텍스트 갱신
    }
}
