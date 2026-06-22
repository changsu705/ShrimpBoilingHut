using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening.Core.Easing;
using UnityEngine.SceneManagement;

public class CrabGame : MonoBehaviour   
{
    // Unity 스크립트 | 참조 0개
    [Header("UI 세팅")]
    [SerializeField] private GameObject gameOverUI; // GameOver 부모 오브젝트
    [SerializeField] private TextMeshProUGUI scoreText; // Text (TMP) (1) 오브젝트

    [Header("집게 오브젝트 제어 제어")]
    public GameObject clawGrabObject;    // 잡았을 때의 집게 오브젝트 (다문 손)
    public GameObject clawReleaseObject; // 놓았을 때의 집게 오브젝트 (편 손)

    public static CrabGame Instance;
    public GameObject[] CrabPrefabs;                        //갑각류 프리팹 배열 선언
    public float[] CrabSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };    //갑각류 크기 선언

    public GameObject currentCrab;                          //현재 들고 있는 갑각류
    public int currentCrabType;                             //현재 들고 있는 갑각류 타입

    public float CrabStartHeight = 6.0f;                    //갑각류 시작시 높이 설정
    public float gameWidth = 5.0f;                           //게임판 너비
    public bool isGameOver = false;                          //게임 상태
    public Camera MainCamera;                                //카메라 참조 (마우스 위치 변환에 필요)
    public float gameScore = 0f;

    public Transform targetTransform;
    public Transform Crabtransform;

    public float crabTimer;

    public float[] CrapScores = { 100, 250, 550, 1150, 2350, 4750, 9550, 19050 };

    [Header("클리어 게이지 시스템")]
    public float maxGauge = 100f;
    private float currentGauge = 0f;

    public float[] ClearRewardTable = { 2f, 4f, 6f, 9f, 13f, 18f, 24f, 31f, 39f, 48f, 60f };
    public float ClearDecrease = 3f;
    public float ClearDecreaseRate = 3f;
    private float noMergeTimer = 0f;
    private bool isTimerRunning = false;
    public Slider ClearSlider;
    public float Clearrate;
    public int ClearCounter = 0;

    public TMP_Text UIScore;




    // Unity 메시지 | 참조 0개
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 전역에서 CrabGame에 접근할 수 있도록 나 자신을 등록함
        }
        else
        {
            Destroy(gameObject); // 혹시 모를 중복 생성 방지
        }
    }

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
                                                                 //손의 위치를 위한 오프셋(X +0.4, Y +2.13) 계산
            Vector3 claw = new Vector3(newPoisition.x + 0.4f, newPoisition.y + 0.48f , newPoisition.z);

            clawGrabObject.transform.position = claw;    //히히 손도 꼽껴야지 (오프셋 적용!)
            clawReleaseObject.transform.position = claw; //히히 손도 꼽껴야지 (오프셋 적용!)

        }

        if (Mouse.current.leftButton.wasPressedThisFrame && crabTimer == -3.0f)
        {
            DropCrab();
        }

        // 게이지가 있을 때만 타이머와 감소 로직이 작동함
        if (currentGauge > 0)
        {
            noMergeTimer += Time.deltaTime;

            // 3초 동안 머지가 일어나지 않았다면 게이지 감소 시작
            if (noMergeTimer >= ClearDecreaseRate)
            {
                currentGauge -= ClearDecrease * Time.deltaTime;
                currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);
                UpdateGaugeUI();
            }
        }
        UIScore.text = "점수 : " + gameScore.ToString();
    }

    public void SpawnNewCrab(Vector3 targetPosition)                                     //갑각류 생성 함수
    {
        if (!isGameOver)                                     //게임 오버가 아닐 때만 새 과일 생성
        {
            // 새로운 갑각류가 로드될 때: 잡은 손 켜고, 편 손 끄기
            if (clawGrabObject != null) clawGrabObject.SetActive(true);
            if (clawReleaseObject != null) clawReleaseObject.SetActive(false);

            currentCrabType = Random.Range(0, 2); //0~2 사이의 랜덤 갑각류 타입

            // 매개변수로 받아온 Vector3의 x, y 값을 직접 활용합니다.
            float targetX = targetPosition.x;
            float targetY = CrabStartHeight;

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
        if (clawGrabObject != null) clawGrabObject.SetActive(false);
        if (clawReleaseObject != null) clawReleaseObject.SetActive(true);

        Rigidbody2D rb = currentCrab.GetComponent<Rigidbody2D>();
        Crab CrabScript = currentCrab.GetComponent<Crab>();

        if (CrabScript != null)
        {
            CrabScript.isDropped = true;
        }
        else
        {
            Debug.Log("Crab.cs 검색안됌 프리팹 롤백 필요.");
        }

        if (rb != null)
        {
            rb.gravityScale = 3f;
            currentCrab = null;
            crabTimer = 1.0f;
        }
    }

    public void MergeCrabs(int CrapType, Vector3 positioing)
    {
        gameScore += CrapScores[CrapType];
        Debug.Log("갑각류 합쳐짐!" + CrapScores[CrapType] + "점 추가됨");
        Debug.Log("현제 점수" + gameScore);
        noMergeTimer = 0f;
        currentGauge += ClearRewardTable[CrapType];
        currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);
        UpdateGaugeUI();

        if (currentGauge >= maxGauge)
        {
            TriggerScreenClear();
            return; // 전체 클리어가 되었으므로 다음 과일 생성은 건너뜁니다.
        }

        if (CrapType < CrabPrefabs.Length - 1)
        {
            // 1. 다음 단계 갑각류 생성
            GameObject newCrap = Instantiate(CrabPrefabs[CrapType + 1], positioing, Quaternion.identity);

            // 2. 매니저의 CrabSizes 배열을 기준으로 최종 목표 크기 계산
            float targetSize = CrabSizes[CrapType + 1];
            Vector3 targetScale = new Vector3(targetSize, targetSize, 1.0f);

            // 3. 생성된 갑각류의 Crab 스크립트를 가져옴
            Crab crabScript = newCrap.GetComponent<Crab>();

            if (crabScript != null)
            {
                // 머지되어 새로 태어난 애는 이미 맵에 떨어진 상태이므로 true 처리 (데스라인 인식용)
                crabScript.isDropped = true;

                // ★ DOTween 애니메이션 실행 (목표 크기를 매개변수로 토스!)
                crabScript.TriggerMergeAnimation(targetScale);
            }
            else
            {
                // 혹시 스크립트가 없다면 예외처리로 그냥 바로 크기 변경
                newCrap.transform.localScale = targetScale;
            }
        }
    }

    private void TriggerScreenClear()
    {
        Debug.Log("★ 게이지 완충! 화면의 모든 과일을 제거합니다! ★");

        // 씬에 존재하는 모든 Crab 스크립트를 가진 오브젝트를 찾음
        Crab[] allcrabs = FindObjectsOfType<Crab>();

        foreach (Crab crab in allcrabs)
        {
            // 여기서 갑각류 터지는 이펙트추가.
            // Instantiate(clearEffectPrefab, fruit.transform.position, Quaternion.identity);
            Destroy(crab.gameObject);
        }

        // 게이지 및 타이머 초기화
        currentGauge = 0f;
        noMergeTimer = 0f;
        UpdateGaugeUI();
    }

    private void UpdateGaugeUI()
    {
        if (ClearSlider != null)
        {
            ClearSlider.value = currentGauge / maxGauge;
        }
    }
    public void GameOver()
    {
        if (isGameOver) return; // 중복 실행 방지

        isGameOver = true;
        Debug.Log("★★★ GAME OVER ★★★");

        // 시간 정지 혹은 결과창 팝업 띄우기
        Time.timeScale = 0f;
        EndGame(gameScore);
        // 예: GameOverUI.SetActive(true);
    }
    public void EndGame(float finalScore)
    {
        // 1. 게임오버 UI 켜기
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // 2. 점수 텍스트 변경하기
        if (scoreText != null)
            scoreText.text = "점수 : " + finalScore.ToString();

        // 필요하다면 게임오버 시 시간을 멈출 수도 있습니다.
        // Time.timeScale = 0f; 
    }
    public void RestartGame()
    {
        // 시간을 멈췄었다면 다시 흐르게 합니다.
        Time.timeScale = 1f;

        // 현재 열려있는 씬의 이름을 가져와서 다시 로드(재시작)합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickGoToLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

}