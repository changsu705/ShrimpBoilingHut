using UnityEngine;
using UnityEngine.InputSystem;

public class MouseTrackingYOnly2D : MonoBehaviour
{
    private float fixedY;

    void Start()
    {
        // 게임 시작시 오브젝트의 y좌표값 기억
        fixedY = transform.position.y;
    }

    void Update()
    {
        // 카메라 기준 마우스 포인터 위치로 좌표 저장
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane));

        // y 좌표값을 제외한 포인터 위치 좌표로 오브젝트 이동
        transform.position = new Vector3(mouseWorldPos.x, fixedY, transform.position.z);
    }
}