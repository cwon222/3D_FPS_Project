using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ���� ���� ���¸� ��Ÿ���� enum
/// </summary>
public enum EnemyState
{
    None = -1,      // �� ����
    Idle = 0,       // �� ���
    Wander,         // �� ��ȸ
    Pursuit,        // �� ����
    Attack          // �� ����
}

/// <summary>
/// ���� �ൿ�� �����ϴ� ��ũ��Ʈ
/// </summary>
public class EnemyStatus : MonoBehaviour
{
    [Header("Pursuit")]
    /// <summary>
    /// �ν� ���� (���� ������ ������ ���� ���·� ����)
    /// </summary>
    [SerializeField]
    float targetRecognition = 8;

    /// <summary>
    /// ���� ���� (���� ������ ������ ��ȸ ���·� ����)
    /// </summary>
    [SerializeField]
    float trackingRange = 10;

    [Header("Attack")]
    /// <summary>
    /// �߻�ü ������Ʈ�� ���� ��
    /// </summary>
    [SerializeField]
    GameObject projectilePrefab;

    /// <summary>
    /// �߻�ü�� ���� ��ġ
    /// </summary>
    [SerializeField]
    Transform projectileSpawnPoint;

    /// <summary>
    /// ���� ����(���� �ȿ� ������ ���� ���·� ����)
    /// </summary>
    [SerializeField]
    float attackRange = 5.0f;

    /// <summary>
    /// ���� �ӵ�
    /// </summary>
    [SerializeField]
    float attackRate = 2.0f;

    /// <summary>
    ///  ���� ���� �ൿ
    /// </summary>
    EnemyState enemyState = EnemyState.None;
    
    /// <summary>
    /// ���� �ֱ� ���� ����(������ �߻� �ð� ����)
    /// </summary>
    float lastAttackTime = 0.0f;

    /// <summary>
    /// ���� ����
    /// </summary>
    Status status;

    /// <summary>
    /// �̵� ��� ���� NavMeshAgent ����
    /// </summary>
    NavMeshAgent navMeshAgent;

    /// <summary>
    /// ������ ���
    /// </summary>
    Transform target;

    //private void Awake()
    public void Setup(Transform target)
    {
        status = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        this.target = target; // Ÿ�� ����

        // NavMeshAgent ������Ʈ���� ȸ���� ������Ʈ ���ϰ� ����
        navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        // ���� Ȱ��ȭ�� �� ���� ���¸� ��� ���·� ����
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        // ���� ��Ȱ��ȭ�� �� ���� ������� ���¸� ����
        StopCoroutine(enemyState.ToString());

        // ���� ���¸� None���� ����
        enemyState = EnemyState.None;
    }

    /// <summary>
    /// ���� ���¸� �����ϴ� �Լ�
    /// </summary>
    /// <param name="newState">������ ����</param>
    public void ChangeState(EnemyState newState)
    {
        // ���� ������� ���¿� �ٲٷ��� �ϴ� ���°� ������ return
        if(enemyState == newState) return;

        // ������ ������� ���� ����
        StopCoroutine(enemyState.ToString());

        // ���� ���� ���¸� �ٲ� ���·� ����
        enemyState = newState;

        // �ٲ� ���� ���
        StartCoroutine(enemyState.ToString());
    }

    /// <summary>
    /// ��� ������ �� �ൿ�ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator Idle()
    {
        // ��ٷȴٰ� ��ȸ ���·� �����ϴ� �ڷ�ƾ ����
        StartCoroutine("AutoChangeFromIdleToWande");

        while(true)
        {
            // ��� ������ �� �ϴ� �ൿ
            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, �߰�, ���Ÿ� ����)
            SelecteState();

            yield return null;
        }
    }

    /// <summary>
    /// ��⿡�� ��ȸ ���·� �ٲ�� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator AutoChangeFromIdleToWande()
    {
        // 1 ~ 4�� �ð� ��ٸ��� ���� ����
        int chageTime = Random.Range(1, 5);

        // ���� �ð����� ��ٸ���
        yield return new WaitForSeconds(chageTime);

        // ���¸� ��ȸ ���·� ����
        ChangeState(EnemyState.Wander);
    }

    /// <summary>
    /// ��ȸ ������ �� �ൿ�� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>

    IEnumerator Wander()
    {
        float currentTime = 0.0f;   // ���� ��ȸ �ð�
        float maxTime = 10.0f;      // �ִ� ��ȸ �ð�

        // �̵��ӵ� ����
        navMeshAgent.speed = status.WalkSpeed;
        
        // ��ǥ ��ġ ����
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // ��ǥ ��ġ�� ȸ���ϱ�
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);    // ��ǥ ��ġ
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);              // ���� ��ġ
        // ��ǥ ��ġ - ���� ��ġ = ���� ����, ���� ���� ���ͷ� ȸ���ϱ�
        transform.rotation = Quaternion.LookRotation(to - from);   
        
        while(true)
        {
            currentTime += Time.deltaTime;

            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);    // ��ǥ ��ġ
            from = new Vector3(transform.position.x, 0, transform.position.z);              // ���� ��ġ

            if((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                // ��ǥ ��ġ�� �����ϰų� �ִ� �ð����� ��ȸ ���̸� 
                ChangeState(EnemyState.Idle);   // ��� ���·� ���� 
            }

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, �߰�, ���Ÿ� ����)
            SelecteState();

            yield return null;
        }
    }

    Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;    // ����  ��ġ�� �������� �ϴ� ���� ������ ����
        int wanderAngle = 0;        // ���õ� ����
        int WanderAngleMin = 0;     // �ּ� ����
        int WanderAngleMax = 360;     // �ִ� ����

        // ���� �� ĳ���Ͱ� �ִ� ���� �߽��� ��ġ
        Vector3 rangePosition = Vector3.zero;
        // ���� �� ĳ���Ͱ� �ִ� ���� �߽��� ũ��
        Vector3 rangeScale = Vector3.one * 100.0f;

        // ���õ� ������ �ּҿ��� �ִ���� ����
        wanderAngle = Random.Range(WanderAngleMin, WanderAngleMax);
        // �ڽ��� ��ġ �߽����� ������ �Ÿ�, ���õ�  ������ ��ġ�� ��ǥ�� ��ǥ �������� ����
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderAngle);

        // ������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
        // x��ǥ
        targetPosition.x = Mathf.Clamp(targetPosition.x,
            rangePosition.x - rangeScale.x * 0.5f,
            rangePosition.x + rangeScale.x * 0.5f);
        // y��ǥ
        targetPosition.y = 0.0f;
        // z ��ǥ
        targetPosition.z = Mathf.Clamp(targetPosition.x,
            rangePosition.z - rangeScale.z * 0.5f,
            rangePosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    /// <summary>
    /// �� �ѷ��� ��ġ�� �����ִ� �Լ�
    /// </summary>
    /// <param name="radius">������</param>
    /// <param name="angle">����</param>
    /// <returns>�� �ѷ��� ����</returns>
    Vector3 SetAngle(float radius, float angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    /// <summary>
    /// Ÿ���� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator Pursuit()
    {
        while (true)
        {
            // ��ȸ�� ���� �ȴ� �ӵ�, ������ ���� �ٴ� �̵� �ӵ�
            navMeshAgent.speed = status.RunSpeed;

            // ��ǥ��ġ�� ���� �÷��̾� ��ġ�� ����
            navMeshAgent.SetDestination(target.position);

            // Ÿ�� ������ ��� ���� �����
            LookRotateTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, �߰�, ���Ÿ� ����)
            SelecteState();

            yield return null;
        }
    }

    /// <summary>
    /// ������ �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator Attack()
    {
        // ������ ���� �̵��� ���ߵ��� ����
        navMeshAgent.ResetPath();

        while (true)
        {
            // ������ ���� Ÿ���� �ٶ󺸱�
            LookRotateTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, �߰�, ����)
            SelecteState();

            if(Time.time - lastAttackTime > attackRate) // ���� �ֱⰡ ������
            {
                // ���� �ð� ����
                lastAttackTime = Time.time;

                // �߻�ü ���� (projectilePrefab ������Ʈ�� projectileSpawnPoint�� ��ġ�� ������ ����)
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                // clone ���� EnemyProjectile ������Ʈ�� Setup�� Ÿ����ġ �߰�
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
            }

            yield return null;
        }
    }

    /// <summary>
    /// Ÿ���� ��� ���� ����� �Լ�
    /// </summary>
    private void LookRotateTarget()
    {
        // ��ǥ ��ġ ����
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        // �� ���� ��ġ
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // �ٷ� ����
        transform.rotation = Quaternion.LookRotation(to - from);
        // õõ�� ����
        //Quaternion rotation = Quaternion.LookRotation(to - from);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }

    /// <summary>
    /// Ÿ�ٰ��� �Ÿ��� ���� �ൿ�� �ϴ� �Լ�(��ȸ, �߰�, ���Ÿ� ����)
    /// </summary>
    private void SelecteState()
    {
        if(target == null) return;  // Ÿ���� ������ ����

        // Ÿ�ٰ� ���� �Ÿ� ���
        float distance = Vector3.Distance(target.position, transform.position);

        if(distance <= attackRange) // Ÿ�ٰ� �Ÿ��� ���� �������� ������
        {
            ChangeState(EnemyState.Attack); // ���� ���·� ����
        }
        else if(distance <= targetRecognition) // Ÿ�ٰ� �Ÿ��� �ν� ���� ���� ������
        {
            ChangeState(EnemyState.Pursuit);    // ���� ���·� ����
        }
        else if(distance >= trackingRange)  // Ÿ�ٰ� �Ÿ��� ���� ���� ���� ũ��
        {
            ChangeState(EnemyState.Wander);     // ��ȸ ���·� ����
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        // ��ȸ ���� �϶� �̵��� ��� ǥ��
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

        Gizmos.color = Color.red;
        // ��ǥ �ν� ���� ǥ��
        Gizmos.DrawWireSphere(transform.position, targetRecognition);

        Gizmos.color = Color.green;
        // ���� ����
        Gizmos.DrawWireSphere(transform.position, trackingRange);

        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        // ���� ���� ����
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
