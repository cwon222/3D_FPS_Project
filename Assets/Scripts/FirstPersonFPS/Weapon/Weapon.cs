using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UnityEvent의 클래스의 일반화 정의에 따라 호출할 수 있는 이벤트 메소드의 ㅐㅁ개변수가 결정된다
/// </summary>
[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
public class Weapon : MonoBehaviour
{
    /// <summary>
    /// 탄 이벤트 변수
    /// </summary>
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();

    /// <summary>
    /// 총구에서 나오는 이펙트 변수
    /// </summary>
    [SerializeField]
    GameObject fireEffect;

    /// <summary>
    /// 탄피 생성 위치
    /// </summary>
    [SerializeField]
    Transform casingSpawnPoint;

    /// <summary>
    /// 무기 설정을 위한 변수
    /// </summary>
    [SerializeField]
    WeaponSetting weaponSetting;

    /// <summary>
    /// 마지막 발사 시간 체크용 변수
    /// </summary>
    float lastAttackTime = 0.0f;

    /// <summary>
    /// 재장전 중인지 체크용 함수(ture : 재장전 중, false : 재장전 중 아님)
    /// </summary>
    bool isReload = false;

    /// <summary>
    /// 발사 애니메이션을 제어 하기위한 변수
    /// </summary>
    PlayerAnimatorController animator;

    /// <summary>
    /// 탄피 생성 후 활성화/비활설화 관리
    /// </summary>
    CasingMemoryPool casingMemoryPool;

    /// <summary>
    /// 외부에서 필요한 정보를 보기 위한 프로퍼티
    /// </summary>
    public WeaponName WeaponName => weaponSetting.WeaponName;

    private void Awake()
    {
        animator = GetComponentInParent<PlayerAnimatorController>(); // 부모 오브젝트에 있는 플레이어 오브젝트에 있는 PlayerAnimatorController 찾기
        casingMemoryPool = GetComponent<CasingMemoryPool>();

        // 처음 탄 수는 최대 탄수로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        fireEffect.SetActive(false); // 총알 발사 이펙트 비활성화

        // 무기가 활성화 될 때 해당 무기의 탄 수를 갱신 한다
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    /// <summary>
    /// 공격시 실행할 함수
    /// </summary>
    /// <param name="type">받은 값</param>
    public void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 떄는 무기 액션 불가능
        if (isReload == true) return;

        // 마우스 왼쪽 클릭 (공격 시작)
        if (type == 0)
        {
            // 연속 공격
            if(weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop"); // 연사 공격 코루틴 시작
            }
            // 단발 공격
            else
            {
                OnAttack(); // 단발 공격 함수 호출
            }
        }
    }

    /// <summary>
    /// 연사 공격 종료 함수
    /// </summary>
    /// <param name="type">받은 값</param>
    public void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽  클릭 (공격 종료)
        if(type == 0)
        {
            StopCoroutine("OnAttackLoop");  // 연사 공격 코루틴 정지
        }
    }

    /// <summary>
    /// 재장전 실행 시키는 함수
    /// </summary>
    public void StartReload()
    {
        // 현재 재장전 중이면 재장전 불가능
        if(isReload == true) return;

        // 무기 액션 동주에 R 키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
        StopWeaponAction();

        StartCoroutine("OnReload"); // 재장전 코루틴 실행
    }

    /// <summary>
    /// 연사 공격시 실행할 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator OnAttackLoop()
    {
        while(true) // 계속
        {
            OnAttack();  // 단발 공격

            yield return null;
        }
    }

    /// <summary>
    /// 단발 공격시 실행할 함수
    /// </summary>
    public void OnAttack()
    {
        // 현재 시간 - 마지막 발사 시간 > 공격 속도
        if (Time.time - lastAttackTime > weaponSetting.attackRate) 
        {
            // 뛰고 있을 때는 공격 불가능
            if(animator.MoveSpeed > 0.5f) // 뛰고 있는 애니메이션이 재생 중이면
            {
                return; // 뛰고 있을 때는 반환
            }

            // 공격주기가 되어야 공격할 수 있도록 하기 위한 현재 시간 저장할 함수
            lastAttackTime = Time.time;
            
            // 탄 수가 없으면 공격 못하게 설정
            if(weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            // 공격시 현재 남은 탄 수 1 감소
            weaponSetting.currentAmmo--;
            // 탄수 UI 갱신
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // 무기 애니메이션 재생
            animator.Play("Fire", -1, 0); // 같은 애니메이션을 반복할 때 애니메이션을 끊고 처음부터 다시 재생
            //animator.Play("Fire"); // 같은 애니메이션을 반복할 때 중간에 끊지 못하고 재생 완료 후 다시 재생

            StartCoroutine("FireEffect"); // 총알 발사시 이펙트 코루틴 시작

            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
        }
    }

    /// <summary>
    /// 총알 발사시 총구 이펙트 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator FireEffect()
    {
        fireEffect.SetActive(true); // 총구 이펙트 활성화

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f); // 기다리기

        fireEffect.SetActive(false);    // 비활성화
    }

    IEnumerator OnReload()
    {
        isReload = true;

        animator.OnReload(); // 재장전 애니메이션 실행

        while(true)
        {
            // 현재 애니메이션니 movement이면 재장전 애니메이션 재생이 종료된것
            if(animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                // 현재 탄 수를 최대로 설정
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                // 바뀐 탄 수 정보를 Text UI에 갱신
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }
}


