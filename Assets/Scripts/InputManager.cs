using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자 입력 감지 및 관리 모듈
public class InputManager : MonoBehaviour
{
    private static InputManager m_instance; // 싱글턴 인스턴스가 할당된 static 변수

    // 싱글턴 접근용 프로퍼티
    public static InputManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<InputManager>();
            }

            return m_instance;
        }
    }

    public bool moveForward { get; private set; } // 앞쪽 이동 입력 상태값
    public bool moveBackward { get; private set; } // 뒷쪽 이동 입력 상태값
    public bool moveRight { get; private set; } // 오른쪽 이동 입력 상태값
    public bool moveLeft { get; private set; } // 왼쪽 이동 입력 상태값
    public bool speedUp { get; private set; } // 속도 증가 입력 상태값
    public bool rotateClockwise { get; private set; } // 시계방향 회전 입력 상태값
    public bool rotateAntiClockwise { get; private set; } // 반시계방향 회전 입력 상태값
    public bool zoomIn { get; private set; } // 줌인 입력 상태값
    public bool zoomOut { get; private set; } // 줌아웃 입력 상태값
    public bool mouseLeftClick { get; private set; } // 마우스 좌클릭 입력 상태값
    public bool mouseLeftDrag { get; private set; } // 마우스 좌측 드래그 입력 상태값
    public bool mouseRightClick { get; private set; } // 마우스 우클릭 입력 상태값
    public bool mouseRightDrag { get; private set; } // 마우스 우측 드래그 입력 상태값
    public float mouseScrollY { get; private set; } // 마우스 수직 스크롤 입력 상태값

    // 루프에서 사용자 입력 감지
    private void Update()
    {
        moveForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow); // 앞쪽 이동 입력 상태 감지
        moveBackward = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow); // 뒤쪽 이동 입력 상태 감지
        moveRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow); // 오른쪽 이동 입력 상태 감지
        moveLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow); // 왼쪽 이동 입력 상태 감지
        speedUp = Input.GetKey(KeyCode.LeftShift); // 속도 증가 입력 상태 감지
        rotateClockwise = Input.GetKey(KeyCode.Q); // 시계방향 회전 입력 상태 감지
        rotateAntiClockwise = Input.GetKey(KeyCode.E); // 반시계방향 회전 입력 상태 감지
        zoomIn = Input.GetKey(KeyCode.F); // 줌인 입력 상태 감지
        zoomOut = Input.GetKey(KeyCode.R); // 줌아웃 입력 상태 감지
        mouseLeftClick = Input.GetMouseButtonDown(0); // 마우스 좌클릭 입력 상태 감지
        mouseLeftDrag = Input.GetMouseButton(0); // 마우스 좌측 드래그 입력 상태 감지
        mouseRightClick = Input.GetMouseButtonDown(1); // 마우스 우클릭 입력 상태 감지
        mouseRightDrag = Input.GetMouseButton(1); // 마우스 우측 드래그 입력 상태 감지
        mouseScrollY = Input.mouseScrollDelta.y; // 마우스 수직 스크롤 값 업데이트
    }
}
