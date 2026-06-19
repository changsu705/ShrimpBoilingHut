using System.Collections.Generic;
using UnityEngine;

public class DeathLine : MonoBehaviour
{
    [Header("게임오버 유예 시간")]
    public float maxWarningTime = 3f;
    private float warningTimer = 0f;

    // 현재 데스라인 영역 안에 들어와 있는 과일들을 추적하는 리스트
    private List<Crab> fruitsInZone = new List<Crab>();

    [Header("시각 연출 (선택)")]
    public GameObject warningUI; // 화면에 띄울 "WARNING!" 텍스트나 UI

    [Header("라인 렌더러 연출")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Color normalColor = Color.white; // 평상시 라인 색상
    [SerializeField] private Color warningColor = Color.red;   // 경고시 변할 빨간색
    [SerializeField] private float flashSpeed = 5f;            // 깜빡이는 속도

    private void Start()
    {
        // 만약 에디터에서 라인 렌더러를 직접 넣지 않았다면 자동으로 찾아옵니다.
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // 시작할 때는 평상시 색상으로 세팅
        SetLineColor(normalColor);
    }

    private void Update()
    {
        // 1. 혹시 리스트 안에서 파괴(Merge 등으로 삭제)된 과일이 있다면 리스트에서 제거
        fruitsInZone.RemoveAll(item => item == null);

        // 2. 라인 안에 떨어진 과일이 하나라도 있다면 타이머 가동
        if (fruitsInZone.Count > 0)
        {
            warningTimer += Time.deltaTime;

            if (warningUI != null) warningUI.SetActive(true);

            // 🎨 [추가] 라인 색상 깜빡임 연출 (Mathf.PingPong을 이용해 0~1 사이를 부드럽게 왕복)
            float lerpFactor = Mathf.PingPong(Time.time * flashSpeed, 1f);
            Color blendedColor = Color.Lerp(normalColor, warningColor, lerpFactor);
            SetLineColor(blendedColor);

            Debug.Log($"[경고] 과일이 선을 넘었습니다! 게임오버까지: {maxWarningTime - warningTimer:F1}초");

            // 3초 제한시간 초과 시 게임오버
            if (warningTimer >= maxWarningTime)
            {
                CrabGame.Instance.GameOver();
            }
        }
        else
        {
            // 라인이 깨끗해지면 타이머 및 UI 리셋, 색상도 원상복구
            warningTimer = 0f;
            if (warningUI != null) warningUI.SetActive(false);

            SetLineColor(normalColor);
        }
    }

    // 라인 렌더러의 시작(Start)색상과 끝(End)색상을 한 번에 바꿔주는 편의용 함수
    private void SetLineColor(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Crab crab = collision.GetComponent<Crab>();

        if (crab != null && crab.isDropped)
        {
            if (!fruitsInZone.Contains(crab))
            {
                fruitsInZone.Add(crab);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Crab crab = collision.GetComponent<Crab>();

        if (crab != null)
        {
            if (fruitsInZone.Contains(crab))
            {
                fruitsInZone.Remove(crab);
            }
        }
    }
}