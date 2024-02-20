using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2f; // ���콺 ����
    public float minYAngle = -45f; // �ּ� Y�� ����
    public float maxYAngle = 80f; // �ִ� Y�� ����
    public float minXAngle = -60f; // �ּ� X�� ����
    public float maxXAngle = 60f; // �ִ� X�� ����

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ���� �߾ӿ� ����
    }

    void Update()
    {
        // ���콺 �Է��� �޾� ī�޶� ȸ������ ���
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // ȸ������ ������ �ݿ��ϰ� �ִ�/�ּ� ���� ���� Ŭ����
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minYAngle, maxYAngle);

        rotationY += mouseX;
        rotationY = Mathf.Clamp(rotationY, minXAngle, maxXAngle);

        // ī�޶��� ȸ���� ����
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }



    // https://acredev.tistory.com/18
    //public float turnSpeed = 4.0f; // ���콺 ȸ�� �ӵ�    
    //private float xRotate = 0.0f; // ���� ����� X�� ȸ������ ���� ���� ( ī�޶� �� �Ʒ� ���� )

    //void Update()
    //{
    //    // �¿�� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� �¿�� ȸ���� �� ���
    //    float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;
    //    // ���� y�� ȸ������ ���� ���ο� ȸ������ ���
    //    float yRotate = transform.eulerAngles.y + yRotateSize;

    //    // ���Ʒ��� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� ȸ���� �� ���(�ϴ�, �ٴ��� �ٶ󺸴� ����)
    //    float xRotateSize = -Input.GetAxis("Mouse Y") * turnSpeed;
    //    // ���Ʒ� ȸ������ ���������� -45�� ~ 80���� ���� (-45:�ϴù���, 80:�ٴڹ���)
    //    // Clamp �� ���� ������ �����ϴ� �Լ�
    //    xRotate = Mathf.Clamp(xRotate + xRotateSize, -45, 80);

    //    // ī�޶� ȸ������ ī�޶� �ݿ�(X, Y�ุ ȸ��)
    //    transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
    //}
}
