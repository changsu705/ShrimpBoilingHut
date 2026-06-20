using UnityEngine;

public class SoundManager : MonoBehaviour
{
   
    public static SoundManager instance;

    public AudioSource effectSource; 

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); // 이미 존재하면 삭제
        }
    }

    // 외부에서 효과음을 재생할 때 호출하는 함수
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            effectSource.PlayOneShot(clip);
        }
    }
}