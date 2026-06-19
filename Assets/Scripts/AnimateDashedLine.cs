using UnityEngine;

public class AnimateDashedLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private float scrollSpeed = 2.0f; // 점선이 움직이는 속도

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // 시간에 따라 X축 오프셋을 변화시킵니다.
        float offset = Time.time * scrollSpeed;

        // 머티리얼의 메인 텍스처 오프셋을 변경하여 움직이는 효과를 줍니다.
        lineRenderer.material.mainTextureOffset = new Vector2(-offset, 0);
    }
}