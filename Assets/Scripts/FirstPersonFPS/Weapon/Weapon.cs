using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UnityEvent의 클래스의 일반화 정의에 따라 호출할 수 있는 이벤트 메소드의 ㅐㅁ개변수가 결정된다
/// </summary>
[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

/// <summary>
/// 무기의 탄창 수 정보가 바뀔 때마다 외부에 있는 메소드를 자동 호출할수있도록 이벤트 클래스 생성
/// </summary>
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// 탄 이벤트 변수
    /// </summary>
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();

    /// <summary>
    /// 탄창 수 이벤트 변수
    /// </summary>
    [HideInInspector]
    public MagazineEvent onMagazineEvent = new MagazineEvent();

    [Header("Fire Effect")]
    /// <summary>
    /// 총구에서 나오는 이펙트 변수
    /// </summary>
    [SerializeField]
    GameObject fireEffect;

    [Header("Spawn Points")]
    /// <summary>
    /// 탄피 생성 위치
    /// </summary>
    [SerializeField]
    Transform casingSpawnPoint;

    /// <summary>
    /// 총알 생성 위치
    /// </summary>
    [SerializeField]
    Transform bulletSpawnPoint;

    [Header("Weapon Setting")]
    /// <summary>
    /// 무기 설정을 위한 변수
    /// </summary>
    [SerializeField]
    WeaponSetting weaponSetting;

    [Header("Aim UI")]
    /// <summary>
    /// 에임 모드에 따라 Aim이미지 활성화 비활성화
    /// </summary>
    [SerializeField]
    Image imageAim;


    /// <summary>
    /// 마지막 발사 시간 체크용 변수
    /// </summary>
    float lastAttackTime = 0.0f;

    /// <summary>
    /// 재장전 중인지 체크용 함수(ture : 재장전 중, false : 재장전 중 아님)
    /// </summary>
    bool isReload = false;

    /// <summary>
    /// 공격 여부 체크용 변수
    /// </summary>
    bool isAttack = false;

    /// <summary>
    /// 모드 전환 체크용 변수
    /// </summary>
    bool isModeChange = false;

    /// <summary>
    /// 기본 모드에서 카메라 화각(FOV)
    /// </summary>
    float defaultModeFOV = 60.0f;

    /// <summary>
    /// aim모드에서의 카메라 화각(FOV)
    /// </summary>
    float aimModeFOV = 30.0f;

    /// <summary>
    /// 발사 애니메이션을 제어 하기위한 변수
    /// </summary>
    PlayerAnimatorController animator;

    /// <summary>
    /// 탄피 생성 후 활성화/비활설화 관리
    /// </summary>
    CasingMemoryPool casingMemoryPool;

    /// <summary>
    /// 공격 효과 생성 후 활성화/비활성화 관리
    /// </summary>
    ImpactMemoryPool impactMemoryPool;

    /// <summary>
    /// 광선 발사를 위한 카메라
    /// </summary>
    Camera mainCamera;

    /// <summary>
    /// 외부에서 필요한 무기 이름 정보를 보기 위한 프로퍼티
    /// </summary>
    public WeaponName WeaponName => weaponSetting.WeaponName;

    /// <summary>
    /// 외부에서 필요한 현재 탄창 수 정보를 보기 위한 프로퍼티
    /// </summary>
    public int CurrentMagazine => weaponSetting.currentMagazine;

    /// <summary>
    /// 외부에서 필요한 최대 탄창 수 정보를 보기 위한 프로퍼티
    /// </summary>
    public int MaxMagazine => weaponSetting.maxMagazine;

    private void Awake()
    {
        animator = GetComponentInParent<PlayerAnimatorController>(); // 부모 오브젝트에 있는 플레이어 오브젝트에 있는 PlayerAnimatorController 찾기
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        // 처음 탄창 수는 최대 탄창 수로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // 처음 탄 수는 최대 탄수로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        fireEffect.SetActive(false); // 총알 발사 이펙트 비활성화

        // 무기가 활성화 될 때 해당 무기의 탄창 수를 갱신 한다
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        // 무기가 활성화 될 때 해당 무기의 탄 수를 갱신 한다
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariables();
    }


    /// <summary>
    /// 공격시 실행할 함수
    /// </summary>
    /// <param name="type">받은 값</param>
    public void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 떄는 무기 액션 불가능
        if (isReload == true) return;

        // 모드 전환 중이면 무기 액션 불가능
        if(isModeChange == true) return;

        // 마우스 왼쪽 클릭 (공격 시작)
        if (type == 0)
        {
            // 연속 공격
            if(weaponSetting.isAutomaticAttack == true)
            {
                isAttack = true; // 연속 공격 중 설정
                StartCoroutine("OnAttackLoop"); // 연사 공격 코루틴 시작
            }
            // 단발 공격
            else
            {
                OnAttack(); // 단발 공격 함수 호출
            }
        }
        // 마우스 오른쪽 클릭 (모드 전환)
        else
        {
            // 공격 중일 떄는 모드 전환 불가능
            if (isAttack == true) return;

            StartCoroutine("OnModeChage"); // 에임 모드 전환 코루틴 실행
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
            isAttack = false;   // 공격 중 아님 설정
            StopCoroutine("OnAttackLoop");  // 연사 공격 코루틴 정지
        }
    }

    /// <summary>
    /// 재장전 실행 시키는 함수
    /// </summary>
    public void StartReload()
    {
        // 현재 재장전 중이거나 탄창 수가 0이면 재장전 불가능
        if(isReload == true || weaponSetting.currentMagazine <= 0) return;

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
            //animator.Play("Fire", -1, 0); // 같은 애니메이션을 반복할 때 애니메이션을 끊고 처음부터 다시 재생
            //animator.Play("Fire"); // 같은 애니메이션을 반복할 때 중간에 끊지 못하고 재생 완료 후 다시 재생

            // 무기 애니메이션 재생 (모드에 따라 AimFire 또는 Fire 애니메이션 재생)
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);

            // 총구 이펙트는 default 모드일 때만 재생
            if (animator.AimModeIs == false) StartCoroutine("FireEffect");

            StartCoroutine("FireEffect"); // 총알 발사시 이펙트 코루틴 시작

            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right); // 탄피 생성

            // 광선을 발사해 원하는 위치 공격과 임펙트 효과 함수 실행
            TwoStepRaycast();
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
                isReload = false;   // 재장전 중 아님 설정

                // 현재 탄창 수를 1감소
                weaponSetting.currentMagazine--;
                // 바뀐 탄창 수 정보를 Text UI에 갱신
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // 현재 탄 수를 최대로 설정
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                // 바뀐 탄 수 정보를 Text UI에 갱신
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 실제 발사되는 위치와 내가 기준으로 하는 타겟의 위치가 달라 타격 불가능 그래서
    /// 에임이 있는 화면 중앙위치를 관통하는 광선을 발사
    /// 발사한 광선의 정보로 총구에서 다시 광선 발사
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // 화면의 중앙 좌표 Aim 기주으로 Raycast 연산
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        // 공격 사거리안에 부딪히는 오브젝트가 있으면 
        if(Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;    // targetPoint는 광선에 부딪힌 위치
        }
        // 공격 사거리 안에 부딪히는 오브젝트가 없으면 
        else
        {
            // targetPoint는 최대 사거리
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        // 화면 중앙 Raycast 선 그리기(확인용)
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        // 위에 Raycast 연산으로 얻어진 targetPoint를 목표 지점으로 설정
        // (위에서 얻은 타겟포인트 - 총알 생성 위치)정규화 = 공격 방향
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        // 총구를 시작지점으로 해서 Raycast
        if(Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if(hit.transform.CompareTag("ImpactEnemy")) // 닿은 대상의 태그가 ImpactEnemy이면
            {
                // 닿은 대상의 위치에있는 EnemyStatus 컴포넌트에 TakeDamage에 파라메터를 weaponSetting.damage 주고 호출
                hit.transform.GetComponent<EnemyStatus>().TakeDamage(weaponSetting.damage);
            }
        }
        // 총구 앞 Raycast 선 그리기(확인용)
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
    }

    /// <summary>
    /// 에임 모드 변환하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator OnModeChage()
    {
        float current = 0;
        float percent = 0;
        float time = 0.35f;

        animator.AimModeIs = !animator.AimModeIs; // 에임 모드 파라미터가 참이면 거짓, 거짓이면 참
        imageAim.enabled = !imageAim.enabled;       // 에임 이미지가 보이면 보이지 않게, 보이지 않으면 보이게 설정

        float start = mainCamera.fieldOfView;   // 현재 카메라의 FOV
        float end = animator.AimModeIs == true ? aimModeFOV : defaultModeFOV; // 에임모드 파라미터에 따라 에임FOV 아니면 e디폴트FOV로 설정

        isModeChange = true; // 모드 변경 중

        while(percent < 1)
        {
            current += Time.deltaTime; // 시간 설정
            percent = current / time;

            // a모드에 따라 카메라의 시야각을 변경
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;   // 모드 변경 중 아님
    }

    /// <summary>
    /// 장전, 공격, 모드 초기화
    /// </summary>
    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }
}


