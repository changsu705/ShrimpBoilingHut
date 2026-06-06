using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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



        // 슬라이더 값이 변경될 때마다 각각의 볼륨 조절 함수를 실행하도록 연결

        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);

        m_MusicBGMSlider.onValueChanged.AddListener(SetMusicVolume);

        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);

    }



    private void Start()

    {

        // 게임 시작할 때 현재 슬라이더에 세팅된 값으로 볼륨 초기화

        SetMasterVolume(m_MusicMasterSlider.value);

        SetMusicVolume(m_MusicBGMSlider.value);

        SetSFXVolume(m_MusicSFXSlider.value);



        // 게임 시작할 때 사운드 UI 창은 기본적으로 꺼두기

        if (m_SoundUIObject != null)

        {

            m_SoundUIObject.SetActive(false);

        }

    }



    // 매 프레임마다 키보드 ESC 입력을 감지

    private void Update()

    {

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)

        {

            ToggleSoundUI();

        }

    }



    // UI를 토글(켜고 끄기)하는 함수

    public void ToggleSoundUI()

    {

        if (m_SoundUIObject != null)

        {

            bool isActive = m_SoundUIObject.activeSelf;

            m_SoundUIObject.SetActive(!isActive);

        }

        else

        {

            Debug.LogWarning("MixerController: m_SoundUIObject가 인스펙터에서 지정되지 않았습니다!");

        }

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