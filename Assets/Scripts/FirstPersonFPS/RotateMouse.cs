using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMouse : MonoBehaviour
{
    /// <summary>
    /// ī��� x�� ȸ�� �ӵ�
    /// </summary>
    public float rotateCamXAxisSpeed = 5.0f;

    /// <summary>
    /// ī�޶� y�� ȸ�� �ӵ�
    /// </summary>
    public float rotateCamYAxisSpeed = 3.0f;

    /// <summary>
    /// ī�޶� x�� �ּ� ȸ�� ����
    /// </summary>
    float minX = -80.0f;    

    /// <summary>
    /// ī�޶� x�� �ִ� ȸ�� ����
    /// </summary>
    float maxX = 50.0f;
    
    /// <summary>
    /// ī�޶� x�� �ּ� ȸ�� ����
    /// </summary>
    //float minY = -80.0f;    

    /// <summary>
    /// ī�޶� x�� �ִ� ȸ�� ����
    /// </summary>
    //float maxY = 80.0f;     

    /// <summary>
    /// ȸ���� x�� ������ ����
    /// </summary>
    float eulerAngleX;

    /// <summary>
    /// ȸ���� y�� ������ ����
    /// </summary>
    float eulerAngleY;      

    /// <summary>
    /// ī�޶� ȸ���� ������ �Լ�
    /// </summary>
    /// <param name="mouseX">���콺 x��</param>
    /// <param name="mouseY">���콺 y��</param>
    public void UpdateRotate(float mouseX, float mouseY)
    {
        // (���콺�� �Ʒ��� ������ -�� �����ε� ������Ʈ�� x���� +�������� ȸ���ؾ� �Ʒ��� ���̱� ������ eulerAngleY -= ... ����
        eulerAngleY += mouseX * rotateCamYAxisSpeed;    // ���콺 �¿� �̵����� ī�޶� y�� ȸ��
        eulerAngleX -= mouseY * rotateCamXAxisSpeed;    // ���콺 ���Ʒ� �̵����� ī�޶� y�� ȸ��

        // ī�޶� x�� ȸ���� ��� ȸ�� ������ ����
        eulerAngleX = ClampAngle(eulerAngleX, minX, maxX);
        //eulerAngleY = ClampAngle(eulerAngleY, minY, maxY);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0); // ȸ�� �ϱ�
    }

    /// <summary>
    /// ȸ���� ������ �ִ� �ּ� �����ϴ� �Լ�
    /// </summary>
    /// <param name="angle">���� ����</param>
    /// <param name="min">�ּ� ����</param>
    /// <param name="max">�ִ� ����</param>
    /// <returns></returns>
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360.0f;
        if (angle > 360) angle -= 360.0f;

        return Mathf.Clamp(angle, min, max); 
    }
    
}
