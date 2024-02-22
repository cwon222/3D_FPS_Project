using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    /// <summary>
    /// 마우스 이동으로 카메라 회전
    /// </summary>
    RotateMouse rotateToMouse;


    private void Awake()
    {
        // 마우스 커서를 보이지 않게 설정
        Cursor.visible = false;
        // 현재 위치에 고정
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateMouse>(); // RotateMouse 컴포넌트 찾기
    }

    private void Update()
    {
        UpdateRotate();
    }


    /// <summary>
    /// 마우스의 X,Y의 이동값을 받기 위한 함수
    /// </summary>
    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X"); // 마우스 X의 이동 값
        float mouseY = Input.GetAxis("Mouse Y"); // 마우스 Y의 이동 값

        rotateToMouse.UpdateRotate(mouseX, mouseY); // 이동한 마우스의 X,Y의 값을 매개변수로 넣기
    }
}
