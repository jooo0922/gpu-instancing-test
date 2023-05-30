using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    [Header("Transforms")]
    public Transform cameraOrbitTransform;

    [Header("Camera Move")]
    public float normalSpeed = 1f; // 보통 이동속도
    public float fastSpeed = 3f; // 빠른 이동속도
    [Range(0f, 10f)]
    [Tooltip("값이 0에 가까울수록 관성효과가 강해지고, 0보다 클수록 관성효과가 약해집니다.")]
    public float moveTime = 5f; // 커매라 이동 시간 (카메라 관성)

    [Header("Camera Rotate")]
    [Range(0.1f, 3f)]
    [Tooltip("키 입력으로 회전 시, 프레임 당 단위 회전량을 설정합니다.")]
    public float rotationAmount = 1f; // 카메라 회전량
    [Range(0.1f, 20f)]
    [Tooltip("카메라 회전 감도를 조절할 수 있습니다. 값이 클수록 회전속도는 느려집니다.")]
    public float rotationSensitivity = 5f; // 카메라 회전 감도 (마우스 회전)
    [Range(5f, 25f)]
    [Tooltip("카메라 수직방향 회전 범위의 최소 각도를 설정합니다.")]
    public float minRotationX = 10f;
    [Range(65f, 85f)]
    [Tooltip("카메라 수직방향 회전 범위의 최대 각도를 설정합니다.")]
    public float maxRotationX = 80f;

    [Header("Camera Zoom")]
    [Range(1f, 10f)]
    [Tooltip("프레임 당 단위 줌값을 설정합니다.")]
    public float zoomScale = 5f;
    [Range(50f, 90f)]
    [Tooltip("카메라 최소 줌값을 설정합니다.")]
    public float minZoom = 70f;
    [Range(260f, 300f)]
    [Tooltip("카메라 최대 줌값을 설정합니다.")]
    public float maxZoom = 280f;

    private float moveSpeed = 1f; // 카메라 이동 속도
    private Vector3 newPosition; // Camera Rig 가 새로 이동할 위치
    private Quaternion newRotationX; // Camera Orbit 이 X축으로 새로 회전할 목표 각도
    private Quaternion newRotationY; // Camera Rig 가 Y축으로 새로 회전할 목표 각도
    private Vector3 newZoom; // Camera Orbit 이 새로 zoom-in/out 할 위치

    private Vector3 dragStartPosition; // 마우스 좌측 드래그 시작지점 좌표 저장
    private Vector3 dragCurrentPosition; // 마우스 좌측 드래그 현재지점 좌표 저장
    private Vector3 rotateStartPosition; // 마우스 우측 드래그 시작지점 좌표 저장
    private Vector3 rotateCurrentPosition; // 마우스 우측 드래그 현재지점 좌표 저장

    // 새로 이동할 위치 및 회전값 초기화
    private void Start()
    {
        newPosition = transform.position;
        newRotationX = cameraOrbitTransform.localRotation;
        newRotationY = transform.rotation;
        newZoom = cameraOrbitTransform.localPosition; // Camera 게임 오브젝트의 로컬 좌표로 초기화 (인스펙터 창에 표시되는 부모 오브젝트 기준 좌표계)
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMouseInput();
        HandleKeyInput();
        Move();
    }

    private void HandleMouseInput()
    {
        if (InputManager.instance.mouseScrollY != 0)
        {
            UpdateNewZoom(InputManager.instance.mouseScrollY * zoomScale); // 마우스 수직 스크롤 간격을 단위 줌에 곱해서 zoomAmount 계산
        }

        // 마우스 왼쪽 클릭 처리
        if (InputManager.instance.mouseLeftClick)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero); // 평면 구조체 생성 (평면의 노멀벡터, 평면의 원점)

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 현재 활성화된 카메라를 기준으로 스크린 좌표(마우스 클릭 좌표)를 기준으로 Ray 생성

            float entry; // 레이의 출발점 ~ 평면과 레이의 교차점까지의 거리가 저장됨.

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry); // Ray 상에서 평면과 교차된 지점의 좌표를 저장함 (드래그 시작 지점)
            }
        }

        // 마우스 왼쪽 드래그 처리
        if (InputManager.instance.mouseLeftDrag)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero); // 평면 구조체 생성 (평면의 노멀벡터, 평면의 원점)

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 현재 활성화된 카메라를 기준으로 스크린 좌표(마우스 클릭 좌표)를 기준으로 Ray 생성

            float entry; // 레이의 출발점 ~ 평면과 레이의 교차점까지의 거리가 저장됨.

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry); // Ray 상에서 평면과 교차된 지점의 좌표를 저장함 (현재 드래그 지점)

                newPosition = transform.position + dragStartPosition - dragCurrentPosition; // 목표 위치를 현재 위치에 드래그 거리만큼을 더해서 계산
            }
        }

        // 마우스 오른쪽 클릭 처리
        if (InputManager.instance.mouseRightClick)
        {
            rotateStartPosition = Input.mousePosition; // 마우스 우클릭 좌표값 저장 (드래그 시작점)
        }

        // 마우스 오른쪽 드래그 처리
        if (InputManager.instance.mouseRightDrag)
        {
            rotateCurrentPosition = Input.mousePosition; // 마우스 우측 드래그 좌표값 저장 (드래그 현재위치)

            Vector3 difference = rotateStartPosition - rotateCurrentPosition; // 마우스 이동거리 저장 (스크린 좌표 기준)

            rotateStartPosition = rotateCurrentPosition; // 마우스 드래그 시작점 갱신

            newRotationY *= Quaternion.Euler(Vector3.up * (-difference.x / rotationSensitivity)); // 마우스 수평방향(x축) 이동거리만큼 회전 (Y축 회전)

            Quaternion rotationX = newRotationX * Quaternion.Euler(Vector3.right * (-difference.y / rotationSensitivity)); // 마우스 수직방향(y축) 이동거리만큼 회전 (X축 회전)

            if (rotationX.eulerAngles.x >= minRotationX && rotationX.eulerAngles.x <= maxRotationX)
            {
                newRotationX = rotationX; // rotationX 의 x값이 정해진 범위 내에 있을 때 newRotationX 업데이트
            }
        }
    }

    // 사용자 입력에 따라 카메라 이동 및 회전 계산
    private void HandleKeyInput()
    {
        if (InputManager.instance.speedUp)
        {
            moveSpeed = fastSpeed;
        }
        else
        {
            moveSpeed = normalSpeed;
        }

        if (InputManager.instance.moveForward)
        {
            newPosition += (transform.forward * moveSpeed);
        }
        if (InputManager.instance.moveBackward)
        {
            newPosition += (transform.forward * -moveSpeed);
        }
        if (InputManager.instance.moveRight)
        {
            newPosition += (transform.right * moveSpeed);
        }
        if (InputManager.instance.moveLeft)
        {
            newPosition += (transform.right * -moveSpeed);
        }

        if (InputManager.instance.rotateClockwise)
        {
            newRotationY *= Quaternion.Euler(Vector3.up * rotationAmount); // y축 회전. 쿼터니언 각 A 에서 B 만큼 더 회전 = A * B
        }
        if (InputManager.instance.rotateAntiClockwise)
        {
            newRotationY *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (InputManager.instance.zoomIn)
        {
            UpdateNewZoom(-zoomScale);
        }
        if (InputManager.instance.zoomOut)
        {
            UpdateNewZoom(zoomScale);
        }
    }

    private void Move()
    {
        // Camera 현재 transform 에서 목표 transform 까지 선형보간
        // moveTime 값이 작을수록 현재 transform(0)에 가깝게 보간됨 -> 목표 transform(1)까지 도달하는 데 시간이 더 걸림 -> 관성(Inertia)효과 뚜렷해짐
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * moveTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotationY, Time.deltaTime * moveTime);
        cameraOrbitTransform.localPosition = Vector3.Lerp(cameraOrbitTransform.localPosition, newZoom, Time.deltaTime * moveTime);
        cameraOrbitTransform.localRotation = Quaternion.Lerp(cameraOrbitTransform.localRotation, newRotationX, Time.deltaTime * moveTime);
    }

    private void UpdateNewZoom(float zoomAmount)
    {
        float cameraOrbitRotationX = cameraOrbitTransform.localRotation.eulerAngles.x; // 현재 Camera Orbit 의 로컬 x축 각도

        // Vector3.forward, cameraOrbitTransform.forward, cameraOrbitTransform.InverseTransformDirection(cameraOrbitTransform.forward) 방향벡터 부정확
        // -> 로컬 방향벡터 직접 계산
        Vector3 localForward = Quaternion.AngleAxis(cameraOrbitRotationX, Vector3.right) * Vector3.forward; // 앞쪽 방향벡터(0, 0, 1)를 x축으로 회전시켜 로컬 forward 벡터 계산

        Vector3 zoom = newZoom + localForward * zoomAmount; // 로컬 forward 방향벡터에 zoomAmount 만큼 늘려서 zoom 값 계산

        if (zoom.y >= minZoom && zoom.y <= maxZoom)
        {
            newZoom = zoom; // zoom y값이 정해진 범위 내에 있을 때 newZoom 업데이트
        }
    }
}
