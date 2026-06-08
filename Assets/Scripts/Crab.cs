using UnityEngine;

public class Crab : MonoBehaviour
{
    public int CrabType;

    public bool hasMerged = false;

    // Unity 메시지 | 참조 0개
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasMerged)                                                     // 이미 합쳐진 갑각류는 무시
            return;

        Crab otherCrab = collision.gameObject.GetComponent<Crab>();        // 다른 갑각류와 충돌 했는지 확인

        if (otherCrab != null && !otherCrab.hasMerged && otherCrab.CrabType == CrabType)     // 충돌한 것이 갑각류고 타입이 같으면(합쳐지지 않았을 경우)
        {
            Debug.Log("갑각류 충돌함!!!");
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

}
