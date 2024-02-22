using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMouse : MonoBehaviour
{
    /// <summary>
    /// 카멜라 x축 회전 속도
    /// </summary>
    public float rotateCamXAxisSpeed = 5.0f;

    /// <summary>
    /// 카메라 y축 회전 속도
    /// </summary>
    public float rotateCamYAxisSpeed = 3.0f;

    /// <summary>
    /// 카메라 x축 최소 회전 범위
    /// </summary>
    float minX = -80.0f;    

    /// <summary>
    /// 카메라 x축 최대 회전 범위
    /// </summary>
    float maxX = 50.0f;
    
    /// <summary>
    /// 카메라 x축 최소 회전 범위
    /// </summary>
    //float minY = -80.0f;    

    /// <summary>
    /// 카메라 x축 최대 회전 범위
    /// </summary>
    //float maxY = 80.0f;     

    /// <summary>
    /// 회전할 x축 저장할 변수
    /// </summary>
    float eulerAngleX;

    /// <summary>
    /// 회전할 y축 저장할 변수
    /// </summary>
    float eulerAngleY;      

    /// <summary>
    /// 카메라 회전을 제어할 함수
    /// </summary>
    /// <param name="mouseX">마우스 x축</param>
    /// <param name="mouseY">마우스 y축</param>
    public void UpdateRotate(float mouseX, float mouseY)
    {
        // (마우스를 아래로 내리면 -로 음수인데 오브젝트의 x축이 +방향으로 회전해야 아래로 보이기 떄문에 eulerAngleY -= ... 설정
        eulerAngleY += mouseX * rotateCamYAxisSpeed;    // 마우스 좌우 이동으로 카메라 y축 회전
        eulerAngleX -= mouseY * rotateCamXAxisSpeed;    // 마우스 위아래 이동으로 카메라 y축 회전

        // 카메라 x축 회전의 경우 회전 범위를 설정
        eulerAngleX = ClampAngle(eulerAngleX, minX, maxX);
        //eulerAngleY = ClampAngle(eulerAngleY, minY, maxY);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0); // 회전 하기
    }

    /// <summary>
    /// 회전에 범위의 최대 최소 설정하는 함수
    /// </summary>
    /// <param name="angle">원래 각도</param>
    /// <param name="min">최소 각도</param>
    /// <param name="max">최대 각도</param>
    /// <returns></returns>
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360.0f;
        if (angle > 360) angle -= 360.0f;

        return Mathf.Clamp(angle, min, max); 
    }
    
}
