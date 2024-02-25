using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// �÷��̾��� �̵��� �����ϴ� ��ũ��Ʈ
// �� ����� ���Ե� ��ũ��Ʈ�� ���� ������Ʈ�� ������Ʈ�� �����ϸ� �ش� ������Ʈ�� ���� �߰� �ȴ�
[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    /// <summary>
    /// �̵� �ӵ�
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// �̵� ��(x, z�� y���� ������ ����� ���� �̵��� ����)
    /// </summary>
    Vector3 moveForce;

    /// <summary>
    /// ���� ��
    /// </summary>
    public float jumpForce;

    /// <summary>
    /// �߷� ��
    /// </summary>
    public float gravity;

    /// <summary>
    /// �̵��ӵ��� �����ϱ� ���� ������Ƽ
    /// </summary>
    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value); // �̵� �ӵ��� ������ �ȵǰ� ����
        get => moveSpeed;
    }

    /// <summary>
    /// �÷��̾� �̵��� �����ϱ� ���� ������Ʈ
    /// </summary>
    CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>(); // ������Ʈ ã��
    }

    private void Update()
    {
        // ���� ������Ʈ�� ���� �ٴڰ� �浹 üũ(�⵿�ϸ� true)
        if (!characterController.isGrounded) // ����� �������� 
        {
            moveForce.y += gravity * Time.deltaTime; // �߷¸�ŭ y�� �̵��ӵ� ����
        }
        characterController.Move(Time.deltaTime * moveForce); // �����Ӵ� moveForce ��ŭ �̵�
    }

    public void MoveTo(Vector3 direction)
    {

        // �̵� ���� = ĳ������ ȸ�� �� * ���� ��
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // �̵� �� = �̵� ���� * �ӵ�
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void jump()
    {
        // �÷��̾ �ٴڿ� ���� ���� ���� ����
        if(characterController.isGrounded)
        {
            moveForce.y = jumpForce; // y������ �߷¸�ŭ ����
        }
    }
}
