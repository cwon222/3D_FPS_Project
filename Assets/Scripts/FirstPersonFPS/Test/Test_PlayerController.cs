using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾ �����ϴ� ��ũ��Ʈ
/// </summary>
public class Test_PlayerController : MonoBehaviour
{
    PlayerInputActionFPS inputAction;

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

    /// <summary>
    /// ���⸦ ����ؼ� ������ �����ϱ� ���� ������Ʈ�� ã�� ���� ����
    /// </summary>
    Weapon weapon;

    Vector2 movePos;
    float run;
    bool isAttack;
    bool isAim;
    bool isReload;

    private void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ� ����
        Cursor.visible = false;
        // ���� ��ġ�� ����
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateMouse>();            // RotateMouse ������Ʈ ã��
        movement = GetComponent<PlayerMovementController>();    // PlayerMovementController ������Ʈ ã��
        status = GetComponent<Status>();                        // Status ������Ʈ ã��
        animator = GetComponent<PlayerAnimatorController>();    // PlayerAnimatorController ������Ʈ ã��
        weapon = GetComponentInChildren<Weapon>();              // �ڽ� ������Ʈ �ȿ� �ִ� Weapon ������Ʈ ã��

        inputAction = new();
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += OnWalkStart;
        inputAction.Player.Move.canceled += OnWalkStart;
        inputAction.Player.Run.performed += OnRunStart;
        inputAction.Player.Run.canceled += OnRunStart;
        inputAction.Player.Jump.performed += Onjump;
        inputAction.Player.Jump.canceled += Onjump;
        inputAction.Player.Fire.performed += OnFireStart;
        inputAction.Player.Fire.canceled += OnFireEnd;
        inputAction.Player.Aim.performed += OnAimStart;
        inputAction.Player.Aim.canceled += OnAimEnd;
        inputAction.Player.Reload.performed += OnReloadStart;
        inputAction.Player.Reload.canceled += OnReloadEnd;
    }

    
    private void OnDisable()
    {
        inputAction.Player.Reload.canceled -= OnReloadEnd;
        inputAction.Player.Reload.performed -= OnReloadStart;
        inputAction.Player.Aim.canceled -= OnAimEnd;
        inputAction.Player.Aim.performed -= OnAimStart;
        inputAction.Player.Fire.canceled -= OnFireEnd;
        inputAction.Player.Fire.performed -= OnFireStart;
        inputAction.Player.Jump.canceled -= Onjump;
        inputAction.Player.Jump.performed -= Onjump;
        inputAction.Player.Run.canceled -= OnRunStart;
        inputAction.Player.Run.performed -= OnRunStart;
        inputAction.Player.Move.canceled -= OnWalkStart;
        inputAction.Player.Move.performed -= OnWalkStart;
        inputAction.Player.Disable();
    }
    private void OnReloadEnd(InputAction.CallbackContext context)
    {
        isReload = false;
    }

    private void OnReloadStart(InputAction.CallbackContext context)
    {
        isReload = true;
    }


    private void OnAimEnd(InputAction.CallbackContext context)
    {
        isAim = false;
    }

    private void OnAimStart(InputAction.CallbackContext context)
    {
        isAim = true;
    }

    private void OnFireEnd(InputAction.CallbackContext context)
    {
        isAttack = false;
    }

    private void OnFireStart(InputAction.CallbackContext context)
    {
        isAttack = true;
    }

    private void Onjump(InputAction.CallbackContext context)
    {
        movement.jump(); // ���� ����
    }

    private void OnRunStart(InputAction.CallbackContext context)
    {
        run = context.ReadValue<float>();
    }

    private void OnWalkStart(InputAction.CallbackContext context)
    {
       movePos  = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        UpdateRotate();         // ���콺 �̵�
        UpdateMove();           // �÷��̾� �̵� ����
        UpdateWeaponAction(); ; // ���� ����
    }


    /// <summary>
    /// ���콺�� X,Y�� �̵����� �ޱ� ���� �Լ�
    /// </summary>
    void UpdateRotate()
    {
        // https://youtu.be/ivPAG6ruf00?si=YsuWsvFe8Ruh-aS_&t=1102
        float mouseX = Input.GetAxis("Mouse X"); // ���콺 X�� �̵� ��
        float mouseY = Input.GetAxis("Mouse Y"); // ���콺 Y�� �̵� ��
        

        rotateToMouse.UpdateRotate(mouseX, mouseY); // �̵��� ���콺�� X,Y�� ���� �Ű������� �ֱ�
    }

    /// <summary>
    /// �÷��̾� �̵� �� �޾� �̵� ���� �̵� ��Ű�� �Լ�
    /// </summary>
    void UpdateMove()
    {
        float x = movePos.x;
        float z = movePos.y;

        if (x != 0 || z != 0) // �̵� ���� ��(�ȱ� �ƴϸ� �ٱ�)
        {
            bool isRun = false; //�޸��� ��ư�� ������ ture �ƴϸ� false

            // ���̳� �ڷ� �̵��� ���� �޸� �� ����
            if (z > 0) // ������ �̵� ���� ��
            {
                isRun = run == 1 ? true : false;
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
    /// �÷��̾��� �ൿ�� �����ϴ� �Լ�
    /// </summary>
    void UpdateWeaponAction()
    {
        if (isAttack == true) // ���콺 ��ư�� ������
        {
            weapon.StartWeaponAction(); // ���� ���� �Լ� ����
        }
        else if (isAttack == false) // ���콺 ��ư�� ����
        {
            weapon.StopWeaponAction();  // ���� ���� �Լ� ����
        }

        if (isAim == true) // ���콺 ������ ��ư ������ 
        {
            weapon.StartWeaponAction(1);    // ���� ��� ����
        }
        else if (isAim == false) // ���콺 ������ ��ư ���
        {
            weapon.StopWeaponAction(1);     // ���� ��� ����
        }

        if (isReload == true) // RŰ ������
        {
            weapon.StartReload();       // ������ �Լ� ����
        }
    }
}
