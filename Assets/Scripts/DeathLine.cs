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
    public GameObject warningUI; // 화면에 띄울 "WARNING!" 텍스트나 깜빡이는 빨간 선 UI

    private void Update()
    {
        // 1. 혹시 리스트 안에서 파괴(Merge 등으로 삭제)된 과일이 있다면 리스트에서 제거 (유니티 필수 예외처리)
        fruitsInZone.RemoveAll(item => item == null);

        // 2. 라인 안에 떨어진 과일이 하나라도 있다면 타이머 가동
        if (fruitsInZone.Count > 0)
        {
            warningTimer += Time.deltaTime;

            // 경고 UI 활성화 (깜빡이는 연출 등을 추가하면 좋습니다)
            if (warningUI != null) warningUI.SetActive(true);

            Debug.Log($"[경고] 과일이 선을 넘었습니다! 게임오버까지: {maxWarningTime - warningTimer:F1}초");

            // 3초 제한시간 초과 시 게임오버
            if (warningTimer >= maxWarningTime)
            {
                CrabGame.Instance.GameOver();
            }
        }
        else
        {
            // 라인이 깨끗해지면 타이머 및 UI 리셋
            warningTimer = 0f;
            if (warningUI != null) warningUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Crab crab = collision.GetComponent<Crab>();

        // 과일 컴포넌트가 있고, 사용자가 조작 중인 과일이 아닌 '이미 떨어진 과일'만 체크
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