/// <summary>
/// ������ ������ �������� �� �������� ����ϴ� �������� ����ü�� ���� ��ũ��Ʈ
/// ���� �߰�, ������ �����ϱ� ������ ���
/// System.Serializable �� ����ؼ� ����ȭ�� �ؼ� �ν����� â�� ��� �������� ����� ����
/// </summary>
[System.Serializable]
public struct WeaponSetting
{
    /// <summary>
    /// ���ݼӵ�
    /// </summary>
    public float attackRate;

    /// <summary>
    /// ���� ��Ÿ�
    /// </summary>
    public float attackDistance;

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public bool isAutomaticAttack;
       
}
