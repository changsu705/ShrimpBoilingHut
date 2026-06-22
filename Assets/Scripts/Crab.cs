using DG.Tweening;
using UnityEngine;

public class Crab : MonoBehaviour
{
    public int CrabType;

    public bool hasMerged = false;

    public bool isDropped = false;

    public AudioClip mergeSound;

    private Collider2D myCollider;
    private Rigidbody2D myRigidbody;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }


    // Unity 메시지 | 참조 0개
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasMerged)                                                     // 이미 합쳐진 갑각류는 무시
            return;

        Crab otherCrab = collision.gameObject.GetComponent<Crab>();        // 다른 갑각류와 충돌 했는지 확인

        if (otherCrab != null && !otherCrab.hasMerged && otherCrab.CrabType == CrabType)     // 충돌한 것이 갑각류고 타입이 같으면(합쳐지지 않았을 경우)
        {
            // ★ 추가: 현재 타입이 매니저에 등록된 프리팹의 마지막 인덱스(최고 단계)라면 머지하지 않고 리턴
            if (this.CrabType >= CrabGame.Instance.CrabPrefabs.Length - 1)
            {
                return; // 함수를 여기서 종료하여 서로 그냥 밀쳐내기만 하고 합쳐지지 않음
            }

            Debug.Log("갑각류 충돌함!!!");
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySound(mergeSound);
                Debug.Log("소리 재생됨!");
            }
            hasMerged = true;                                                                // 합쳐짐 표시
            otherCrab.hasMerged = true;

            Vector3 mergePosition = (transform.position + otherCrab.transform.position) / 2f;      // 두 과일의 중간 위치 계산

            CrabGame gameManager = FindAnyObjectByType<CrabGame>();
            if (gameManager != null)
            {
                gameManager.MergeCrabs(CrabType, mergePosition);
                Debug.Log("갑각류 합쳐짐!!!");
            }

            Destroy(otherCrab.gameObject);
            Destroy(gameObject);
        }
    }
    public void TriggerMergeAnimation(Vector3 targetScale)
    {
        // 1. 생성 순간 물리와 콜라이더를 잠시 꺼서 끼임 원천 차단
        if (myCollider != null) myCollider.enabled = false;
        if (myRigidbody != null) myRigidbody.simulated = false;

        // 2. 시작 크기는 0으로 설정
        transform.localScale = Vector3.zero;

        // 3. 매니저가 준 목표 크기(targetScale)로 0.15초 동안 키우기
        transform.DOScale(targetScale, 0.3f)
            .SetEase(Ease.OutBack) // 뿅~ 하고 살짝 커졌다 자리잡는 연출
            .OnComplete(() =>
            {
                // 4. 애니메이션이 끝나면 안전하게 물리와 콜라이더 켜기
                if (myCollider != null) myCollider.enabled = true;
                if (myRigidbody != null) myRigidbody.simulated = true;
            });
    }
}
