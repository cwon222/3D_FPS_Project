/// <summary>
/// ������ ������ �������� �� �������� ����ϴ� �������� ����ü�� ���� ��ũ��Ʈ
/// ���� �߰�, ������ �����ϱ� ������ ���
/// System.Serializable �� ����ؼ� ����ȭ�� �ؼ� �ν����� â�� ��� �������� ����� ����
/// </summary>

public enum WeaponName
{
    Rifle = 0   // ������
}

[System.Serializable]
public struct WeaponSetting
{
    /// <summary>
    /// ���� �̸�
    /// </summary>
    public WeaponName WeaponName;

    /// <summary>
    /// ���� źâ ��
    /// </summary>
    public int currentMagazine;

    /// <summary>
    /// �ִ� źâ ��
    /// </summary>
    public int maxMagazine;

    /// <summary>
    /// ���� ź�� ��
    /// </summary>
    public int currentAmmo;

    /// <summary>
    /// �ִ� ź�� ��
    /// </summary>
    public int maxAmmo;

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
