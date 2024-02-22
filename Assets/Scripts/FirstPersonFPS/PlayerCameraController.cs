using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    /// <summary>
    /// ���콺 �̵����� ī�޶� ȸ��
    /// </summary>
    RotateMouse rotateToMouse;


    private void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ� ����
        Cursor.visible = false;
        // ���� ��ġ�� ����
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateMouse>(); // RotateMouse ������Ʈ ã��
    }

    private void Update()
    {
        UpdateRotate();
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
}
