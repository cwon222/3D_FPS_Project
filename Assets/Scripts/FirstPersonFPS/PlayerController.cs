using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ �����ϴ� ��ũ��Ʈ
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// �޸��� Ű ����
    /// </summary>
    KeyCode keyCodeRun = KeyCode.LeftShift;

    /// <summary>
    /// ���� Ű ����
    /// </summary>
    KeyCode keyCodeJump = KeyCode.Space;

    /// <summary>
    /// ���콺 �̵����� ī�޶� ȸ��
    /// </summary>
    RotateMouse rotateToMouse;

    /// <summary>
    /// Ű���� �Է����� �÷��̾� �̵�, ����
    /// </summary>
    PlayerMovementController movement; 

    /// <summary>
    /// �÷��̾��� �̵��ӵ� ����
    /// </summary>
    Status status;

    /// <summary>
    /// �÷��̾� ���ϸ��̼� ���� �ϱ� ���� ã�� ������Ʈ ����
    /// </summary>
    PlayerAnimatorController animator;  

    private void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ� ����
        Cursor.visible = false;
        // ���� ��ġ�� ����
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateMouse>(); // RotateMouse ������Ʈ ã��
        movement = GetComponent<PlayerMovementController>(); // PlayerMovementController ������Ʈ ã��
        status = GetComponent<Status>();                        // Status ������Ʈ ã��
        animator = GetComponent<PlayerAnimatorController>();    // PlayerAnimatorController ������Ʈ ã��
    }

    private void Update()
    {
        UpdateRotate(); // ���콺 �̵�
        UpdateMove();   // �÷��̾� �̵� ����
        UpdateJump();   // ���� ����
    }


    /// <summary>
    /// ���콺�� X,Y�� �̵����� �ޱ� ���� �Լ�
    /// </summary>
    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X"); // ���콺 X�� �̵� ��
        float mouseY = Input.GetAxis("Mouse Y"); // ���콺 Y�� �̵� ��

        rotateToMouse.UpdateRotate(mouseX, mouseY); // �̵��� ���콺�� X,Y�� ���� �Ű������� �ֱ�
    }

    /// <summary>
    /// �÷��̾� �̵� �� �޾� �̵� ���� �̵� ��Ű�� �Լ�
    /// </summary>
    void UpdateMove()
    {
        float x = Input.GetAxis("Horizontal");  // �յ� �� �ޱ�
        float z = Input.GetAxis("Vertical");    // �翷 �� �ޱ�

        if (x != 0 || z != 0) // �̵� ���� ��(�ȱ� �ƴϸ� �ٱ�)
        {
            bool isRun = false; //�޸��� ��ư�� ������ ture �ƴϸ� false

            // ���̳� �ڷ� �̵��� ���� �޸� �� ����
            if (z > 0) // ������ �̵� ���� ��
            {
                isRun = Input.GetKey(keyCodeRun); // ����Ʈ Ű�� �����ſ� ���� true �Ǵ� false
            }
            // isRun == true �̸� RunSpeed(�ٴ� �ӵ�) // isRun == false �̸� WalkSpeed (�ȴ� �ӵ�)
            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            // �޸��� ���̸� �ִϸ����� MoveSpeed �ؽ��� 1(�ٴ� �ִϸ��̼�)
            // �ȴ� ���̸� �ִϸ����� MoveSpeed �ؽ��� 0.5(�ȴ� �ִϸ��̼�)
            animator.MoveSpeed = isRun == true ? 1 : 0.5f; 
        }
        else // ���� ���� ��
        {
            movement.MoveSpeed = 0; // �̵��ӵ� 0
            animator.MoveSpeed = 0; // �ִϸ����� MoveSpeed �ؽ��� 0(Idle �ִϸ��̼�)
        }

        movement.MoveTo(new Vector3(x, 0, z));  // ���� �̵� ���� ���� �÷��̾� �̵�
    }

    /// <summary>
    /// �÷��̾ ���� ���� ��Ű�� �Լ�
    /// </summary>
    void UpdateJump()
    {
        if(Input.GetKey(keyCodeJump)) // �����̽� �� ������
        {
            movement.jump(); // ���� ����
        }
    }
}
