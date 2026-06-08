using UnityEngine;
using UnityEngine.InputSystem;

public class CrabGame : MonoBehaviour
{
    // Unity 스크립트 | 참조 0개

    public GameObject[] CrabPrefabs;                        //갑각류 프리팹 배열 선언
    public float[] CrabSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };    //갑각류 크기 선언

    public GameObject currentCrab;                          //현재 들고 있는 갑각류
    public int currentCrabType;                             //현재 들고 있는 갑각류 타입

    public float CrabStartHeight = 6.0f;                    //갑각류 시작시 높이 설정
    public float gameWidth = 5.0f;                           //게임판 너비
    public bool isGameOver = false;                          //게임 상태
    public Camera MainCamera;                                //카메라 참조 (마우스 위치 변환에 필요)

    public Transform targetTransform;
    public Transform Crabtransform;

    public float crabTimer;

    public float[] CrapScores = { 100, 250, 550, 1150, 2350, 4750, 9550, 1 };

    // Unity 메시지 | 참조 0개

    void Start()
    {
        MainCamera = Camera.main;
        SpawnNewCrab(transform.position);
        crabTimer = -3.0f;
    }

    void Update()
    {
        if (isGameOver) return;                        //게임 오버면 리턴

        if (crabTimer >= 0)
        {
            crabTimer -= Time.deltaTime;
        }

        if (crabTimer < 0 && crabTimer > -2) 
        {
            SpawnNewCrab(transform.position);
            crabTimer = -3.0f;
        }

        // 현재 생성된 갑각류가 있고, 추적할 대상 오브젝트가 지정되어 있을 때만 처리
        if (currentCrab != null && targetTransform != null)
        {
            float targetWorldX = targetTransform.position.x;


            Vector3 newPoisition = currentCrab.transform.position;
            newPoisition.x = targetWorldX;             //갑각류 위치 업데이트

            float halfFruitSize = CrabSizes[currentCrabType] / 2f;

            // X의 위치가 왼쪽 벽을 벗어나지 않도록 제한
            if (newPoisition.x < -gameWidth / 2 + halfFruitSize)
            {
                newPoisition.x = -gameWidth / 2 + halfFruitSize;
            }

            // 오른쪽 벽 제한 공식 (-로 수정 완료)
            if (newPoisition.x > gameWidth / 2 - halfFruitSize)
            {
                newPoisition.x = gameWidth / 2 - halfFruitSize;
            }

            currentCrab.transform.position = newPoisition;       //갑각류 좌표 갱신
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && crabTimer == -3.0f)
        {
            DropCrab();
        }
    }

    // [수정] 외부에서 transform.position(Vector3)으로 직접 참조할 수 있도록 매개변수 타입을 Vector3로 변경했습니다.
    public void SpawnNewCrab(Vector3 targetPosition)                                     //갑각류 생성 함수
    {
        if (!isGameOver)                                     //게임 오버가 아닐 때만 새 과일 생성
        {
            currentCrabType = Random.Range(0, 3); //0~2 사이의 랜덤 갑각류 타입

            // 매개변수로 받아온 Vector3의 x, y 값을 직접 활용합니다.
            float targetX = targetPosition.x;
            float targetY = targetPosition.y;

            Vector3 spawnPosition = new Vector3(targetX, targetY, 0);

            float halfCrabSize = CrabSizes[currentCrabType] / 2f;

            //X 의 위치가 게임 영역을 벗어나지 않도록 제한
            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfCrabSize, gameWidth / 2 - halfCrabSize);

            currentCrab = Instantiate(CrabPrefabs[currentCrabType], spawnPosition, Quaternion.identity); //갑각류 생성
            currentCrab.transform.localScale = new Vector3(CrabSizes[currentCrabType], CrabSizes[currentCrabType], 1); //갑각류 크기 설정

            Rigidbody2D rb = currentCrab.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.gravityScale = 0f;                        //시작 시에는 중력 스케일을 0 으로 해준다.
            }
        }
    }

    public void DropCrab()
    {
        Rigidbody2D rb = currentCrab.GetComponent<Rigidbody2D>();

        if(rb != null)
        {
            rb.gravityScale = 3f;
            currentCrab = null;
            crabTimer = 1.0f;
        }
    }

    public void MergeCrabs(int CrapType, Vector3 positioing)
    {
        if (CrapType < CrabPrefabs.Length - 1)
        {
            GameObject newCrap = Instantiate(CrabPrefabs[CrapType + 1], positioing, Quaternion.identity);
            newCrap.transform.localScale = new Vector3(CrabSizes[CrapType + 1], CrabSizes[CrapType + 1], 1.0f);
        }
    }
}