using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2f; // 마우스 감도
    public float minYAngle = -45f; // 최소 Y축 각도
    public float maxYAngle = 80f; // 최대 Y축 각도
    public float minXAngle = -60f; // 최소 X축 각도
    public float maxXAngle = 60f; // 최대 X축 각도

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // 마우스 커서를 중앙에 고정
    }

    void Update()
    {
        // 마우스 입력을 받아 카메라 회전량을 계산
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 회전량을 각도에 반영하고 최대/최소 범위 내에 클램핑
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minYAngle, maxYAngle);

        rotationY += mouseX;
        rotationY = Mathf.Clamp(rotationY, minXAngle, maxXAngle);

        // 카메라의 회전을 적용
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }



    // https://acredev.tistory.com/18
    //public float turnSpeed = 4.0f; // 마우스 회전 속도    
    //private float xRotate = 0.0f; // 내부 사용할 X축 회전량은 별도 정의 ( 카메라 위 아래 방향 )

    //void Update()
    //{
    //    // 좌우로 움직인 마우스의 이동량 * 속도에 따라 카메라가 좌우로 회전할 양 계산
    //    float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;
    //    // 현재 y축 회전값에 더한 새로운 회전각도 계산
    //    float yRotate = transform.eulerAngles.y + yRotateSize;

    //    // 위아래로 움직인 마우스의 이동량 * 속도에 따라 카메라가 회전할 양 계산(하늘, 바닥을 바라보는 동작)
    //    float xRotateSize = -Input.GetAxis("Mouse Y") * turnSpeed;
    //    // 위아래 회전량을 더해주지만 -45도 ~ 80도로 제한 (-45:하늘방향, 80:바닥방향)
    //    // Clamp 는 값의 범위를 제한하는 함수
    //    xRotate = Mathf.Clamp(xRotate + xRotateSize, -45, 80);

    //    // 카메라 회전량을 카메라에 반영(X, Y축만 회전)
    //    transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
    //}
}
