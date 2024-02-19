using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    /// <summary>
    /// ���콺 �ΰ���
    /// </summary>
    public float sensitivity = 500.0f;

    /// <summary>
    /// X���� ��ġ
    /// </summary>
    public float rotationX;

    /// <summary>
    /// Y���� ��ġ
    /// </summary>
    public float rotationY;

    private void Update()
    {
        float mouseMoveX = Input.GetAxis("MouseX"); // ���콺 X���� ������ ���� �޾Ƽ� mouseX�� ����
        float mouseMoveY = Input.GetAxis("MouseY"); // ���콺 Y���� ������ ���� �޾Ƽ� mouseY�� ����

        rotationY += mouseMoveX * sensitivity * Time.deltaTime;
        rotationX += mouseMoveY * sensitivity * Time.deltaTime;

        if(rotationX > 35.0f)
        {
            rotationX = 35.0f;
        }
        if(rotationX < -30.0f)
        {
            rotationX = -30.0f;
        }

        transform.eulerAngles = new Vector3(-rotationX, rotationY, 0);
    }
}





























// https://acredev.tistory.com/18#%E2%97%8F%C2%A0%20%EB%A7%88%EC%9A%B0%EC%8A%A4%20%EC%9D%B4%EB%8F%99%EC%9D%84%20%EC%9D%B8%EC%8B%9D%ED%95%B4%2C%20%ED%99%94%EB%A9%B4%EC%9D%B4%20%EB%94%B0%EB%9D%BC%20%EC%9B%80%EC%A7%81%EC%9D%B4%EB%8A%94%20%EC%8A%A4%ED%81%AC%EB%A6%BD%ED%8A%B8%20%EC%9E%91%EC%84%B1%20(MouseMove.cs)-1