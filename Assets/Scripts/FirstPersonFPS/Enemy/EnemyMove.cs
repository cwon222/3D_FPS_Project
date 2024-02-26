using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ���� ���� ���¸� ��Ÿ���� enum
/// </summary>
public enum EnemyState
{
    None = -1,
    Idle = 0,
    Wander,
}

/// <summary>
/// ���� �ൿ�� �����ϴ� ��ũ��Ʈ
/// </summary>
public class EnemyMove : MonoBehaviour
{
    /// <summary>
    ///  ���� ���� �ൿ
    /// </summary>
    EnemyState enemyState = EnemyState.None;

    /// <summary>
    /// ���� ����
    /// </summary>
    Status status;

    /// <summary>
    /// �̵� ��� ���� NavMeshAgent ����
    /// </summary>
    NavMeshAgent navMeshAgent;

    private void Awake()
    {
        status = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();

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

            yield return null;
        }
    }

    Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;    // ����  ��ġ�� �������� �ϴ� ���� ������ ����
        int wanderAngle = 0;        // ���õ� ����
        int WanderAngleMin = 0;     // �ּ� ����
        int WanderAngleMax = 0;     // �ִ� ����

        // ���� �� ĳ���Ͱ� �ִ� ���� �߽��� ��ġ
        Vector3 rangePosition = Vector3.zero;
        // ���� �� ĳ���Ͱ� �ִ� ���� �߽��� ũ��
        Vector3 rangeScale = Vector3.one * 100.0f;

        // ���õ� ������ �ּҿ��� �ִ���� ����
        wanderAngle = Random.Range(WanderAngleMin, WanderAngleMax);
        // �ڽ��� ��ġ �߽����� ������ �Ÿ�, ���õ�  ������ ��ġ�� ��ǥ�� ��ǥ �������� ����
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderAngle);

        // ������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
        targetPosition.x = Mathf.Clamp(targetPosition.x,
            rangePosition.x - rangeScale.x * 0.5f,
            rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        // ��ȸ ���� �϶� �̵��� ��� ǥ��
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);
    }
}
