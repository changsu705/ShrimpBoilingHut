using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening; // 잊지 말고 꼭 추가!

public class MixerController : MonoBehaviour
{
    // 어디서나 이 믹서 컨트롤러에 접근할 수 있도록 static 인스턴스 선언
    public static MixerController instance { get; private set; }

    [Header("[오디오 믹서 지정]")]
    [SerializeField] private AudioMixer m_AudioMixer;

    [Header("[2D 실린더 슬라이더 UI 지정]")]
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    [Header("[ESC로 켜고 끌 UI 지정]")]
    [SerializeField] private GameObject m_SoundUIObject;
    
    [Header("[새로 추가됨: UI 컴포넌트 & 버튼]")]
    [SerializeField] private Button m_CloseButton;         // 닫기 버튼 지정용
    [SerializeField] private CanvasGroup m_CanvasGroup;    // UI 페이드 연출용 (알파값 조절)

    [Header("[새로 추가됨: 연출 속도]")]
    [SerializeField] private float m_FadeDuration = 0.4f;  // UI가 열고 닫히는 시간

    private bool isTweening = false; // 연출 중 중복 입력 방지용 플래그

    private void Awake()
    {
        // 싱글톤 검사 및 방어 코드
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않고 유지
        }
        else
        {
            Destroy(gameObject); // 이미 존재한다면 새로 생겨난 중복 매니저 파괴
            return; // 아래 리스너 등록 코드가 실행되지 않도록 즉시 종료
        }

        
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);

        
        if (m_CloseButton != null)
        {
            m_CloseButton.onClick.AddListener(CloseSoundUI);
        }
    }

    private void Start()
    {
        
        SetMasterVolume(m_MusicMasterSlider.value);
        SetMusicVolume(m_MusicBGMSlider.value);
        SetSFXVolume(m_MusicSFXSlider.value);

     
        if (m_SoundUIObject != null)
        {
            if (m_CanvasGroup != null) m_CanvasGroup.alpha = 0f;
            m_SoundUIObject.SetActive(false);
        }
    }


    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            
            if (isTweening) return;

            if (m_SoundUIObject.activeSelf)
            {
                CloseSoundUI(); 
            }
            else
            {
                OpenSoundUI(); 
            }
        }
    }


    public void OpenSoundUI()
    {
        if (m_SoundUIObject == null || isTweening) return;

        isTweening = true;
        m_SoundUIObject.SetActive(true); 

        if (m_CanvasGroup != null)
        {
            m_CanvasGroup.alpha = 0f; 
            m_CanvasGroup.DOKill();
           
            m_CanvasGroup.DOFade(1f, m_FadeDuration).OnComplete(() => isTweening = false);
        }
        else
        {
            isTweening = false;
        }

    
        m_AudioMixer.DOKill();
        m_AudioMixer.DOSetFloat("BGM", -20f, m_FadeDuration);
    }

   
    public void CloseSoundUI()
    {
        if (m_SoundUIObject == null || isTweening) return;

        isTweening = true;

        if (m_CanvasGroup != null)
        {
            m_CanvasGroup.DOKill();
           
            m_CanvasGroup.DOFade(0f, m_FadeDuration).OnComplete(() =>
            {
                m_SoundUIObject.SetActive(false); 
                isTweening = false;
            });
        }
        else
        {
            m_SoundUIObject.SetActive(false);
            isTweening = false;
        }

      
        m_AudioMixer.DOKill();
        float originalBGMVolume = m_MusicBGMSlider.value;
        float targetdB = (originalBGMVolume <= 0.0001f) ? -80f : Mathf.Log10(originalBGMVolume) * 20;
        
        m_AudioMixer.DOSetFloat("BGM", targetdB, m_FadeDuration);
    }


    public void SetMasterVolume(float volume)
    {
        if (volume <= 0.0001f)
        {
            m_AudioMixer.SetFloat("Master", -80f);
        }
        else
        {
            m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }
    }

    public void SetMusicVolume(float volume)
    {
        // UI가 완전히 켜져서 연출 중이 아닐 때만 슬라이더 조작으로 믹서 볼륨 변경 가능하게 보호
        if (isTweening) return;

        if (volume <= 0.0001f)
        {
            m_AudioMixer.SetFloat("BGM", -80f);
        }
        else
        {
            m_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (volume <= 0.0001f)
        {
            m_AudioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            m_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }
    }
}