using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    [Header("Components")]
    /// <summary>
    /// 현재 정보가 출력되는 무기
    /// </summary>
    [SerializeField]
    Weapon weapon;

    /// <summary>
    /// 플레이어의 상태(이동속도, 체력)
    /// </summary>
    [SerializeField]
    Status status;


    [Header("Weapon Base")]
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

    [Header("Ammo")]
    /// <summary>
    /// 현재 / 최대 탄 수 출력용 Text
    /// </summary>
    [SerializeField]
    TextMeshProUGUI textAmmo;

    [Header("Magazine")]
    /// <summary>
    /// 탄창 UI 프리펩
    /// </summary>
    [SerializeField]
    GameObject magazineUIPrefab;

    /// <summary>
    /// 탄창 UI가 배치되는 판넬
    /// </summary>
    [SerializeField]
    Transform magazineParent;

    /// <summary>
    /// 탄장 UI 리스트
    /// </summary>
    List<GameObject> magazineList;

    [Header("HP & BloodScreen UI")]
    /// <summary>
    /// 플레이어의 체력을 출력하기 위한
    /// </summary>
    [SerializeField]
    TextMeshProUGUI textHP;

    /// <summary>
    /// 플레이어가 공격 받으면 화면에 표시되기 위한
    /// </summary>
    [SerializeField]
    Image imageBloodScreen;

    /// <summary>
    /// 공격 받을때 변경되는 이미지를 설정하기 위한
    /// </summary>
    [SerializeField]
    AnimationCurve curveBloodScreen;


    

    private void Awake()
    {
        SetupWeapon();  // 현재 무기 정보 갱신 함수 호출
        SetupMagazine();    // 현재 탄창 정보를 갱신 함수 호출

        // 메소드가 등록되어 있는 이벤트 클래스(weapon들)의
        // Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
        // Weapon::OnEnable() : 무기 오브젝트가 활성화 될 때
        // Weapon::OnAttack() : 공격으로 탄이 소모되었을 떄 
        //UpdateAmmoHUD() 호출한다
        status.onHPEvent.AddListener(UpdateHPHUD);
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

    /// <summary>
    /// 화면에 탄창 이미지 아이콘을 관리 함수
    /// </summary>
    private void SetupMagazine()
    {
        // weapon에 등록되어있는 최대 탄창 개수만큼 Image Icon을 생성
        magazineList = new List<GameObject>();
        for(int i = 0; i <weapon.MaxMagazine; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab); 
            clone.transform.SetParent(magazineParent); // magazineParent 오브젝트의 자식으로 등록
            clone.SetActive(false); // 후 모두 비활성화

            magazineList.Add(clone);// 모두 리스트에 저장
        }

        // weapon에 등록되어 있는 현재 탄창 개수 만큼 오브젝트 활성화
        for(int i = 0; i < weapon.CurrentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    /// <summary>
    /// 탄창 이미지를 갱신할 함수
    /// </summary>
    /// <param name="currentMagazine">현재 탄창 수</param>
    private void UpdateMagazineHUD(int currentMagazine)
    {
        // 전부 비활성화
        for(int i = 0; i < magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false);
        }
        // currentMagazine 개수 만큼 활성화
        for(int i = 0; i < currentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    /// <summary>
    /// 바뀐 체력을 업데이트할 함수
    /// </summary>
    /// <param name="previous">이전 체력</param>
    /// <param name="current">현재 체력</param>
    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP" + current; // 체력 텍스트 변경

        if(previous - current > 0) // 남은 체력이 0 이상이면
        {
            StopCoroutine("OnBloodScreen"); // 피격 이미지 코루틴 정지
            StartCoroutine("OnBloodScreen");    // 피격 이미지 코루틴 실행
        }
    }

    /// <summary>
    /// 플레이어가 피격당하면 이미지를 관리할 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent)); // 알파 값을 1에서 0까지 1초 동안 감ㅅ소
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
