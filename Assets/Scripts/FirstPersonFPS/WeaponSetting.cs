/// <summary>
/// 무기의 종류가 여러개일 떄 공용으로 사용하는 변수들을 구조체로 묶은 스크립트
/// 변수 추가, 삭제가 용이하기 때문에 사용
/// System.Serializable 를 사용해서 직렬화를 해서 인스펙터 창에 멤버 변수들의 목록을 띄우기
/// </summary>
[System.Serializable]
public struct WeaponSetting
{
    /// <summary>
    /// 공격속도
    /// </summary>
    public float attackRate;

    /// <summary>
    /// 공격 사거리
    /// </summary>
    public float attackDistance;

    /// <summary>
    /// 연속 공격 여부
    /// </summary>
    public bool isAutomaticAttack;
       
}
