using UnityEngine;

public class CrabGame : MonoBehaviour
{


// Unity 스크립트 | 참조 0개

    public GameObject[] CrabPrefabs;                        //과일 프리팹 배열 선언
    public float[] CrabSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };    //과일 크기 선언

    public GameObject currentCrab;                          //현재 들고 있는 과일
    public int currentCrabType;                             //현재 들고 있는 과일 타입

    public float CrabStartHeight = 6.0f;                    //과일 시작시 높이 설정
    public float gameWidth = 5.0f;                           //게임판 너비
    public bool isGameOver = false;                          //게임 상태
    public Camera mainCamera;                                //카메라 참조 (마우스 위치 변환에 필요)

    // Unity 메시지 | 참조 0개

    // 참조 1개
    public void SpawnNewCrab(Transform targetTransform)                                     //과일 생성 함수
    {
        if (!isGameOver)                                     //게임 오버가 아닐 때만 새 과일 생성
        {
            currentCrabType = Random.Range(0, 3); //0~2 사이의 랜덤 과일 타입

            // [수정] 마우스 좌표 계산 코드를 지우고, 전달받은 오브젝트의 X 좌표를 사용합니다.
            float targetX = targetTransform.position.x;
            float targetY = targetTransform.position.y;

            Vector3 spawnPosition = new Vector3(targetX, targetY, 0);      //X 좌표만 사용 하고 나머지는 설정한 값으로 한다.

            float halfCrabSize = CrabSizes[currentCrabType] / 2f;

            //X 의 위치가 게임 영역을 벗어나지 않도록 제한
            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfCrabSize, gameWidth / 2 - halfCrabSize);

            currentCrab = Instantiate(CrabPrefabs[currentCrabType], spawnPosition, Quaternion.identity); //과일 생성
            currentCrab.transform.localScale = new Vector3(CrabSizes[currentCrabType], CrabSizes[currentCrabType], 1); //과일 크기 설정

            Rigidbody2D rb = currentCrab.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.gravityScale = 0f;                        //시작 시에는 중력 스케일을 0 으로 해준다.
            }
        }
    }

}

