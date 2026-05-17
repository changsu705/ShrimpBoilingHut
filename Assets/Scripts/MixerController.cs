using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerController : MonoBehaviour
{
    [Header("[오디오 믹서 지정]")]
    [SerializeField] private AudioMixer m_AudioMixer;

    [Header("[2D 실린더 슬라이더 UI 지정]")]
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    private void Awake()
    {
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
    }

    public void SetMasterVolume(float volume)
    {
        // 실린더 슬라이더 값이 완전히 0(최하)이 되면 로그 계산을 거치지 않고 바로 무음(-80dB) 처리
        if (volume <= 0.0001f)
        {
            m_AudioMixer.SetFloat("Master", -80f);
        }
        else
        {
            // 로그 공식을 사용해 자연스러운 볼륨 곡선 적용
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